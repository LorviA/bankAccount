using bankAccount.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bankAccount.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllProducts();
        Task<Account?> GetProductById(Guid id);
        Task<Account?> AddProduct(Account account);
        Task<Account?> DeleteProduct(Guid id);
        Task<IEnumerable<Account>> GetProductByOwner(Guid OwnerId);
        Task<Account?> UpdateProdict(Guid Id, Account account);
        Task<IEnumerable<Account>> AddTransfer(Transaction transfer);
    }
}
