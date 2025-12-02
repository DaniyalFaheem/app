using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognitionAttendance.Data.Repositories;

namespace FaceRecognitionAttendance.Services.FaceRecognition
{
    /// <summary>
    /// Face recognition service implementation
    /// </summary>
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly IUserRepository _userRepository;
        private Dictionary<int, byte[]> _faceEncodings = new Dictionary<int, byte[]>();
        private readonly double _recognitionThreshold = 0.4;

        public FaceRecognitionService(IUserRepository userRepository)
        {
            _userRepository = userRepository ??  throw new ArgumentNullException(nameof(userRepository));
        }

        public byte[] GenerateEncoding(Mat faceImage)
        {
            try
            {
                // TODO: Implement actual face encoding using a face recognition model
                // For now, we'll use a simple feature extraction as placeholder
                
                // Resize face to standard size
                var resized = new Mat();
                Cv2.Resize(faceImage, resized, new Size(128, 128));

                // Convert to grayscale
                var gray = new Mat();
                Cv2.CvtColor(resized, gray, ColorConversionCodes.BGR2GRAY);

                // Flatten to 1D array (simple encoding)
                var encoding = new byte[128 * 128];
                var indexer = gray.GetGenericIndexer<byte>();
                
                int idx = 0;
                for (int y = 0; y < gray.Height; y++)
                {
                    for (int x = 0; x < gray.Width; x++)
                    {
                        encoding[idx++] = indexer[y, x];
                    }
                }

                resized.Dispose();
                gray.Dispose();

                return encoding;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating face encoding: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public async Task<FaceRecognitionResult? > RecognizeFaceAsync(Mat faceImage)
        {
            try
            {
                if (_faceEncodings.Count == 0)
                {
                    await LoadEncodingsAsync();
                }

                var encoding = GenerateEncoding(faceImage);
                if (encoding.Length == 0)
                    return null;

                double minDistance = double.MaxValue;
                int bestMatchId = -1;

                // Compare with all stored encodings
                foreach (var kvp in _faceEncodings)
                {
                    var distance = CalculateDistance(encoding, kvp.Value);
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        bestMatchId = kvp.Key;
                    }
                }

                // Check if match is within threshold
                if (minDistance < _recognitionThreshold && bestMatchId != -1)
                {
                    var user = await _userRepository.GetByIdAsync(bestMatchId);
                    
                    if (user != null)
                    {
                        return new FaceRecognitionResult
                        {
                            UserId = user.Id,
                            UserName = user.Name,
                            Distance = minDistance,
                            IsMatch = true
                        };
                    }
                }

                return new FaceRecognitionResult
                {
                    UserId = -1,
                    UserName = "Unknown",
                    Distance = minDistance,
                    IsMatch = false
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recognizing face: {ex.Message}");
                return null;
            }
        }

        public async Task LoadEncodingsAsync()
        {
            try
            {
                _faceEncodings = await _userRepository.GetAllFaceEncodingsAsync();
                Console.WriteLine($"Loaded {_faceEncodings.Count} face encodings");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading face encodings: {ex.Message}");
            }
        }

        public double CalculateDistance(byte[] encoding1, byte[] encoding2)
        {
            if (encoding1. Length != encoding2.Length)
                return double.MaxValue;

            // Calculate Euclidean distance
            double sum = 0;
            for (int i = 0; i < encoding1.Length; i++)
            {
                double diff = encoding1[i] - encoding2[i];
                sum += diff * diff;
            }

            return Math.Sqrt(sum) / encoding1.Length; // Normalize by length
        }
    }
}