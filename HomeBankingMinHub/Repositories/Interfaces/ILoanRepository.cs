using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAll();
        Loan FindById(long id);
    }
}
