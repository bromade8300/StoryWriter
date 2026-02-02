using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StoryWriter.Classes
{
    public class Character
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public required string Name { get; set; }

        [BsonElement("Role")]
        public string? Role { get; set; }  // Protagoniste, Antagoniste, Secondaire, Figurant

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("Traits")]
        public List<string>? Traits { get; set; }

        [BsonElement("Notes")]
        public string? Notes { get; set; }

        [BsonElement("ImageUrl")]
        public string? ImageUrl { get; set; }
    }
}
