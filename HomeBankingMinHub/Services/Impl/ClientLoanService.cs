using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services.Impl
{
    public class ClientLoanService : IClientLoanService
    {
        private readonly IClientLoanRepository _clientLoanRepository;
        public ClientLoanService(IClientLoanRepository clientLoanRepository)
        {
            _clientLoanRepository = clientLoanRepository;
        }

        public ClientLoan CreateClientLoan(double amount, long clientId, long loanId, string payment)
        {
            return new ClientLoan
            {
                Amount = amount * 1.2,
                ClientId = clientId,
                LoanId = loanId,
                Payments = payment
            };
        }

        public void SaveClientLoan(ClientLoan clientLoan)
        {
            _clientLoanRepository.Save(clientLoan);
        }
    }
}
