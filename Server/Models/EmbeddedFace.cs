namespace BlazorFaceRecog.Server.Models;

public class EmbeddedFace(Guid Id, string Name, byte[] Image, float[] Embedding)
{
    public Guid Id { get; set; } = Id;
    public string Name { get; set; } = Name;
    public byte[] Image { get; set; } = Image;
    public float[] Embedding { get; set; } = Embedding;
}