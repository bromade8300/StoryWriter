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
   public class BookService
    {

        private static IMongoCollection<Book>?  _BookCollection;

        public static void Start() 
        {
            try
            {
                _BookCollection = DataBaseService.GetMongoCollection<Book>("Books");
            } catch (Exception e)
            {
                
            }
        }
        public static void InsertBook(Book Book)
        {
            try
            {
                BookService.Start();
                _BookCollection.InsertOne(Book);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void UpdateBook(Book Book)
        {
            _BookCollection.ReplaceOne(c => c.Id == Book.Id, Book);
        }

        public static void DeleteBook(string id)
        {
            _BookCollection.DeleteOne(c => c.Id == id);
        }

        public static List<Book> GetBooks()
        {
            return _BookCollection.Find(c => true).ToList();
        }

        public static Book GetBook(string id)
        {
            return _BookCollection.Find(c => c.Id == id).FirstOrDefault();
        }

    }
}
