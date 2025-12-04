using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;
using FaceRecognitionAttendance.Services.Camera;
using FaceRecognitionAttendance.Services.FaceRecognition;
using FaceRecognitionAttendance.Helpers;

namespace FaceRecognitionAttendance.ViewModels
{
    /// <summary>
    /// ViewModel for Register User Window
    /// Handles user registration with face capture (80 images)
    /// </summary>
    public class RegisterUserViewModel : BaseViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly ICameraService _cameraService;
        private readonly IFaceDetectionService _faceDetectionService;
        private readonly IFaceRecognitionService _faceRecognitionService;

        private System.Windows.Threading.DispatcherTimer? _frameTimer;
        private List<Mat> _capturedFaces = new List<Mat>();
        private const int REQUIRED_IMAGES = 80;

        public RegisterUserViewModel(
            IUserRepository userRepository,
            ICameraService cameraService,
            IFaceDetectionService faceDetectionService,
            IFaceRecognitionService faceRecognitionService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
            _faceDetectionService = faceDetectionService ?? throw new ArgumentNullException(nameof(faceDetectionService));
            _faceRecognitionService = faceRecognitionService ?? throw new ArgumentNullException(nameof(faceRecognitionService));

            User = new User
            {
                RegistrationDate = DateTime.Now,
                IsActive = true
            };

            // Initialize commands
            StartCaptureCommand = new RelayCommand(StartCapture, CanStartCapture);
            StopCaptureCommand = new RelayCommand(StopCapture, () => IsCapturing);
            SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Initialize data
            InitializeData();
        }

        #region Properties

