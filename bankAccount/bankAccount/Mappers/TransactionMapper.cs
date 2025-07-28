using bankAccount.Dtos.TransactionDto;
using bankAccount.Models;

namespace bankAccount.Mappers
{
    public static class TransactionMapper
    {
        public static Transaction ToTransactionFromCreate(this CreateTransactionDto transactionModel)
        {
            return new Transaction
            {
                AccountId = transactionModel.AccountId,
                Amount = transactionModel.Amount,
                Currency = transactionModel.Currency,
                Type = transactionModel.Type,
                Description = transactionModel.Description,
                Time = transactionModel.Time
            };
        }

        public static Transaction ToTransferFromCreate(this CreateTransferDto transactionModel)
        {
            return new Transaction
            {
                AccountId = transactionModel.AccountId,
                CounterpartyAccountId = transactionModel.CounterpartyAccountId,
                Amount = transactionModel.Amount,
                Currency = transactionModel.Currency,
                Type = transactionModel.Type,
                Description = transactionModel.Description,
                Time = transactionModel.Time
            };
        }
    }
}
