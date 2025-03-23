using MongoDB.Bson;
using MongoDB.Driver;
using StoryWriter.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryWriter.BDD
{
    public class DataBaseService
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;

        public static void Start() {
            try
            {
                _client = new MongoClient(BddSettings.ConnectionString);
                _database = _client.GetDatabase(BddSettings.DatabaseName);
            }
            catch (Exception e)
            {

            }
        }

        public static IMongoCollection<T> GetMongoCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
