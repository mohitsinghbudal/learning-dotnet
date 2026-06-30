using Li_copy.I_InterfaceLayer.ReportInterface;
using Li_copy.Models.Book;
using Li_copy.Models.DTO;

namespace Li_copy.ServiceLayer.ReportService
{
    public class ReportService  : IReportService
    {
        private readonly IReportDLL _reportDLL;

        public ReportService(IReportDLL reportDLL)
        {
            _reportDLL = reportDLL;
        }
        public async Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedBooksAsync()
        {
            return await _reportDLL.GetMostBorrowedBooksAsync();
        }
        public async Task<decimal> TotalFineCollectedAsync()
        {
            return await _reportDLL.TotalFineCollectedAsync();
        }

        //public async Task<IEnumerable<FineDto>> TotalFineMonthly(DateTime start, DateTime end)
        //{
        //    return await _reportDLL.TotalFineMonthly(start, end);
        //}
        public async Task<int> BorrowingTrend(DateTime start, DateTime end)
        {
            return await _reportDLL.BorrowingTrend(start, end);
        }
        public async Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedCustomAsync(DateTime start, DateTime end)
        {
            return await _reportDLL.GetMostBorrowedCustomAsync(start, end);
        }
        public async Task<IEnumerable<TopBorrower>> TopBorrowerAsync(DateTime start, DateTime end)
        {
            return await _reportDLL.TopBorrowerAsync(start, end);
        }
    }
}
