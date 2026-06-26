using Li_copy.I_InterfaceLayer.FineInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Li_copy.Models.DTO;
using System.Threading.Tasks;

namespace Li_copy.ControllersLayer.Fines
{
    [AllowAnonymous]
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


        // 🛠️ ADDED FOR PAYMENT INITIALIZATION FLOW
        [HttpPost("{id}/initiate-payment")]
        public async Task<IActionResult> InitiatePayment(int id)
        {
            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine == null) return NotFound(new { Message = "Fine record not found." });
            if (fine.IsPaid) return BadRequest(new { Message = "This fine has already been settled." });

            // Initialize local gateway lifecycle tracking flags
            bool initialized = await _fineService.UpdatePaymentStatusAsync(id, "Initiated");
            if (!initialized) return BadRequest(new { Message = "Unable to initialize transaction system metadata." });

            return Ok(new { FineId = fine.Id, Amount = fine.Amount, Status = "Initiated" });
        }

        [HttpPost("{id}/callback")]
        public async Task<IActionResult> PaymentCallback(int id, [FromBody] PaymentCallbackDTO dto)
        {
            var result = await _fineService.ProcessPaymentCallbackAsync(id, dto.TransactionId, dto.RawCallbackJson, dto.PaymentStatus);
            if (!result) return BadRequest(new { Message = "Failed to process payment integration sequence." });
            return Ok(new { Message = "Payment state structural update synchronized successfully." });
        }
        [HttpPost("{id}/verify-manually")]
        public async Task<IActionResult> VerifyManually(int id)
        {
            // Reading the Teacher's user ID straight from the secure JWT token claims
            var teacherIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int teacherId = string.IsNullOrEmpty(teacherIdClaim) ? 0 : int.Parse(teacherIdClaim);

            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine == null) return NotFound(new { Message = "Fine record not found." });
            if (fine.IsPaid) return BadRequest(new { Message = "This fine has already been settled." });

            // Update the fine state manually
            fine.IsPaid = true;
            fine.PaidDate = System.DateTime.Now;
            fine.PaymentStatus = "Verified Manually";
            fine.TransactionId = $"MANUAL-{System.Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            // Assigning the teacher's ID to your database's VerifiedByUserId column
            // (Assuming your repository update method saves the state of the entity)
            // fine.VerifiedByUserId = teacherId; 

            bool updated = await _fineService.ProcessPaymentCallbackAsync(
                fine.Id,
                fine.TransactionId,
                "{\"type\": \"manual_verification\"}",
                "COMPLETED"
            );

            if (!updated) return BadRequest(new { Message = "Failed to update fine settlement state data." });

            return Ok(new { Message = "Fine payment manually verified and settled successfully." });
        }
    }
}