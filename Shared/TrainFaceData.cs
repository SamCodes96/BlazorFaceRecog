using SixLabors.ImageSharp;

namespace BlazorFaceRecog.Shared
{
    public record class TrainFaceData(string Name, byte[] Data)
    {
        public ImageData GetImageData()
        {
            var image = Image.Load(Data);

            return new(Data, image.Width, image.Height);
        }
    };
}
