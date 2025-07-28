using bankAccount.Dtos.AccountDto;
using bankAccount.Models;

namespace bankAccount.Mappers
{
    public static class AccountMapper
    {
        public static Account ToAccountFromCreate(this CreateAccountDto accountModel)
        {
            return new Account
            {
                OwnerId = accountModel.OwnerId,
                Type = accountModel.Type,
                Balance = accountModel.Balance,
                InterestRate = accountModel.InterestRate,
                OpeningDate = accountModel.OpeningDate,
                Transactions = accountModel.Transactions
            };
        }

        public static Account ToAccountFromUpdate(this UpdateAccountDto accountModel, Guid Id)
        {
            return new Account
            {
                OwnerId = accountModel.OwnerId,
                Type = accountModel.Type,
                Balance = accountModel.Balance,
                InterestRate = accountModel.InterestRate,
                CloseDate = accountModel.CloseDate,
                Transactions = accountModel.Transactions
            };
        }
    }
}