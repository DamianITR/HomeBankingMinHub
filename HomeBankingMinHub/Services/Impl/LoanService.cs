using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;

namespace HomeBankingMindHub.Services.Impl
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public Loan FindLoanById(long id)
        {
            return _loanRepository.FindById(id);
        }

        public IEnumerable<Loan> GetAll()
        {
            return _loanRepository.GetAll();
        }

        public List<string> GetAllPaymentsLoan(long id)
        {
            string listPayments = _loanRepository.GetAllPaymentsLoan(id);
            List<string> paymentsAllowed = new List<string>();
            if (listPayments is string && listPayments != null)
            {
                string[] numbers = listPayments.Split(',');
                foreach (string number in numbers)
                {
                    paymentsAllowed.Add(number);
                }
            }
            else
            {
                throw new Exception("se produjo un erro con la lista de payments del loan");
            }
            return paymentsAllowed;
        }
    }
}
