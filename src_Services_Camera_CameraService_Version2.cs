using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services.Camera
{
    /// <summary>
    /// Camera service implementation using OpenCV
    /// </summary>
    public class CameraService : ICameraService, IDisposable
    {
        private VideoCapture?  _capture;
        private Mat? _frame;
        private CancellationTokenSource?  _cancellationTokenSource;
        private Task?  _captureTask;
        private bool _isRunning;

        public bool IsRunning => _isRunning;

        public event EventHandler<Mat>? FrameCaptured;

        public bool Start(int cameraIndex = 0)
        {
            try
            {
                if (_isRunning)
                {
                    Stop();
                }

                _capture = new VideoCapture(cameraIndex);
                
                if (! _capture.IsOpened())
                {
                    Console.WriteLine($"Failed to open camera {cameraIndex}");
                    return false;
                }

                // Set camera properties
                _capture. Set(VideoCaptureProperties.FrameWidth, 640);
                _capture.Set(VideoCaptureProperties.FrameHeight, 480);
                _capture.Set(VideoCaptureProperties. Fps, 30);

                _frame = new Mat();
                _isRunning = true;
                _cancellationTokenSource = new CancellationTokenSource();

                // Start capture loop
                _captureTask = Task.Run(() => CaptureLoop(_cancellationTokenSource.Token));

                Console.WriteLine($"Camera {cameraIndex} started successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting camera: {ex.Message}");
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                _captureTask?. Wait(1000);

                _capture?.Release();
                _capture?.Dispose();
                _capture = null;

                _frame?.Dispose();
                _frame = null;

                Console. WriteLine("Camera stopped");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping camera: {ex. Message}");
            }
        }

        public Mat? GetFrame()
        {
            return _frame?. Clone();
        }

        private void CaptureLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _capture != null && _frame != null)
            {
                try
                {
                    _capture. Read(_frame);

                    if (! _frame.Empty())
                    {
                        FrameCaptured?.Invoke(this, _frame. Clone());
                    }

                    // Small delay to control frame rate
                    Thread.Sleep(33); // ~30 FPS
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in capture loop: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}