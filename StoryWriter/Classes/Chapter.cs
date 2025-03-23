using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StoryWriter.Classes
{
    public class Chapter
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("Name")]
        public required  string Name { get; set; }

        [BsonElement("Number")]
        public int Number { get; set; }
        
        [BsonElement("Content")]
        public int Content { get; set; }
    }
}
