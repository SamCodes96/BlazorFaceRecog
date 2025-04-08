using System.Drawing;
using FaceONNX;

namespace BlazorFaceRecog.Server.Logic;

public class FaceLogic(
    IFace68LandmarksExtractor extractor,
    IFaceDetector detector,
    IFaceClassifier classifier)
{
    public FaceDetectionResult[] Detect(Bitmap image) => detector.Forward(image);

    public float[] GetEmbedding(Bitmap image, Rectangle faceArea)
    {
        if (faceArea.IsEmpty)
            return new float[512];

        var points = extractor.Forward(image, faceArea);

        using var aligned = image.Align(faceArea, points.RotationAngle);
        return classifier.Forward(aligned);
    }
}