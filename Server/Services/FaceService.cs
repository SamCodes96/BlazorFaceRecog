﻿using System.Drawing;
using System.Drawing.Imaging;
using BlazorFaceRecog.Server.Logic;
using BlazorFaceRecog.Server.Repositories;
using BlazorFaceRecog.Shared;

namespace BlazorFaceRecog.Server.Services;

public class FaceService(
    FaceRepository faceRepository,
    FaceLogic faceLogic)
{
    public bool FaceIsTrained => faceRepository.Count > 0;

    public Rectangle[] DetectInImage(byte[] imageData)
    {
        using var bmImage = GetBitmapImage(imageData);

        var result = faceLogic.Detect(bmImage);

        return result.Select(x => x.Box).ToArray();
    }

    public string RecogniseInImage(byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        return faceRepository.GetNearestFace(embedding);
    }

    public void TrainFromImage(TrainFaceModel data, byte[] imageData, Rectangle? faceArea = null)
    {
        using var bmImage = GetBitmapImage(imageData);
        faceArea ??= new Rectangle(0, 0, bmImage.Width, bmImage.Height);

        var embedding = faceLogic.GetEmbedding(bmImage, faceArea.Value);
        faceRepository.Add(data.Id, data.Name, embedding);
    }

    public byte[] CropFaceInImage(byte[] imageData, Rectangle faceArea)
    {
        using var bmImage = GetBitmapImage(imageData);

        using var croppedImage = new Bitmap(bmImage.Width, bmImage.Height);
        using Graphics g = Graphics.FromImage(croppedImage);
        g.DrawImage(
            bmImage,
            new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
            faceArea,
            GraphicsUnit.Pixel);

        using var ms = new MemoryStream();
        croppedImage.Save(ms, ImageFormat.Jpeg);

        return ms.ToArray();
    }

    private static Bitmap GetBitmapImage(byte[] imageData)
    {
        using var ms = new MemoryStream(imageData);
        return new Bitmap(ms);
    }
}
