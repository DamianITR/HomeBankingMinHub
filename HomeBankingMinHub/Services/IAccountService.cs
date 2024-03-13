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
        IEnumerable<Account> GetAllAccounts();
        Account FindByIdAndEmail(long id, string email);
        Account FindByNumber(string accountNumber);
        bool ClientHaveAccount(long idClient, string email);
    }
}
