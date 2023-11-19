using System.Drawing;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Server.Services
{
    public class FaceService(
        FaceRepository faceRepository,
        FaceLogic faceLogic)
    {
        public bool FaceIsTrained => faceRepository.Count > 0;

        public Rectangle[] DetectInImage(ImageData data)
        {
            using var bmImage = GetBitmapImage(data.Bytes);

            var result = faceLogic.Detect(bmImage);

            return result.Select(x => x.Box).ToArray();
        }

        public string RecogniseInImage(ImageData data, Rectangle face)
        {
            using var bmImage = GetBitmapImage(data.Bytes);

            var vector = faceLogic.GetEmbedded(bmImage, face);
            var (label, min) = faceRepository.FromSimilarity(vector);

            return label + $" - {min}";
        }

        public void TrainFromImage(TrainFaceData data, Rectangle face)
        {
            using var bmImage = GetBitmapImage(data.Data);

            var vector = faceLogic.GetEmbedded(bmImage, face);
            faceRepository.Add(data.Name, vector);
        }

        private static Bitmap GetBitmapImage(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
    }
}
