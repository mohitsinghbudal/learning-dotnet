using Li_copy.Models.Book;
using Li_copy.Models.DTO;

namespace Li_copy.I_InterfaceLayer.ReportInterface
{
    public interface IReportService
    {
       
            Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedBooksAsync();
            Task<decimal> TotalFineCollectedAsync();
        //Task<IEnumerable<FineDto>> TotalFineMonthly(DateTime start, DateTime end);
        Task<int> BorrowingTrend(DateTime start, DateTime end);
        Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedCustomAsync(DateTime start, DateTime end);
        Task<IEnumerable<TopBorrower>> TopBorrowerAsync(DateTime start, DateTime end);

    }
    public interface IReportDLL
    {

        Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedBooksAsync();
        Task<decimal> TotalFineCollectedAsync();
        //Task<IEnumerable<FineDto>> TotalFineMonthly(DateTime start, DateTime end);
        Task<int> BorrowingTrend(DateTime start, DateTime end);

        Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedCustomAsync(DateTime start, DateTime end);
        Task<IEnumerable<TopBorrower>> TopBorrowerAsync(DateTime start, DateTime end);

    }
}
