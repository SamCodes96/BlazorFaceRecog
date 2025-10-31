namespace BlazorFaceRecog.Server.Configuration;

public class AppSettings
{
    public required int Threshold { get; init; }

    public required bool UseGpu { get; init; }
}
