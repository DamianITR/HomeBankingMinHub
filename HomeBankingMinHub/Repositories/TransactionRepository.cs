﻿using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;

namespace HomeBankingMindHub.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}