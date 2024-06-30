namespace BlazorFaceRecog.Server.Dtos;

public record class EmbeddedFaceDto(Guid Id, string Name, float[] Embedding);