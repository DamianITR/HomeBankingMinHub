using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface IClientLoanService
    {
        void SaveClientLoan(ClientLoan clientLoan);
        ClientLoan CreateClientLoan(double amount, long clientId, long loanId, string payment);
    }
}
