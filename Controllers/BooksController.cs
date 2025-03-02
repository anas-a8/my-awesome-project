using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL BOOKS (PUBLIC)
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            return Ok(await _context.Books.ToListAsync());
        }

        // ✅ GET BOOKS GROUPED BY AUTHOR
        [HttpGet("grouped-by-author")]
        public IActionResult GetBooksGroupedByAuthor()
        {
            var groupedBooks = _context.Books
                .GroupBy(b => b.Author)
                .Select(g => new { Author = g.Key, Books = g.ToList() })
                .ToList();

            return Ok(groupedBooks);
        }

        // ✅ GET TOP 3 MOST BORROWED BOOKS
        [HttpGet("most-borrowed")]
        public IActionResult GetMostBorrowedBooks()
        {
            var topBooks = _context.Books
                .OrderByDescending(b => b.TimesBorrowed)
                .Take(3)
                .ToList();

            return Ok(topBooks);
        }

        // ✅ PROTECTED: ADD A NEW BOOK (ADMIN ONLY)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return Ok("Book added successfully.");
        }

        // ✅ PROTECTED: DELETE BOOK (ADMIN ONLY)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();
            
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return Ok("Book deleted successfully.");
        }
    }
}
