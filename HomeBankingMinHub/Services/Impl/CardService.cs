using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services.Impl
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        public CardService(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public Card CreateCard(Client client, CardType cardType, CardColor cardColor)
        {
            string newNumberCard = CreateNumberCard();
            return new Card
            {
                CardHolder = client.FirstName + " " + client.LastName,
                Type = cardType,
                Color = cardColor,
                Number = newNumberCard,
                Cvv = GeneratorNumbers.CreateNewNumberCvv(),
                FromDate = DateTime.Now,
                ThruDate = DateTime.Now.AddYears(5),
                ClientId = client.Id,
            };
        }

        public string CreateNumberCard()
        {
            string newNumberCard;
            do
            {
                newNumberCard = GeneratorNumbers.CreateNewNumberCard();
            }
            while (_cardRepository.ExistNumberCard(newNumberCard));

            return newNumberCard;
        }

        public bool ExistSpecificCard(long clientId, CardType cardType, CardColor cardColor)
        {
            return _cardRepository.ExistSpecificCard(clientId, cardType, cardColor);
        }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return _cardRepository.GetCardsByClient(clientId);
        }

        public int GetCountCardsByClient(long clientId)
        {
            return _cardRepository.GetCountCardsByClient(clientId);
        }

        public void SaveCard(Card card)
        {
            _cardRepository.Save(card);
        }
    }
}
