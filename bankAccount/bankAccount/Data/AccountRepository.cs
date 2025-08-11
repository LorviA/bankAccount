using bankAccount.Interfaces;
using bankAccount.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace bankAccount.Data
{
    public class AccountRepository(ApplicationDbContext context) : IAccountRepository
    {
#pragma warning disable CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.
        private readonly ApplicationDbContext _context = context;
#pragma warning restore CS9124 // Parameter is captured into the state of the enclosing type and its value is also used to initialize a field, property, or event.

        public async Task<Account?> AddProduct(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
            
            return account;
        }

        public async Task<Account?> UpdateProdict(Guid id, Account account)
        {
            var old = await Task.FromResult(_context.Accounts.FirstOrDefault(p => p.Id == id));
            if (old == null) 
            {
                return null;
            }
            old.Balance = account.Balance;
            old.InterestRate = account.InterestRate;
            await _context.SaveChangesAsync();
            return old;
        }

        public async Task<IEnumerable<Account>> GetAllProducts() => await Task.FromResult(_context.Accounts);

        public async Task<Account?> GetProductById(Guid id)
        {
            return await Task.FromResult(_context.Accounts.FirstOrDefault(c => c.Id == id));
        }

        public async Task<IEnumerable<Account>> GetProductByOwner(Guid ownerId) =>
            await Task.FromResult(_context.Accounts.Where(x => x.OwnerId == ownerId));

        public async Task<Account?> DeleteProduct(Guid id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == id);
            if (account == null)
            {
                return null;
            }

            await Task.FromResult(_context.Accounts.Remove(account));
            await _context.SaveChangesAsync();
            return (account);
        }

        public async Task<Account?> CreateTransaction(Guid id, Transaction transaction)
        {
            
                var account = await Task.FromResult(_context.Accounts.FirstOrDefault(a => a.Id == id));
                if (account == null)
                {
                    return null;
                }

                var balance = account.Balance;
                if (transaction.Type == TransactionType.Credit)
                    account.Balance = balance - transaction.Amount;
                if (transaction.Type == TransactionType.Debit)
                    account.Balance = balance + transaction.Amount;
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();

                return account;
        }

        public async Task<MbResult<IEnumerable<Account>>> AddTransfer(Transaction transfer)
        {
            const int maxRetries = 3;
            int attempt = 0;

            var strategy = context.Database.CreateExecutionStrategy();

            var fromAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transfer.AccountId);

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == transfer.CounterpartyAccountId);

            if (fromAccount == null || toAccount == null)
            {
                return MbResult<IEnumerable<Account>>.Failure(
                    MbError.NotFound("One or both accounts not found"));
            }
            var totalBefore = fromAccount.Balance + toAccount.Balance;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            strategy.Execute(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                
                    try
                    {
                        await using var transaction = await _context.Database.BeginTransactionAsync(
                            IsolationLevel.Serializable);

                        // Загрузка счетов с проверкой версии

                        if (fromAccount.Balance < transfer.Amount)
                            return MbResult<IEnumerable<Account>>.Failure(
                                MbError.Validation("Insufficient funds"));

                        // Выполнение перевода
                        fromAccount.Balance -= transfer.Amount;
                        toAccount.Balance += transfer.Amount;

                        // Создание записей о транзакциях
                        var creditTransfer = new Transaction
                        {
                            Id = Guid.NewGuid(),
                            Amount = transfer.Amount,
                            AccountId = transfer.AccountId,
                            CounterpartyAccountId = transfer.CounterpartyAccountId,
                            Time = DateTime.UtcNow,
                            Type = TransactionType.Credit,
                            Description = transfer.Description
                        };

                        var debitTransfer = new Transaction
                        {
                            Id = Guid.NewGuid(),
                            Amount = transfer.Amount,
                            AccountId = transfer.CounterpartyAccountId,
                            CounterpartyAccountId = transfer.AccountId,
                            Time = DateTime.UtcNow,
                            Type = TransactionType.Debit,
                            Description = transfer.Description
                        };

                        await _context.Transactions.AddAsync(creditTransfer);
                        await _context.Transactions.AddAsync(debitTransfer);

                        // Проверка целостности балансов
                        var totalAfter = fromAccount.Balance + toAccount.Balance;

                        if (totalAfter != totalBefore)
                        {
                            await transaction.RollbackAsync();
                            return MbResult<IEnumerable<Account>>.Failure(
                                MbError.Validation("Balance integrity check failed"));
                        }

                        //await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        //return MbResult<IEnumerable<Account>>.Success([fromAccount, toAccount]);
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (DbUpdateConcurrencyException ex)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
                        // Обработка конфликта версий
                        attempt++;

                        if (attempt >= maxRetries)
                        {
                            return MbResult<IEnumerable<Account>>.Failure(
                                MbError.Conflict("Concurrency conflict after maximum retries"));
                        }

                    }
                    catch (Exception ex)
                    {
                        return MbResult<IEnumerable<Account>>.Failure(
                            MbError.Internal($"Error processing transfer: {ex.Message}"));
                    }
                    

                    return MbResult<IEnumerable<Account>>.Failure(
                    MbError.Conflict("Concurrency conflict"));

            });
            await _context.SaveChangesAsync();
            return MbResult<IEnumerable<Account>>.Success([fromAccount, toAccount]);
        }
        

        public async Task<decimal?> GetBalance(Guid id)
        {
            return await Task.FromResult(_context.Accounts.FirstOrDefault(c => c.Id == id)?.Balance);
        }

        public async Task<Account?> CloseAccountById(Guid id)
        {
            var old = await Task.FromResult(_context.Accounts.FirstOrDefault(p => p.Id == id));
            if (old == null)
            {
                return null;
            }
            old.CloseDate = DateTime.UtcNow;
            old.Balance = old.Balance * (1 + old.InterestRate/100);
            await _context.SaveChangesAsync();
            return old;
        }

        public async Task AccrueInterestAsync(Guid accountId)
        {
#pragma warning disable EF1002
            await _context.Database.ExecuteSqlRawAsync($"CALL accrue_interest('{accountId}')");
#pragma warning restore EF1002
        }
    }
}
