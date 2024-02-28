using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;

namespace HomeBankingMindHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public bool ExistNumberCard(string numberCard)
        {
            return FindByCondition(card => card.Number.Equals(numberCard))
                .Any();
        }

        public bool ExistSpecificCard(long clientId, CardType typeCard, CardColor colorCard)
        {
            return FindByCondition(card => card.ClientId == clientId && card.Type.Equals(typeCard) && card.Color.Equals(colorCard))
                .Any();
        }

        public IEnumerable<Card> GetCardsByClient(long clientId)
        {
            return FindByCondition(card => card.ClientId == clientId)
                .ToList();
        }

        public int GetCountCardsByClient(long clientId)
        {
            return FindByCondition(card => card.ClientId == clientId)
                .Count();
        }

        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
