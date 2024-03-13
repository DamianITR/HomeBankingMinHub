using HomeBankingMindHub.Models;

namespace HomeBankingMindHub.Services
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAll();
        Loan FindLoanById(long id);
        List<string> GetAllPaymentsLoan(long id);
    }
}
