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


}
