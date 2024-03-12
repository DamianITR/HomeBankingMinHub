using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services.Impl
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account CreateAccountWithIdClientAndAccountNumber(long? idClient, string accountNumber)
        {
            var newAccount = new Account
            {
                Number = accountNumber,
                CreationDate = DateTime.Now,
                Balance = 0,
                ClientId = (long)idClient,
            };

            return newAccount;
        }

        public string CreateNewNumberAccount()
        {
            string newNumberAccount;
            do
            {
                newNumberAccount = GeneratorNumbers.CreateNewNumberAccount();
            }
            while (_accountRepository.ExistNumberAccount(newNumberAccount));

            return newNumberAccount;
        }

        public Account FindByIdAndEmail(long id, string email)
        {
            return _accountRepository.FindByIdAndClientEmail(id, email);
        }

        public IEnumerable<Account> GetAccountsByClient(long idClient)
        {
            var accountsList = _accountRepository.GetAccountsByClient(idClient);
            if (accountsList == null)
            {
                throw new Exception("La lista que retorna GetAccountsByClient es null");
            }

            return accountsList;
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }

        public int GetCountAccountsByClient(long idClient)
        {
            int accounts = _accountRepository.GetCountAccountsByClient(idClient);
            if (accounts >= 3)
            {
                throw new Exception("A superado el maximo de cuentas propias permitidas");
            }
            return accounts;
        }

        public void SaveAccount(Account account)
        {
            _accountRepository.Save(account);
        }
    }
}
