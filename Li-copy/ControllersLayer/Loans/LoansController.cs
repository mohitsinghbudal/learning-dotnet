//using Li_copy.Helper;
//using Li_copy.I_InterfaceLayer.LoanInterface;
//using Li_copy.Models.Loans;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Li_copy.ControllersLayer.Loans
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LoansController : ControllerBase
//    {
//        private readonly IloanService _loanService;

//        public LoansController(IloanService loanService)
//        {
//            _loanService = loanService;
//        }

//        // 🛠️ FIXED: Used by Admin (1) and Teacher (3) in your React app global fetch
//        // Also removed the trailing slash issue by matching root route cleanly
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var roleId = ClaimsHelper.GetRoleId(User);

//            // React app says: isTeacher (3) || isAdmin (1) can view global history
//            if (roleId != 1 && roleId != 3)
//            {
//                return Unauthorized();
//            }

//            var loans = await _loanService.GetAllLoansAsync();
//            return Ok(loans);
//        }

//        // 🛠️ FIXED: Was throwing 405 because React called GET api/Loans/17
//        // Route now correctly catches the incoming userId from your frontend fetch string `${historyApi}/${user.id}`
//        [HttpGet("myLoans")]
//        public async Task<IActionResult> GetUserLoanAsync()
//        {
//            var roleId = ClaimsHelper.GetRoleId(User);
//            var userId = ClaimsHelper.GetUserId(User);



//            // React allows Student (2) to see their personal records
//            if (roleId != 2 )
//            {
//                return Unauthorized();
//            }

//            // Using the 'id' passed dynamically from the frontend URL parameters
//            var loan = await _loanService.GetUserLoanAsync(userId);
//            if (loan == null)
//                return NotFound();

//            return Ok(loan);
//        }

//        // student initiate borrows the book
//        [HttpPost]
//        public async Task<IActionResult> Create(Loan loan)
//        {
//            var roleId = ClaimsHelper.GetRoleId(User);

//            if (roleId != 2 && roleId != 3) // Allowing both Student or Teacher roles
//            {
//                return Unauthorized();
//            }

//            var id = await _loanService.CreateLoanAsync(loan);

//            return Ok(new
//            {
//                Message = "Loan request submitted successfully",
//                LoanId = id
//            });
//        }

//        // 🛠️ FIXED ROUTE NAME: Avoids conflicting with base actions
//        // Teacher verify the borrow-book
//        [HttpPost("verify/{loanId}")]
//        public async Task<IActionResult> VerifyBorrowAsync(int loanId)
//        {
//            var userId = ClaimsHelper.GetUserId(User);
//            var roleId = ClaimsHelper.GetRoleId(User);

//            if (roleId != 3) // Strictly Teacher role 
//            {
//                return Unauthorized();
//            }

//            var result = await _loanService.VerifyBorrowAsync(loanId, userId);

//            if (!result)
//            {
//                return BadRequest(new { message = "Borrow verification failed." });
//            }

//            return Ok(new { message = "Book checkout verified successfully." });
//        }

//        // student return the book borrowed
//        // Matches your React payload: axios.post(`${historyApi}/${loanId}/return`)
//        [HttpPost("{id}/return")]
//        public async Task<IActionResult> ReturnBook(int id)
//        {
//            var result = await _loanService.ReturnBookAsync(id);
//            if (!result) return NotFound(new { Message = "Loan record execution failed or already returned." });
//            return Ok(new { Message = "Book return completed successfully." });
//        }

//        [HttpGet("my-borrowings")]
//        public async Task<IActionResult> GetMyBorrowings()
//        {
//            var userId = ClaimsHelper.GetUserId(User);
//            var roleId = ClaimsHelper.GetRoleId(User);

//            try
//            {
//                var history = await _loanService.GetPersonalHistoryAsync(userId, roleId);
//                return Ok(history);
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Forbid(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpGet("all-borrowings")]
//        public async Task<IActionResult> GetAllBorrowings()
//        {
//            var roleId = ClaimsHelper.GetRoleId(User);

