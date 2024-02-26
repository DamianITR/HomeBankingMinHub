using HomeBankingMindHub.Models.Emuns;

namespace HomeBankingMinHub.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public TransactionType Type { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public Account Account { get; set; }

        public long AccountId { get; set; }
    }
}
