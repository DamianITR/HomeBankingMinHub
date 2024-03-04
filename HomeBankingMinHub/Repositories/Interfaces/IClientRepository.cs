using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface IClientRepository
    {
        IEnumerable<Client> GetAllClients();
        void Save(Client client);
        Client FindById(long id);
        Client FindByEmail(string email);
        bool ExistByEmail(string email);
        long? GetIdClientFromEmail(string email);
    }
}
