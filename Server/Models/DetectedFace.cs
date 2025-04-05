namespace BlazorFaceRecog.Server.Models;

public record class DetectedFace
{
    public required string Name { get; init; }
    public required float Score
    {
        get;
        init
        {
            field = value < 1 ? value * 100 : value;
        }
    }
}