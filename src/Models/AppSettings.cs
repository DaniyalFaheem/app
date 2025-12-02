namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// Application settings configuration
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Camera device index (default 0)
        /// </summary>
        public int CameraIndex { get; set; } = 0;
        
        /// <summary>
        /// Face recognition distance threshold (lower = stricter)
        /// </summary>
        public double RecognitionThreshold { get; set; } = 0.4;
        
        /// <summary>
        /// Attendance cooldown in minutes
        /// </summary>
        public int AttendanceCooldownMinutes { get; set; } = 5;
        
        /// <summary>
        /// Session timeout in minutes
        /// </summary>
        public int SessionTimeoutMinutes { get; set; } = 30;
        
        /// <summary>
        /// Number of images to capture during registration
        /// </summary>
        public int RegistrationImageCount { get; set; } = 80;
        
        /// <summary>
        /// Number of consecutive frames required for stable recognition
        /// </summary>
        public int StabilityFrameCount { get; set; } = 4;
        
        /// <summary>
        /// Face detection confidence threshold
        /// </summary>
        public double DetectionConfidenceThreshold { get; set; } = 0.7;
        
        /// <summary>
        /// Minimum face size in pixels
        /// </summary>
        public int MinimumFaceSize { get; set; } = 100;
        
        /// <summary>
        /// Blur detection threshold (Laplacian variance)
        /// </summary>
        public double BlurThreshold { get; set; } = 100.0;
        
        /// <summary>
        /// Database connection string
        /// </summary>
        public string DatabasePath { get; set; } = string.Empty;
        
        /// <summary>
        /// Face images storage path
        /// </summary>
        public string FaceImagesPath { get; set; } = string.Empty;
        
        /// <summary>
        /// CSV export path
        /// </summary>
        public string ExportPath { get; set; } = string.Empty;
    }
}