//            try
//            {
//                var globalHistory = await _loanService.GetGlobalBorrowRecordsAsync(roleId);
//                return Ok(globalHistory);
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                return Forbid(ex.Message);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var result = await _loanService.DeleteLoanAsync(id);

//            if (!result)
//                return NotFound();

//            return Ok("Loan deleted successfully");
//        }
//    }

//    public class ReturnBookDto
//    {
//        public DateTime ReturnDate { get; set; }
//    }
//}


using Li_copy.Helper;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Loans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Loans
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IloanService _loanService;

        public LoansController(IloanService loanService)
        {
            _loanService = loanService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roleId = ClaimsHelper.GetRoleId(User);

            if (roleId != 1 && roleId != 3)
            {
                return Unauthorized();
            }

            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("myLoans")]
        public async Task<IActionResult> GetUserLoanAsync()
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var userId = ClaimsHelper.GetUserId(User);

            if (roleId != 2)
            {
                return Unauthorized();
            }

            var loan = await _loanService.GetUserLoanAsync(userId);
            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        // Student initiates borrowing the book
        [HttpPost]
        public async Task<IActionResult> Create(Loan loan)
        {
            var roleId = ClaimsHelper.GetRoleId(User);

            if (roleId != 2) // Explicitly restricted back to Student only as requested in frontend updates
            {
                return Unauthorized();
            }

            var id = await _loanService.CreateLoanAsync(loan);

            return Ok(new
            {
                Message = "Loan request submitted successfully",
                LoanId = id
            });
        }

        // Teacher verifies the borrow checkout request
        [HttpPost("verify/{loanId}")]
        public async Task<IActionResult> VerifyBorrowAsync(int loanId)
        {
            var userId = ClaimsHelper.GetUserId(User);
            var roleId = ClaimsHelper.GetRoleId(User);

            if (roleId != 3)
            {
                return Unauthorized();
            }

            var result = await _loanService.VerifyBorrowAsync(loanId, userId);

            if (!result)
            {
                return BadRequest(new { message = "Borrow verification failed." });
            }

            return Ok(new { message = "Book checkout verified successfully." });
        }

        // Student marking a book as returned (Changes status to "Pending Return" or handles direct initial return submission)
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            if (roleId != 2)
            {
                return Unauthorized();
            }

            var result = await _loanService.ReturnBookAsync(id);
            if (!result) return NotFound(new { Message = "Loan record execution failed or already processed." });
            return Ok(new { Message = "Book return submitted. Waiting for teacher verification." });
        }
        // POST: api/VerifyReturn/{loanId}
        [HttpPost("verifyReturn/{loanId}")]
        public async Task<IActionResult> VerifyReturnAsync(int loanId)
        {
            var teacherId = ClaimsHelper.GetUserId(User);
            var roleId = ClaimsHelper.GetRoleId(User);

            // Strictly check that the user executing this action is a Teacher (Role 3)
            if (roleId != 3)
            {
                return Unauthorized(new { message = "Only teachers can verify book returns." });
            }

            // Calls your service layer, passing both the loan assignment identifier and the acting teacher's user context ID
            var result = await _loanService.VerifyReturnAsync(loanId, teacherId);

            if (result != 100)
            {
                return BadRequest(new { message = "Return verification execution failed. Check record status." });
            }

            return Ok(new { message = "Book return verified and closed out successfully." });
        }

        [HttpGet("my-borrowings")]
        public async Task<IActionResult> GetMyBorrowings()
        {
            var userId = ClaimsHelper.GetUserId(User);
            var roleId = ClaimsHelper.GetRoleId(User);

            try
            {
                var history = await _loanService.GetPersonalHistoryAsync(userId, roleId);
                return Ok(history);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all-borrowings")]
        public async Task<IActionResult> GetAllBorrowings()
        {
            var roleId = ClaimsHelper.GetRoleId(User);

            try
            {
                var globalHistory = await _loanService.GetGlobalBorrowRecordsAsync(roleId);
                return Ok(globalHistory);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _loanService.DeleteLoanAsync(id);

            if (!result)
                return NotFound();

            return Ok("Loan deleted successfully");
        }


    }
}