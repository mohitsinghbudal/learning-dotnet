using Li_copy.I_InterfaceLayer.FineInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Li_copy.Models.DTO;

namespace Li_copy.ControllersLayer.Fines
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly IfineService _fineService;

        public FinesController(IfineService fineService)
        {
            _fineService = fineService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _fineService.GetAllFinesAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine == null) return NotFound();
            return Ok(fine);
        }

        [HttpGet("loan/{loanId}")]
        public async Task<IActionResult> GetByLoanId(int loanId)
        {
            var fine = await _fineService.GetFineByLoanIdAsync(loanId);
            if (fine == null) return NotFound();
            return Ok(fine);
        }

        [HttpPost("{id}/callback")]
        public async Task<IActionResult> PaymentCallback(int id, [FromBody] PaymentCallbackDTO dto)
        {
            var result = await _fineService.ProcessPaymentCallbackAsync(id, dto.TransactionId, dto.RawCallbackJson, dto.PaymentStatus);
            if (!result) return BadRequest(new { Message = "Failed to process payment integration sequence." });
            return Ok(new { Message = "Payment state structural update synchronized successfully." });
        }
    }

    
}

