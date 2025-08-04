using bankAccount.Interfaces;
using bankAccount.Models;
using System.Security.Principal;

namespace bankAccount.Data
{
    public class AccountRepository: IAccountRepository
    {
        private static List<Account> _accounts;

        public AccountRepository() 
        {
            _accounts =
            [
                new() { Id = Guid.NewGuid(), OwnerId = Guid.NewGuid(), Type = AccountType.Credit, Balance = 1000, InterestRate = 0},
                new() { Id = Guid.NewGuid(), OwnerId = Guid.NewGuid(), Type = AccountType.Checking, Balance = 320, InterestRate = 0}
            ];
        }

        public async Task<Account?> AddProduct(Account account)
        {
            _accounts.Add(account);
            var result = account;
            return result;
        }

        public async Task<Account?> UpdateProdict(Guid Id, Account account)
        {
            var old = await Task.FromResult(_accounts.FirstOrDefault(p => p.Id == Id));
            if (old != null) 
            {
                old.Balance = account.Balance;
                old.InterestRate = account.InterestRate;
                return old;
            }
            return null;
        }
        public async Task<IEnumerable<Account>> GetAllProducts() => await Task.FromResult(_accounts);

        public async Task<Account?> GetProductById(Guid id)
        {
            Console.WriteLine("Repo");
            Console.WriteLine(id);
            var account = await Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
            Console.WriteLine(account);
            if (account != null)
            {
                Console.WriteLine("Yes");
                return account;
            }
            return null;
        }
        public async Task<IEnumerable<Account>> GetProductByOwner(Guid OwnerId) =>
            await Task.FromResult(_accounts.Where(a => a.OwnerId == OwnerId));

        public async Task<Account?> DeleteProduct(Guid id)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                await Task.FromResult(_accounts.Remove(account));
                return account;
            }
            return null;
        }

        public async Task<Account?> CreateTransaction(Guid id, Transaction transaction)
        {
            var account = await Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
            if (account != null)
            {
                var balance = account.Balance;
                if(transaction.Type == TransactionType.Credit)
                    account.Balance = balance - transaction.Amount;
                if (transaction.Type == TransactionType.Debit)
                    account.Balance = balance + transaction.Amount;
                account.Transactions.Add(transaction);
                return account;
            }
            return null;
        }

        public async Task<IEnumerable<Account>> AddTransfer(Transaction transfer)
        {
            var fromAccount = await Task.FromResult(_accounts.FirstOrDefault(a => a.Id == transfer.AccountId));
            var toAccount = await Task.FromResult(_accounts.FirstOrDefault(a => a.Id == transfer.CounterpartyAccountId));

            if (fromAccount == null || toAccount == null) return null;

            if (fromAccount.Balance < transfer.Amount)
                return null;

            var balanceF = fromAccount.Balance;
            var balanceT = toAccount.Balance;
            fromAccount.Balance = balanceF - transfer.Amount;
            toAccount.Balance = balanceT+  transfer.Amount;

            var fTransfer = transfer;
            fTransfer.Type = TransactionType.Credit;
            fromAccount.Transactions.Add(fTransfer);
            var tTransfer = transfer;
            tTransfer.Type = TransactionType.Debit;
            toAccount.Transactions.Add(tTransfer);
            return [fromAccount, toAccount];
        }
    }
}
