using Li_copy.Helper;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.Models.Book;
using Li_copy.Models.Loans;
using Li_copy.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Li_copy.ControllersLayer.Books
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookServices _bookService;

        public BooksController(IBookServices bookService)
        {
            _bookService = bookService;
        }


        [HttpGet("count")]
        public async Task<int> GetCount()
        {
            return await _bookService.GetCount();
        }

        //global books read
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var roles = await _bookService.GetBooksAsync(page, pageSize);
            return Ok(roles);
        }
        //[AllowAnonymous]
        [HttpGet("approved")]
        public async Task<IActionResult> GetVerifiedBookAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var books = await _bookService.GetVerifiedBookAsync(page, pageSize);
            return Ok(books);

        }
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var userId = ClaimsHelper.GetUserId(User);

            Console.WriteLine($"RoleId = {roleId}");

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} : {claim.Value}");
            }

            Console.WriteLine("we reached inside book add :" + roleId + userId);
            if (roleId != 3 && roleId != 1)
            {
                return Forbid();
            }



            book.CreatedByUserId = userId;

            try
            {
                var id = await _bookService.AddBookAsync(book, roleId, userId);

                return Ok(new { BookId = id });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("verify/newBook/{id}")]
        public async Task<IActionResult> VerifyBook(int id)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var adminId = ClaimsHelper.GetUserId(User);

            var result = await _bookService.VerifyBookAsync(id, roleId, adminId);

            if (!result)
            {
                return BadRequest("Book not found or already verified.");
            }

            return Ok("Book verified successfully.");

        }

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBookAsync([FromBody] BorrowRequestDto request)
        {
            Console.WriteLine("reached inside borrow");


            var roleId = ClaimsHelper.GetRoleId(User);
            var studentId = ClaimsHelper.GetUserId(User);


            try
            {
                // Passes control to the application layer to create a pending request records
                var result = await _bookService.CreateBorrowRequestAsync(request.BookId, roleId, studentId);

                if (!result)
                    return BadRequest("Unable to submit borrow request. Out of copies or unauthorized.");

                return Ok(new { message = "📖 Borrow request sent to librarian" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("borrow/approve/{LoanId}")]
        public async Task<IActionResult> ApproveBorrowAsync(int LoanId)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var librarianId = ClaimsHelper.GetUserId(User);

            Console.WriteLine(LoanId + librarianId);
            Console.WriteLine($"RoleId = {roleId}");
            Console.WriteLine($"UserId = {librarianId}");

            try
            {
                // Pass to the service layer where it checks if roleId == 3 (Librarian) or 1 (Admin)
                var result = await _bookService.ApproveBorrowRequestAsync(LoanId, roleId, librarianId);

                if (!result)
                    return Forbid(); // User lacks authority or request record doesn't exist

                return Ok(new { message = "✅ Borrow request approved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchByAsync( [FromQuery] string? title, [FromQuery] string? author, [FromQuery] string? category)
        {
           

            try
            {
                var result = await _bookService.SearchBookAsync(title, author, category);

                if (result == null )
                {
                    return NotFound(new { message = "No books found matching your criteria." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception here (e.g., _logger.LogError(ex, "..."));
                return StatusCode(500, new { message = "An error occurred while processing your search." });
            }
        }

    }
}

    // Small data transfer object wrapper for handling clean JSON payloads from React
    public class BorrowRequestDto
    {
        public int BookId { get; set; }
    }

