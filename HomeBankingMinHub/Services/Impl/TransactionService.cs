using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository)
        {
            this._transactionRepository = transactionRepository;
        }

        public Transaction CreateTransaction(long accountId, double amount, string loanName)
        {
            return new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Date = DateTime.Now,
                Description = loanName + " loan approved",
                Type = TransactionType.CREDIT
            };
        }

        public void SaveTransaction(Transaction transaction)
        {
            _transactionRepository.Save(transaction);
        }
    }
}
