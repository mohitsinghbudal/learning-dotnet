namespace Li_copy.I_InterfaceLayer.FineInterface

{
    using Li_copy.Models.Fine;
    public interface IfineDLL
    {
        Task<Fine?> GetFineByLoanIdAsync(int UserId);
        Task<int> CreateFineAsync(Fine fine);
        Task<bool> UpdateFineAsync(Fine fine);
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int UserId);

        Task<IEnumerable<Fine?>> GetFineByUserIdAsync(int userId);

        Task<bool> UpdatePaymentInitializationAsync(
    int fineId,
    string transactionId,
    string requestJson,
    string responseJson);

        Task<Fine?> GetFineByTransactionIdAsync(string transactionId);
        
        Task<bool> MarkFinePaidAsync(
            string transactionId,
            string callbackJson,
            string verificationJson);
    }
}
