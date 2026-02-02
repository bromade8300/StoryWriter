using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using StoryWriter.BDD;
using StoryWriter.Classes;

namespace WebApi.Controllers;

[ApiController]
[Route("/Book")]
public class BookController : ControllerBase
{

    private readonly ILogger<BookController> _logger;

    public BookController(ILogger<BookController> logger)
    {
        _logger = logger;
    }

    // Insert a new book
    [HttpPost]
    public IActionResult InsertBook([FromBody]Book book)
    {
        try
        {
            BookService.InsertBook(book);
            return Ok("Book inserted successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Update an existing book
    [HttpPut("{id}")]
    public IActionResult UpdateBook(string id, [FromBody] Book book)
    {
        if (id != book.Id)
        {
            return BadRequest("Book ID mismatch.");
        }

        try
        {
            BookService.UpdateBook(book);
            return Ok("Book updated successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Delete a book by ID
    [HttpDelete("{id}")]
    public IActionResult DeleteBook(string id)
    {
        try
        {
            BookService.DeleteBook(id);
            return Ok("Book deleted successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Get all books
    [HttpGet]
    public ActionResult<List<Book>> GetBooks()
    {
        return BookService.GetBooks();
    }

    // Get a book by ID
    [HttpGet("{id}")]
    public ActionResult<Book> GetBook(string id)
    {
        var book = BookService.GetBook(id);
        if (book == null)
        {
            return NotFound("Book not found.");
        }
        return book;
    }

    // ========== CHAPTER ENDPOINTS ==========

    // Add a chapter to a book
    [HttpPost("{bookId}/chapters")]
    public IActionResult AddChapter(string bookId, [FromBody] Chapter chapter)
    {
        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.AddChapter(bookId, chapter);
            return Ok("Chapter added successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Update a chapter in a book
    [HttpPut("{bookId}/chapters/{chapterId}")]
    public IActionResult UpdateChapter(string bookId, string chapterId, [FromBody] Chapter chapter)
    {
        if (chapterId != chapter.Id)
        {
            return BadRequest("Chapter ID mismatch.");
        }

        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.UpdateChapter(bookId, chapter);
            return Ok("Chapter updated successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Delete a chapter from a book
    [HttpDelete("{bookId}/chapters/{chapterId}")]
    public IActionResult DeleteChapter(string bookId, string chapterId)
    {
        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.DeleteChapter(bookId, chapterId);
            return Ok("Chapter deleted successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Get a specific chapter from a book
    [HttpGet("{bookId}/chapters/{chapterId}")]
    public ActionResult<Chapter> GetChapter(string bookId, string chapterId)
    {
        var book = BookService.GetBook(bookId);
        if (book == null)
        {
            return NotFound("Book not found.");
        }

        var chapter = BookService.GetChapter(bookId, chapterId);
        if (chapter == null)
        {
            return NotFound("Chapter not found.");
        }
        return chapter;
    }

    // ========== CHARACTER ENDPOINTS ==========

    // Add a character to a book
    [HttpPost("{bookId}/characters")]
    public IActionResult AddCharacter(string bookId, [FromBody] Character character)
    {
        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.AddCharacter(bookId, character);
            return Ok("Character added successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Update a character in a book
    [HttpPut("{bookId}/characters/{characterId}")]
    public IActionResult UpdateCharacter(string bookId, string characterId, [FromBody] Character character)
    {
        if (characterId != character.Id)
        {
            return BadRequest("Character ID mismatch.");
        }

        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.UpdateCharacter(bookId, character);
            return Ok("Character updated successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Delete a character from a book
    [HttpDelete("{bookId}/characters/{characterId}")]
    public IActionResult DeleteCharacter(string bookId, string characterId)
    {
        try
        {
            var book = BookService.GetBook(bookId);
            if (book == null)
            {
                return NotFound("Book not found.");
            }
            BookService.DeleteCharacter(bookId, characterId);
            return Ok("Character deleted successfully.");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    // Get all characters from a book
    [HttpGet("{bookId}/characters")]
    public ActionResult<List<Character>> GetCharacters(string bookId)
    {
        var book = BookService.GetBook(bookId);
        if (book == null)
        {
            return NotFound("Book not found.");
        }

        return BookService.GetCharacters(bookId) ?? new List<Character>();
    }

}
