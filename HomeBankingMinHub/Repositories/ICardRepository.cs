using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMinHub.Models;

namespace HomeBankingMindHub.Repositories
{
    public interface ICardRepository
    {
        void Save(Card card);
        bool ExistNumberCard(string numberCard);
        bool ExistSpecificCard(long clientId, CardType typeCard, CardColor colorCard);
        IEnumerable<Card> GetCardsByClient(long clientId);
    }
}
