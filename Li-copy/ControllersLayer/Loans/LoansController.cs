using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Loans;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Loans
{
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
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);

            if (loan == null)
                return NotFound();

            return Ok(loan);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Loan loan)
        {
            var id = await _loanService.CreateLoanAsync(loan);

            return Ok(new
            {
                Message = "Loan created successfully",
                LoanId = id
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Loan loan)
        {
            loan.Id = id;

            var result = await _loanService.UpdateLoanAsync(loan);

            if (!result)
                return NotFound();

            return Ok("Loan updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _loanService.DeleteLoanAsync(id);

            if (!result)
                return NotFound();

            return Ok("Loan deleted successfully");
        }
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id, [FromBody] ReturnBookDto dto)
        {
            var result = await _loanService.ReturnBookAsync(id, dto.ReturnDate);
            if (!result) return NotFound(new { Message = "Loan record execution failed." });
            return Ok(new { Message = "Book processing verified complete." });
        }



    }
    public class ReturnBookDto
    {
        public DateTime ReturnDate { get; set; }
    }
}
