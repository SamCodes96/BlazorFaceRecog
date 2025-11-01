using System.ComponentModel.DataAnnotations;

namespace BlazorFaceRecog.Server.Configuration;

public class AppSettings
{
    [Range(0, 100, MinimumIsExclusive = true, MaximumIsExclusive = true)]
    public required int Threshold { get; init; }

    public required bool UseGpu { get; init; }
}
