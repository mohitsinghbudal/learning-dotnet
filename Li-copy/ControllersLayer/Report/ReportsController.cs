using Li_copy.DataLayer.Report;
using Li_copy.I_InterfaceLayer.ReportInterface;
using Li_copy.I_InterfaceLayer.ReportInterface;
using Li_copy.Models.Book;
using Li_copy.Models.DTO;
using Li_copy.ServiceLayer.ReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace Li_copy.ControllersLayer.Report
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _report;

        public ReportsController(IReportService report)
        {
            _report = report;
        }

        [HttpGet("mostborrow")]
        public async Task<ActionResult<IEnumerable<MostBorrowedBookDto>>> Get()
        {
            try
            {
                var books = await _report.GetMostBorrowedBooksAsync();

                return Ok(books);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("totalfine")]
        public async Task<ActionResult<decimal>> getFine()
        {
            try
            {
                var total = await _report.TotalFineCollectedAsync();
                return Ok(new { TotalFine = total }); // Returning as an object is cleaner for JSON
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //    [HttpGet("monthly-fine")]
        //    public async Task<ActionResult<IEnumerable<FineDto>>> GetMonthlyFine(
        //[FromQuery] DateTime start,
        //[FromQuery] DateTime end)
        //    {
        //        try
        //        {
        //            var result = await _report.TotalFineMonthly(start, end);
        //            return Ok(result);
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
        [HttpGet("borrowing-trend")]
        public async Task<ActionResult<int>> GetBorrowingTrend(
    [FromQuery] DateTime start,
    [FromQuery] DateTime end)
        {
            try
            {
                var count = await _report.BorrowingTrend(start, end);
                return Ok(new { Count = count });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("most-borrowed-custom")]
        public async Task<ActionResult<IEnumerable<MostBorrowedBookDto>>> GetMostBorrowedCustom(
    [FromQuery] DateTime start,
    [FromQuery] DateTime end)
        {
            try
            {
                var result = await _report.GetMostBorrowedCustomAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("top-borrower")]
        public async Task<ActionResult<IEnumerable<TopBorrower>>> TopBorrower(
    [FromQuery] DateTime start,
    [FromQuery] DateTime end)
        {
            try
            {
                var result = await _report.TopBorrowerAsync(start, end);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }

}   

