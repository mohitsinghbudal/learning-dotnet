using Li_copy.Helper;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.Models.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Books
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookServices _bookService;

        public BooksController(IBookServices bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            var roles = await _bookService.GetBooksAsync();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var userId = ClaimsHelper.GetUserId(User);

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

        [Authorize]
        [HttpPost("verify/{id}")]
        public async Task<IActionResult> VerifyBook(int id)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var adminId = ClaimsHelper.GetUserId(User);

            var result = await _bookService.VerifyBookAsync(id, roleId, adminId);

            if (!result)
                return Forbid();

            return Ok("Book verified");
        }

        //[HttpPost("borrow")]
        //public async Task<IActionResult> BorrowBookAsync()
        //{
        //    var roleId = ClaimsHelper.GetRoleId(User);
        //    var adminId = ClaimsHelper.GetUserId(User);

        //}

        //[HttpPost("borrow/approve/{id}")]
        //public async Task<IActionResult> ApproveBorrowAsync(int id)
        //{
        //    var roleId = ClaimsHelper.GetRoleId(User);
        //    var adminId = ClaimsHelper.GetUserId(User);


        //}

        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBookAsync([FromBody] BorrowRequestDto request)
        {
            if (request == null || request.BookId <= 0)
                return BadRequest("Invalid book request payload.");

            // Return 401 if the caller is not authenticated instead of allowing the helper to throw
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized();

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

        [HttpPost("borrow/approve/{id}")]
        public async Task<IActionResult> ApproveBorrowAsync(int id)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var librarianId = ClaimsHelper.GetUserId(User);

            try
            {
                // Pass to the service layer where it checks if roleId == 3 (Librarian) or 1 (Admin)
                var result = await _bookService.ApproveBorrowRequestAsync(id, roleId, librarianId);

                if (!result)
                    return Forbid(); // User lacks authority or request record doesn't exist

                return Ok(new { message = "✅ Borrow request approved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // Small data transfer object wrapper for handling clean JSON payloads from React
    public class BorrowRequestDto
    {
        public int BookId { get; set; }
    }
}

