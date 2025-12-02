using OpenCvSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services.FaceRecognition
{
    /// <summary>
    /// Face recognition result
    /// </summary>
    public class FaceRecognitionResult
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public double Distance { get; set; }
        public bool IsMatch { get; set; }
    }

    /// <summary>
    /// Face recognition service interface
    /// </summary>
    public interface IFaceRecognitionService
    {
        /// <summary>
        /// Generate face encoding from face image
        /// </summary>
        byte[] GenerateEncoding(Mat faceImage);

        /// <summary>
        /// Compare face image with stored encodings
        /// </summary>
        Task<FaceRecognitionResult? > RecognizeFaceAsync(Mat faceImage);

        /// <summary>
        /// Load all face encodings from database
        /// </summary>
        Task LoadEncodingsAsync();

        /// <summary>
        /// Calculate distance between two encodings
        /// </summary>
        double CalculateDistance(byte[] encoding1, byte[] encoding2);
    }
}