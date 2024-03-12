using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IAccountService
    {
        string CreateNewNumberAccount();
        void SaveAccount(Account account);
        Account CreateAccountWithIdClientAndAccountNumber(long? idClient, string accountNumber);
        int GetCountAccountsByClient(long idClient);
        IEnumerable<Account> GetAccountsByClient(long idClient);
    }
}
