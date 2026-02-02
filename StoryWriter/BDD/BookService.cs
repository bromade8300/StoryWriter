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

        // Chapter methods
        public static void AddChapter(string bookId, Chapter chapter)
        {
            var book = GetBook(bookId);
            if (book != null)
            {
                book.Chapters ??= new List<Chapter>();
                book.Chapters.Add(chapter);
                UpdateBook(book);
            }
        }

        public static void UpdateChapter(string bookId, Chapter chapter)
        {
            var book = GetBook(bookId);
            if (book?.Chapters != null)
            {
                var index = book.Chapters.FindIndex(c => c.Id == chapter.Id);
                if (index >= 0)
                {
                    book.Chapters[index] = chapter;
                    UpdateBook(book);
                }
            }
        }

        public static void DeleteChapter(string bookId, string chapterId)
        {
            var book = GetBook(bookId);
            if (book?.Chapters != null)
            {
                book.Chapters.RemoveAll(c => c.Id == chapterId);
                UpdateBook(book);
            }
        }

        public static Chapter? GetChapter(string bookId, string chapterId)
        {
            var book = GetBook(bookId);
            return book?.Chapters?.FirstOrDefault(c => c.Id == chapterId);
        }

        // Character methods
        public static void AddCharacter(string bookId, Character character)
        {
            var book = GetBook(bookId);
            if (book != null)
            {
                book.Characters ??= new List<Character>();
                book.Characters.Add(character);
                UpdateBook(book);
            }
        }

        public static void UpdateCharacter(string bookId, Character character)
        {
            var book = GetBook(bookId);
            if (book?.Characters != null)
            {
                var index = book.Characters.FindIndex(c => c.Id == character.Id);
                if (index >= 0)
                {
                    book.Characters[index] = character;
                    UpdateBook(book);
                }
            }
        }

        public static void DeleteCharacter(string bookId, string characterId)
        {
            var book = GetBook(bookId);
            if (book?.Characters != null)
            {
                book.Characters.RemoveAll(c => c.Id == characterId);
                UpdateBook(book);
            }
        }

        public static List<Character>? GetCharacters(string bookId)
        {
            var book = GetBook(bookId);
            return book?.Characters;
        }

    }
}
