using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        Account FindByIdAndClientEmail(long id, string email);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        int GetCountAccountsByClient(long clientId);
        bool ExistNumberAccount(string numberAccount);
    }
}
