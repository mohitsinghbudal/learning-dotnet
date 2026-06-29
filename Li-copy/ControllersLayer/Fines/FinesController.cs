using Li_copy.Helper;
using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Li_copy.ControllersLayer.Fines
{
    [Authorize]
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

        [HttpGet("loan")]
        public async Task<IActionResult> GetByLoanId()
        {
            var roleId = ClaimsHelper.GetRoleId(User);
            var userId = ClaimsHelper.GetUserId(User);

            if (roleId != 2) return Unauthorized();

            var fines = await _fineService.GetFineByUserIdAsync(userId);
            return fines != null ? Ok(fines) : NotFound();
        }

        [HttpPost("initiate-payment/pay")]
        public async Task<IActionResult> InitiatePayment([FromBody] EsewaPaymentRequestDTO request)
        {
            var result = await _fineService.PayFineAsync(request);
            return result != null ? Ok(result) : NotFound("Fine Not Found or already paid");
        }

        [HttpPost("{id}/callback")]
        public async Task<IActionResult> PaymentCallback([FromBody] PaymentCallbackDTO dto)
        {
            if (dto == null) return BadRequest("Invalid callback.");
            bool result = await _fineService.ProcessPaymentCallbackAsync(dto);
            return result ? Ok(new { Message = "Payment processed successfully." }) : BadRequest("Payment processing failed.");
        }

        [HttpPost("{id}/verify-manually")]
        public async Task<IActionResult> VerifyManually(int id)
        {
            if (ClaimsHelper.GetRoleId(User) != 2) return Unauthorized();

            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine == null) return NotFound(new { Message = "Fine record not found." });
            if (fine.IsPaid) return BadRequest(new { Message = "This fine has already been settled." });

            bool updated = await _fineService.ProcessPaymentCallbackAsync(
                fine.Id,
                $"MANUAL-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                "{\"type\": \"manual_verification\"}",
                "COMPLETED"
            );

            return updated ? Ok(new { Message = "Fine payment manually verified and settled successfully." })
                           : BadRequest(new { Message = "Failed to update fine settlement state data." });
        }

        [AllowAnonymous]
        [HttpGet("esewa/success")]
        public async Task<IActionResult> EsewaSuccess(string data)
        {
            if (string.IsNullOrEmpty(data)) return BadRequest("No payment data received.");

            var result = await _fineService.ProcessEsewaSuccessAsync(data);
            return result ? Ok("Payment Successful") : BadRequest("Payment verification failed.");
        }

        [AllowAnonymous]
        [HttpGet("esewa/failure")]
        public IActionResult EsewaFailure(string data)
        {
            // Handle failure logic here (e.g., log the failure, redirect to frontend failure page)
            return Ok(new { Message = "Payment failed or was cancelled by the user.", Data = data });
        }
    }
}