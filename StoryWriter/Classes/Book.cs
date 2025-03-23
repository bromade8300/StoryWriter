using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryWriter.Classes
{
   public class Book
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Title")]
        public required string Title { get; set; }

        [BsonElement("Author")]
        public required string Author { get; set; }

        [BsonElement("Chapters")]
        public List<Chapter>? Chapters { get; set; }
    }
}
