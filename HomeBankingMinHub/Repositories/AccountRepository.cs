﻿using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public Account FindByIdAndClientEmail(long id, string email)
        {
            return FindByCondition(account => account.Id == id && account.Client.Email.Equals(email))
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
                .Include(account => account.Transactions)
                .ToList();
        }

        public int GetCountAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
                .Count();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }


        public void Save(Account account)
        {
            Create(account);
            SaveChanges();
        }

        public bool ExistNumberAccount(string numberAccount)
        {
            return FindByCondition(account => account.Number.Equals(numberAccount))
                .Any();
        }
    }
}
