using Li_copy.Models.Book;
using Li_copy.Models.Loans;

namespace Li_copy.I_InterfaceLayer.LoanInterface
{
    public interface IloanService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<int> CreateLoanAsync(Loan loan);
        Task<bool> UpdateLoanAsync(Loan loan);
        Task<bool> DeleteLoanAsync(int id);
        Task<bool> ReturnBookAsync(int loanId);
        Task<int> VerifyReturnAsync(int loanId, int teacherId);
        Task<IEnumerable<LoanHistoryDto>> GetPersonalHistoryAsync(int userId, int roleId);
        Task<IEnumerable<LoanHistoryDto>> GetGlobalBorrowRecordsAsync(int roleId);

        Task<IEnumerable<LoanDTO?>> GetUserLoanAsync(int id);

        Task<bool> VerifyBorrowAsync(int LoanId , int IssueId);
    }

}
