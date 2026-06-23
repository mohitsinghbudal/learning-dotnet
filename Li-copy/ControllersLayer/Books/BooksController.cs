using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.Models.Book;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.Helper;

namespace Li_copy.ControllersLayer.Books
{
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
    }
}
