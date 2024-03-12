using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTOs;

namespace HomeBankingMindHub.Services
{
    public interface IClientService
    {
        Client GetClientByEmail(string email);
        Client GetClientById(long id);
        ClientDTO GetClientDTOByEmail(string email);
        ClientDTO GetClientDTOById(long id);
        IEnumerable<Client> GetAllClients();
        IEnumerable<ClientDTO> GetAllClientsDTO();
        bool ExistClientByEmail(string email);
        Client CreateClient(Client client);
        void SaveClient(Client client);
        long? GetIdNewClientFromEmail(string clientEmail);
    }
}
