using Rect = System.Drawing.Rectangle;

namespace BlazorFaceRecog.Shared;

public record class AnalyzedImage(string Name, float Score, Rect Face);