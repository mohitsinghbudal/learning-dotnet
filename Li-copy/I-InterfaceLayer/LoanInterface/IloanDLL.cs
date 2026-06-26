using Li_copy.Models.Book;
using Li_copy.Models.Loans;

namespace Li_copy.I_InterfaceLayer.LoanInterface
{
    public interface IloanDLL
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<int> CreateLoanAsync(Loan loan);
        Task<bool> UpdateLoanAsync(Loan loan);
        Task<bool> DeleteLoanAsync(int id);
        Task<IEnumerable<LoanHistoryDto>> GetPersonalBorrowHistoryAsync(int userId);
        Task<IEnumerable<LoanHistoryDto>> GetAllBorrowedBooksAsync();
        Task<IEnumerable<LoanDTO?>> GetUserLoanAsync(int userId);

        Task<bool> VerifyBorrowAsync(int LoanId ,int IssueId);

         Task UpdateLoanAsync(LoanDTO loan);

        Task<bool> ReturnBookAsync(int loanId);
        Task<bool> UpdateAndReleaseInventoryAsync(Loan loan);
    }
}
