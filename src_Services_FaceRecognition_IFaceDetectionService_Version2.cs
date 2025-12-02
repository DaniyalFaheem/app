using OpenCvSharp;
using System.Collections.Generic;

namespace FaceRecognitionAttendance.Services.FaceRecognition
{
    /// <summary>
    /// Face detection result
    /// </summary>
    public class FaceDetectionResult
    {
        public Rect BoundingBox { get; set; }
        public float Confidence { get; set; }
        public Mat FaceImage { get; set; } = new Mat();
    }

    /// <summary>
    /// Face detection service interface
    /// </summary>
    public interface IFaceDetectionService
    {
        /// <summary>
        /// Detect faces in frame
        /// </summary>
        List<FaceDetectionResult> DetectFaces(Mat frame);

        /// <summary>
        /// Check if detection model is loaded
        /// </summary>
        bool IsModelLoaded { get; }
    }
}