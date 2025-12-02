namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// User type enumeration for Students and Faculty
    /// </summary>
    public enum UserType
    {
        Student = 0,
        Faculty = 1
    }

    /// <summary>
    /// Faculty salary calculation types
    /// </summary>
    public enum FacultyType
    {
        /// <summary>
        /// Type 1: Regular monthly salary with deductions for absences
        /// </summary>
        MonthlyRegular = 0,
        
        /// <summary>
        /// Type 2: Visiting faculty with fixed 30-day contract (no deductions)
        /// </summary>
        VisitingFixed = 1,
        
        /// <summary>
        /// Type 3: Visiting faculty with per-day rate (rate × present days)
        /// </summary>
        VisitingPerDay = 2
    }

    /// <summary>
    /// User roles for authentication and authorization
    /// </summary>
    public enum UserRole
    {
        User = 0,
        Admin = 1
    }

    /// <summary>
    /// Face detection quality status
    /// </summary>
    public enum FaceQualityStatus
    {
        Good = 0,
        TooBlurry = 1,
        TooSmall = 2,
        MultipleFaces = 3,
        NoFaceDetected = 4,
        LowConfidence = 5
    }

    /// <summary>
    /// Pose directions for face capture guidance
    /// </summary>
    public enum PoseDirection
    {
        Straight = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }
}