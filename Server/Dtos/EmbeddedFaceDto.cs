namespace BlazorFaceRecog.Server.Dtos;

public record class EmbeddedFaceDto(string Name, float[] Embedding);