using OpenCvSharp;
using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.IO;

namespace FaceRecognitionAttendance.Services.FaceRecognition
{
    /// <summary>
    /// Face detection service using OpenCV DNN
    /// </summary>
    public class FaceDetectionService : IFaceDetectionService
    {
        private Net?  _net;
        private readonly float _confidenceThreshold = 0.7f;

        public bool IsModelLoaded => _net != null;

        public FaceDetectionService()
        {
            LoadModel();
        }

        private void LoadModel()
        {
            try
            {
                // TODO: Load face detection model
                // Download from: https://github.com/opencv/opencv/tree/master/samples/dnn/face_detector
                // Files needed: deploy.prototxt and res10_300x300_ssd_iter_140000. caffemodel
                
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                var modelPath = Path.Combine(appPath, "Resources", "Models");
                var prototxtPath = Path.Combine(modelPath, "deploy.prototxt");
                var caffeModelPath = Path.Combine(modelPath, "res10_300x300_ssd_iter_140000.caffemodel");

                if (File.Exists(prototxtPath) && File.Exists(caffeModelPath))
                {
                    _net = CvDnn.ReadNetFromCaffe(prototxtPath, caffeModelPath);
                    Console.WriteLine("Face detection model loaded successfully");
                }
                else
                {
                    Console. WriteLine("WARNING: Face detection model files not found!");
                    Console.WriteLine($"Expected location: {modelPath}");
                    Console.WriteLine("Please download model files and place them in the Resources/Models folder");
                    
                    // Use Haar Cascade as fallback
                    Console.WriteLine("Using Haar Cascade as fallback...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading face detection model: {ex.Message}");
            }
        }

        public List<FaceDetectionResult> DetectFaces(Mat frame)
        {
            var results = new List<FaceDetectionResult>();

            try
            {
                if (_net != null)
                {
                    return DetectWithDnn(frame);
                }
                else
                {
                    return DetectWithHaarCascade(frame);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error detecting faces: {ex.Message}");
            }

            return results;
        }

        private List<FaceDetectionResult> DetectWithDnn(Mat frame)
        {
            var results = new List<FaceDetectionResult>();

            if (_net == null) return results;

            try
            {
                // Prepare input blob
                var blob = CvDnn.BlobFromImage(frame, 1.0, new Size(300, 300), 
                    new Scalar(104, 177, 123), false, false);

                _net.SetInput(blob);
                var detection = _net.Forward();

                // Process detections
                var rows = detection.Size(2);
                for (int i = 0; i < rows; i++)
                {
                    var confidence = detection. At<float>(0, 0, i, 2);

                    if (confidence > _confidenceThreshold)
                    {
                        int x1 = (int)(detection.At<float>(0, 0, i, 3) * frame.Width);
                        int y1 = (int)(detection.At<float>(0, 0, i, 4) * frame.Height);
                        int x2 = (int)(detection. At<float>(0, 0, i, 5) * frame.Width);
                        int y2 = (int)(detection. At<float>(0, 0, i, 6) * frame.Height);

                        var rect = new Rect(x1, y1, x2 - x1, y2 - y1);

                        // Ensure rect is within frame bounds
                        rect.X = Math.Max(0, rect.X);
                        rect. Y = Math.Max(0, rect.Y);
                        rect.Width = Math.Min(rect.Width, frame.Width - rect.X);
                        rect. Height = Math.Min(rect. Height, frame.Height - rect. Y);

                        if (rect.Width > 0 && rect.Height > 0)
                        {
                            var faceImage = new Mat(frame, rect);

                            results.Add(new FaceDetectionResult
                            {
                                BoundingBox = rect,
                                Confidence = confidence,
                                FaceImage = faceImage. Clone()
                            });
                        }
                    }
                }

                blob.Dispose();
                detection.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DNN face detection: {ex.Message}");
            }

            return results;
        }

        private List<FaceDetectionResult> DetectWithHaarCascade(Mat frame)
        {
            var results = new List<FaceDetectionResult>();

            try
            {
                var cascade = new CascadeClassifier("haarcascade_frontalface_default.xml");
                var gray = new Mat();
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

                var faces = cascade.DetectMultiScale(gray, 1.1, 4, HaarDetectionTypes.ScaleImage, new Size(100, 100));

                foreach (var rect in faces)
                {
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        var faceImage = new Mat(frame, rect);

                        results.Add(new FaceDetectionResult
                        {
                            BoundingBox = rect,
                            Confidence = 0.9f, // Haar doesn't provide confidence
                            FaceImage = faceImage.Clone()
                        });
                    }
                }

                gray.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Haar Cascade detection: {ex.Message}");
            }

            return results;
        }
    }
}