using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ITransactionService
    {
        Transaction CreateTransaction(long accountId, double amount, string loanName);
        void SaveTransaction(Transaction transaction);
    }
}
