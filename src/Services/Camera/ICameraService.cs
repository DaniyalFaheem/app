using OpenCvSharp;
using System;

namespace FaceRecognitionAttendance.Services.Camera
{
    /// <summary>
    /// Camera service interface
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// Start camera capture
        /// </summary>
        bool Start(int cameraIndex = 0);

        /// <summary>
        /// Stop camera capture
        /// </summary>
        void Stop();

        /// <summary>
        /// Get current frame
        /// </summary>
        Mat?  GetFrame();

        /// <summary>
        /// Check if camera is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Frame captured event
        /// </summary>
        event EventHandler<Mat>? FrameCaptured;
    }
}