using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTOs;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
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
                throw new Exception();
            }

            return new ClientDTO(client);
        }

        public ClientDTO GetClientDTOById(long id)
        {
            Client client = GetClientById(id);
            if (client == null)
            {
                throw new Exception();
            }

            return new ClientDTO(client);
        }
    }
}