        private User _user = null!;
        public User User
        {
            get => _user;
            set
            {
                if (SetProperty(ref _user, value))
                {
                    UpdateVisibility();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public List<string> Departments { get; set; } = new List<string>
        {
            "Computer Science",
            "Electrical Engineering",
            "Mechanical Engineering",
            "Civil Engineering",
            "Business Administration",
            "Mathematics",
            "Physics",
            "Chemistry",
            "Biology"
        };

        public List<UserType> UserTypes { get; set; } = new List<UserType>
        {
            UserType.Student,
            UserType.Faculty
        };

        public List<FacultyType> FacultyTypes { get; set; } = new List<FacultyType>
        {
            FacultyType.MonthlyRegular,
            FacultyType.VisitingFixed,
            FacultyType.VisitingPerDay
        };

        private BitmapSource? _cameraFrame;
        public BitmapSource? CameraFrame
        {
            get => _cameraFrame;
            set => SetProperty(ref _cameraFrame, value);
        }

        private bool _isCapturing;
        public bool IsCapturing
        {
            get => _isCapturing;
            set
            {
                if (SetProperty(ref _isCapturing, value))
                {
                    ((RelayCommand)StartCaptureCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)StopCaptureCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private int _capturedCount;
        public int CapturedCount
        {
            get => _capturedCount;
            set
            {
                if (SetProperty(ref _capturedCount, value))
                {
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _statusMessage = "Fill in user details and click 'Start Capture' to begin.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _isFaculty;
        public bool IsFaculty
        {
            get => _isFaculty;
            set => SetProperty(ref _isFaculty, value);
        }

        private bool _isMonthlyRegular;
        public bool IsMonthlyRegular
        {
            get => _isMonthlyRegular;
            set => SetProperty(ref _isMonthlyRegular, value);
        }

        private bool _isVisitingFixed;
        public bool IsVisitingFixed
        {
            get => _isVisitingFixed;
            set => SetProperty(ref _isVisitingFixed, value);
        }

        private bool _isVisitingPerDay;
        public bool IsVisitingPerDay
        {
            get => _isVisitingPerDay;
            set => SetProperty(ref _isVisitingPerDay, value);
        }

        private bool _dialogResult;
        public bool DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        #endregion

        #region Commands

        public ICommand StartCaptureCommand { get; }
        public ICommand StopCaptureCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Methods

        private void InitializeData()
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            IsFaculty = User?.UserType == UserType.Faculty;
            
            if (IsFaculty && User?.FacultyType.HasValue == true)
            {
                IsMonthlyRegular = User.FacultyType == Models.FacultyType.MonthlyRegular;
                IsVisitingFixed = User.FacultyType == Models.FacultyType.VisitingFixed;
                IsVisitingPerDay = User.FacultyType == Models.FacultyType.VisitingPerDay;
            }
            else
            {
                IsMonthlyRegular = false;
                IsVisitingFixed = false;
                IsVisitingPerDay = false;
            }
        }

        private bool CanStartCapture()
        {
            return !IsCapturing && ValidateUserDetails();
        }

        private bool ValidateUserDetails()
        {
            if (string.IsNullOrWhiteSpace(User?.Name) ||
                string.IsNullOrWhiteSpace(User?.Phone) ||
                string.IsNullOrWhiteSpace(User?.Department))
            {
                return false;
            }

            if (User.UserType == UserType.Faculty && !User.FacultyType.HasValue)
            {
                return false;
            }

            return true;
        }

        private void StartCapture()
        {
            try
            {
                if (!_faceDetectionService.IsModelLoaded)
                {
                    MessageBox.Show("Face detection model is not loaded. Please restart the application.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Start camera
                if (!_cameraService.Start())
                {
                    MessageBox.Show("Failed to start camera. Please check if camera is connected and not in use by another application.",
                        "Camera Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                IsCapturing = true;
                CapturedCount = 0;
                _capturedFaces.Clear();
                StatusMessage = $"Capturing images... {CapturedCount}/{REQUIRED_IMAGES}. Look at the camera and move your head slightly.";

                // Start frame timer
                _frameTimer = new System.Windows.Threading.DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };
                _frameTimer.Tick += OnFrameTimerTick;
                _frameTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting capture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCapture();
            }
        }

        private void OnFrameTimerTick(object? sender, EventArgs e)
        {
            try
            {
                var frame = _cameraService.GetFrame();
                if (frame == null || frame.Empty())
                    return;

                var displayFrame = frame.Clone();

                // Detect faces
                var faces = _faceDetectionService.DetectFaces(frame);

                if (faces.Count == 1)
                {
                    var face = faces[0];
                    
                    // Draw rectangle on display frame
                    Cv2.Rectangle(displayFrame, face.BoundingBox, Scalar.Green, 3);
                    
                    // Capture face image periodically
                    if (_capturedFaces.Count < REQUIRED_IMAGES && _capturedFaces.Count % 1 == 0)
                    {
                        // Extract face region
                        var faceImage = new Mat(frame, face.BoundingBox);
                        _capturedFaces.Add(faceImage.Clone());
                        CapturedCount = _capturedFaces.Count;
                        
                        StatusMessage = $"Capturing... {CapturedCount}/{REQUIRED_IMAGES}. Keep looking at the camera.";
                        
                        if (CapturedCount >= REQUIRED_IMAGES)
                        {
                            StatusMessage = $"✓ Capture complete! {REQUIRED_IMAGES} images captured. Click 'Save' to register.";
                            StopCapture();
                        }
                    }
                }
                else if (faces.Count > 1)
                {
                    StatusMessage = "⚠ Multiple faces detected. Please ensure only one person is in frame.";
                }
                else
                {
                    StatusMessage = "⚠ No face detected. Please look at the camera.";
                }

                // Update display - Convert Mat to BitmapSource
                using (var bitmap = BitmapConverter.ToBitmap(displayFrame))
                {
                    var bitmapImage = new BitmapImage();
                    using (var memory = new System.IO.MemoryStream())
                    {
                        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze();
                    }
                    CameraFrame = bitmapImage;
                }
                displayFrame.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in frame processing: {ex.Message}");
            }
        }

        private void StopCapture()
        {
            try
            {
                _frameTimer?.Stop();
                _frameTimer = null;
                _cameraService.Stop();
                IsCapturing = false;

                if (CapturedCount >= REQUIRED_IMAGES)
                {
                    StatusMessage = $"✓ Ready to save! {REQUIRED_IMAGES} images captured.";
                }
                else if (CapturedCount > 0)
                {
                    StatusMessage = $"Capture stopped. {CapturedCount}/{REQUIRED_IMAGES} images captured. Need {REQUIRED_IMAGES - CapturedCount} more.";
                }
                else
                {
                    StatusMessage = "Capture stopped. No images captured.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping capture: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            return !IsSaving && CapturedCount >= REQUIRED_IMAGES && ValidateUserDetails();
        }

        private async Task SaveAsync()
        {
            if (!ValidateUserDetails())
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_capturedFaces.Count < REQUIRED_IMAGES)
            {
                MessageBox.Show($"Need {REQUIRED_IMAGES} face images. Only {_capturedFaces.Count} captured.", 
                    "Insufficient Images", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsSaving = true;
            StatusMessage = "Saving user...";

            try
            {
                // Generate face encoding from captured images
                // Using the first good quality image for encoding
                var bestImage = _capturedFaces[_capturedFaces.Count / 2]; // Middle image
                var encoding = _faceRecognitionService.GenerateEncoding(bestImage);

                if (encoding == null || encoding.Length == 0)
                {
                    MessageBox.Show("Failed to generate face encoding. Please try again.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                User.FaceEncoding = encoding;

                // Save profile photo
                var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var faceImagesPath = Path.Combine(appDataPath, "FaceRecognitionAttendance", "FaceImages");
                Directory.CreateDirectory(faceImagesPath);

                var photoFileName = $"user_{DateTime.Now:yyyyMMddHHmmss}_{User.Phone}.jpg";
                var photoPath = Path.Combine(faceImagesPath, photoFileName);
                
                Cv2.ImWrite(photoPath, bestImage);
                User.PhotoPath = photoPath;

                // Save to database
                var savedUser = await _userRepository.AddAsync(User);

                if (savedUser != null)
                {
                    MessageBox.Show($"User '{User.Name}' registered successfully!", 
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    DialogResult = true;
                    
                    // Cleanup
                    CleanupCapturedImages();
                }
                else
                {
                    MessageBox.Show("Failed to save user to database.", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel? All captured images will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                StopCapture();
                CleanupCapturedImages();
                DialogResult = false;
            }
        }

        private void CleanupCapturedImages()
        {
            foreach (var face in _capturedFaces)
            {
                face?.Dispose();
            }
            _capturedFaces.Clear();
        }

        public void Cleanup()
        {
            StopCapture();
            CleanupCapturedImages();
        }

        #endregion
    }
}
