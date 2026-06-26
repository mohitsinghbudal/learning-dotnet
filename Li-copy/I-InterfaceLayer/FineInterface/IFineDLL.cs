namespace Li_copy.I_InterfaceLayer.FineInterface

{
    using Li_copy.Models.Fine;
    public interface IfineDLL
    {
        Task<Fine?> GetFineByLoanIdAsync(int loanId);
        Task<int> CreateFineAsync(Fine fine);
        Task<bool> UpdateFineAsync(Fine fine);
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int id);

      
    }
}
