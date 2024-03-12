using HomeBankingMindHub.Models.DTOs;
using System.Text.Json.Serialization;

namespace HomeBankingMinHub.Models.DTOs
{
    public class ClientDTO
    {
        [JsonIgnore]

        public long Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public ICollection<AccountDTO> Accounts { get; set; }

        public ICollection<ClientLoanDTO> Credits { get; set; }

        public ICollection<CardDTO> Cards { get; set; }

        public ClientDTO()
        {
        }

        public ClientDTO(Client client)
        {
            Id = client.Id;
            FirstName = client.FirstName;
            LastName = client.LastName;
            Email = client.Email;
            Accounts = client.Accounts.Select(account => new AccountDTO(account)).ToList();
            Credits = client.ClientLoans.Select(clientLoan => new ClientLoanDTO(clientLoan)).ToList();
            Cards = client.Cards.Select(card => new CardDTO(card)).ToList();
        }
    }
}
