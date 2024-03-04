using HomeBankingMindHub.Models;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;

namespace HomeBankingMindHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll()
                .ToList();
        }

        public string[] GetAllPaymentsLoan(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .Select(loan => loan.Payments)
                .ToArray();
        }
    }
}
