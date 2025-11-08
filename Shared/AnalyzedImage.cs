using System.Drawing;

namespace BlazorFaceRecog.Shared;

public record class AnalyzedImage(string Name, float Score, Rectangle Face);
