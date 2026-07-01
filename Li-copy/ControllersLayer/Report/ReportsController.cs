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
using System.Text;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;


namespace Li_copy.ControllersLayer.Report
{
    [Authorize]
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
        [AllowAnonymous]
        [HttpGet("csv")]
        public async Task<IActionResult> ExportCsv([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                // 1. Fetch all data from the service
                var borrowedBooks = await _report.GetMostBorrowedCustomAsync(start, end);
                var topBorrowers = await _report.TopBorrowerAsync(start, end);
                var totalFine = await _report.TotalFineCollectedAsync();

                var builder = new StringBuilder();
                builder.AppendLine("\uFEFFReport Export Date," + DateTime.Now.ToString("yyyy-MM-dd"));
                builder.AppendLine("Report Period," + start.ToString("yyyy-MM-dd") + " to " + end.ToString("yyyy-MM-dd"));
                builder.AppendLine();

                // 2. Add Most Borrowed Books Section
                builder.AppendLine("--- MOST BORROWED BOOKS ---");
                builder.AppendLine("Title,BorrowCount");
                foreach (var item in borrowedBooks)
                {
                    string title = item.Title.Contains(",") ? $"\"{item.Title}\"" : item.Title;
                    builder.AppendLine($"{title},{item.BorrowCount}");
                }
                builder.AppendLine();

                // 3. Add Top Borrowers Section
                builder.AppendLine("--- TOP BORROWERS ---");
                builder.AppendLine("UserName,BorrowCount");
                foreach (var user in topBorrowers)
                {
                    builder.AppendLine($"{user.UserName},{user.BorrowCount}");
                }
                builder.AppendLine();

                // 4. Add Fines Section
                builder.AppendLine("--- TOTAL FINES ---");
                builder.AppendLine("TotalFineCollected," + totalFine.ToString("F2"));

                return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"FullLibraryReport_{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var borrowedBooks = await _report.GetMostBorrowedCustomAsync(start, end);
                var topBorrowers = await _report.TopBorrowerAsync(start, end);
                var totalFine = await _report.TotalFineCollectedAsync();

                using var workbook = new XLWorkbook();

                // 1. Most Borrowed (Using InsertTable for cleaner code)
                var ws1 = workbook.Worksheets.Add("Most Borrowed");
                ws1.Cell(1, 1).InsertTable(borrowedBooks);
                ws1.Columns().AdjustToContents();

                // 2. Top Borrowers
                var ws2 = workbook.Worksheets.Add("Top Borrowers");
                ws2.Cell(1, 1).InsertTable(topBorrowers);
                ws2.Columns().AdjustToContents();

                // 3. Financials
                var ws3 = workbook.Worksheets.Add("Financials");
                ws3.Cell(1, 1).Value = "Total Fine Collected";
                ws3.Cell(1, 2).Value = totalFine;
                ws3.Cell(1, 2).Value = totalFine;
                ws3.Cell(1, 2).Style.NumberFormat.Format = "$#,##0.00"; // Added currency formatting
                ws3.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"LibraryReport_{DateTime.Now:yyyyMMdd}.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                // 1. Fetch all data
                var borrowedBooks = await _report.GetMostBorrowedCustomAsync(start, end);
                var topBorrowers = await _report.TopBorrowerAsync(start, end);
                var totalFine = await _report.TotalFineCollectedAsync();

                QuestPDF.Settings.License = LicenseType.Community;

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Centimetre);
                        page.Header().Text("Library Report").FontSize(20).SemiBold().FontColor(Colors.Green.Medium);

                        // Use a Column to stack elements vertically
                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            // --- Most Borrowed Books ---
                            column.Item().PaddingTop(10).Text("Most Borrowed Books").FontSize(14).Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c => { c.ConstantColumn(300); c.RelativeColumn(); });
                                table.Header(h => { h.Cell().Text("Title").Bold(); h.Cell().Text("Count").Bold(); });
                                foreach (var item in borrowedBooks)
                                {
                                    table.Cell().Text(item.Title);
                                    table.Cell().Text(item.BorrowCount.ToString());
                                }
                            });

                            // --- Top Borrowers ---
                            column.Item().PaddingTop(20).Text("Top Borrowers").FontSize(14).Bold();
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                                table.Header(h => { h.Cell().Text("User").Bold(); h.Cell().Text("Count").Bold(); });
                                foreach (var user in topBorrowers)
                                {
                                    table.Cell().Text(user.UserName);
                                    table.Cell().Text(user.BorrowCount.ToString());
                                }
                            });

                            // --- Financials ---
                            column.Item().PaddingTop(20).Text("Financial Summary").FontSize(14).Bold();
                            column.Item().Text($"Total Fine Collected: ${totalFine:F2}").FontSize(12);
                        });
                    });
                });

                return File(document.GeneratePdf(), "application/pdf", $"LibraryReport_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

    }

}   

