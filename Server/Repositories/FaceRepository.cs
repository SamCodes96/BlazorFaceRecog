﻿using BlazorFaceRecog.Server.Dtos;
using MongoDB.Driver;

namespace BlazorFaceRecog.Server.Repositories;

public class FaceRepository(IMongoDatabase database, IConfiguration configuration)
    : RepositoryBase<EmbeddedFaceDto>(database, "Faces")
{
    public void Add(Guid id, string name, byte[] image, float[] embedding)
    {
        Collection.InsertOne(new EmbeddedFaceDto(id, name, image, embedding));
    }

    public void Update(Guid id, string name)
    {
        var filter = Builders<EmbeddedFaceDto>.Filter.Eq(x => x.Id, id);
        var update = Builders<EmbeddedFaceDto>.Update.Set(x => x.Name, name);

        Collection.UpdateOne(filter, update);
    }

    public void Delete(Guid id)
    {
        var filter = Builders<EmbeddedFaceDto>.Filter.Eq(x => x.Id, id);

        Collection.DeleteOne(filter);
    }

    public string GetNearestFace(float[] embedding)
    {
        var searchOptions = new VectorSearchOptions<EmbeddedFaceDto>()
        {
            IndexName = configuration.GetSection("MongoDB")["SearchIndexName"],
            NumberOfCandidates = (int)GetCount()
        };

        var result = Collection.Aggregate()
            .VectorSearch(f => f.Embedding, embedding, 1, searchOptions)
            .Project(Builders<EmbeddedFaceDto>.Projection
                .Include(f => f.Name))
            .FirstOrDefault();

        return result?[nameof(EmbeddedFaceDto.Name)]?.AsString ?? "Unknown Face";
    }
}