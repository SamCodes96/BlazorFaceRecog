using System.Drawing;
using FaceONNX;

namespace BlazorFaceRecog.Server.Logic
{
    public class FaceLogic(
        IFace68LandmarksExtractor extractor,
        IFaceDetector detector,
        IFaceClassifier classifier)
    {
        public FaceDetectionResult[] Detect(Bitmap bitmap)
        {
            return detector.Forward(bitmap);
        }

        public float[] GetEmbedded(Bitmap image, Rectangle face)
        {
            if (!face.IsEmpty)
            {
                // landmarks
                var points = extractor.Forward(image, face);

                // alignment
                using var aligned = image.Align(face, points.RotationAngle);
                return classifier.Forward(aligned);
            }

            return new float[512];
        }
    }
}
