using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTOs;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public Client CreateClient(Client client)
        {
            //encripto la password
            String clientPasswordHashed = Encryptor.EncryptPassword(client.Password);

            Client newClient = new Client
            {
                Email = client.Email,
                Password = clientPasswordHashed,
                FirstName = client.FirstName,
                LastName = client.LastName,
            };

            return newClient;
        }

        public bool ExistClientByEmail(string email)
        {
            return _clientRepository.ExistByEmail(email);
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _clientRepository.GetAllClients();
        }

        public IEnumerable<ClientDTO> GetAllClientsDTO()
        {
            var clientsList = _clientRepository.GetAllClients();
            var clientsDTO = new List<ClientDTO>();

            foreach (Client client in clientsList)
            {
                var newClientDTO = new ClientDTO(client);
                clientsDTO.Add(newClientDTO);
            }

            return clientsDTO;
        }

        public Client GetClientByEmail(string email)
        {
            return _clientRepository.FindByEmail(email);
        }

        public Client GetClientById(long id)
        {
            return _clientRepository.FindById(id);
        }

        public ClientDTO GetClientDTOByEmail(string email)
        {
            Client client = GetClientByEmail(email);
            if (client == null)
            {
                throw new Exception("No se encontro al cliente");
            }

            return new ClientDTO(client);
        }

        public ClientDTO GetClientDTOById(long id)
        {
            Client client = GetClientById(id);
            if (client == null)
            {
                throw new Exception("No se encontro al cliente");
            }

            return new ClientDTO(client);
        }

        public long? GetIdNewClientFromEmail(string clientEmail)
        {
            long? id = _clientRepository.GetIdClientFromEmail(clientEmail);
            if (id == null)
            {
                throw new Exception($"No existe cliente con mail: {clientEmail}");
            }
            return id;
        }

        public void SaveClient(Client client)
        {
            _clientRepository.Save(client);
        }
    }
}
