using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
    }
}