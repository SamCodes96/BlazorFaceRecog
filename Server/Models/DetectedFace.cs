namespace BlazorFaceRecog.Server.Models;

public record class DetectedFace
{
    public DetectedFace(string name, float score)
    {
        Name = name;
        Score = score;
    }

    private float _percentageScore;

    public string Name { get; set; }
    public float Score
    {
        get => _percentageScore;
        set => _percentageScore = value < 1
            ? value * 100
            : value;
    }
}