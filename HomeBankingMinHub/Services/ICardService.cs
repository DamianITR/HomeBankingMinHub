using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ICardService
    {
        int GetCountCardsByClient(long clientId);
        bool ExistSpecificCard(long clientId, CardType cardType, CardColor cardColor);
        string CreateNumberCard();
        Card CreateCard(Client client, CardType cardType, CardColor cardColor);
        void SaveCard(Card card);
        IEnumerable<Card> GetCardsByClient(long clientId);
    }
}
