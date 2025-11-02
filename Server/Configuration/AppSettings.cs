using System.ComponentModel.DataAnnotations;

namespace BlazorFaceRecog.Server.Configuration;

public class AppSettings
{
    [Range(0, 100, MaximumIsExclusive = true)]
    public int Threshold { get; init; }

    public bool UseGpu { get; init; }
}
