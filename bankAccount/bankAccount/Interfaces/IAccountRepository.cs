using bankAccount.Models;

namespace bankAccount.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllProducts();
        Task<Account?> GetProductById(Guid id);
        Task<Account?> AddProduct(Account account);
        Task<Account?> DeleteProduct(Guid id);
        Task<IEnumerable<Account>> GetProductByOwner(Guid ownerId);
        // ReSharper disable once IdentifierTypo
        Task<Account?> UpdateProdict(Guid id, Account account);
        Task<MbResult<IEnumerable<Account>>> AddTransfer(Transaction transfer);
        Task<Account?> CreateTransaction(Guid id, Transaction transaction);
        Task<decimal?> GetBalance(Guid id);
        Task<Account?> CloseAccountById(Guid id);
        Task AccrueInterestAsync(Guid accountId);
    }
}
