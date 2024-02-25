using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using QuickEndpoint_ApiExample.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuickEndpoint_ApiExample
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private static readonly List<Book> books = new List<Book>
        {
            new Book { Id = "1", Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Year = 1925 },
            new Book { Id = "2", Title = "Moby-Dick", Author = "Herman Melville", Year = 1851 },
            // Add more books here
        };

        [HttpGet]
        public IEnumerable<Book> GetBooks()
        {
            return books;
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(string id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return book;
        }

        [HttpPost]
        public IActionResult CreateBook(Book book)
        {
            books.Add(book);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(string id, Book book)
        {
            var index = books.FindIndex(b => b.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            books[index] = book;
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(string id)
        {
            var index = books.FindIndex(b => b.Id == id);
            if (index == -1)
            {
                return NotFound();
            }
            books.RemoveAt(index);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PatchBook(string id, [FromBody] JsonPatchDocument<Book> patchDoc)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(book);

            return NoContent();
        }
    }
}
