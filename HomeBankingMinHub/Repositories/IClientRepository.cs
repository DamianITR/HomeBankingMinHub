﻿using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAllClients();
        void Save(Client client);
        Client FindById(long id);
        Client FindByEmail(string email);

        Boolean ExistByEmail(string email);
    }
}
