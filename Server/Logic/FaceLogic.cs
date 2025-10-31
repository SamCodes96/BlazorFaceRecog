using System.Drawing;
using FaceONNX;

namespace BlazorFaceRecog.Server.Logic;

public interface IFaceLogic
{
    FaceDetectionResult[] DetectFaces(Bitmap image);
    float[] GetEmbeddingForFace(Bitmap image, Rectangle faceArea);
}

public class FaceLogic(
    IFace68LandmarksExtractor extractor,
    IFaceDetector detector,
    IFaceClassifier classifier) : IFaceLogic
{
    public FaceDetectionResult[] DetectFaces(Bitmap image) => detector.Forward(image);

    public float[] GetEmbeddingForFace(Bitmap image, Rectangle faceArea)
    {
        if (faceArea.IsEmpty)
            return new float[512];

        var points = extractor.Forward(image, faceArea);

        using var aligned = image.Align(faceArea, points.RotationAngle);
        return classifier.Forward(aligned);
    }
}