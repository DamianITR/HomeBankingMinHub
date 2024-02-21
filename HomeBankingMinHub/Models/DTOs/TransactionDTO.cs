namespace HomeBankingMinHub.Models.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }

        public String Type { get; set; }

        public double Amount { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
