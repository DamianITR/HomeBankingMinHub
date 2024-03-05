using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMinHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Transactions;
using Transaction = HomeBankingMinHub.Models.Transaction;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ILoanRepository _loanRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository,
            IAccountRepository accountRepository,
            ILoanRepository loanRepository,
            IClientLoanRepository clientLoanRepository,
            ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();
                var newLoansDTOList = new List<LoanDTO>();
                {
                    foreach (var loan in loans)
                    {
                        var newloanDTO = new LoanDTO()
                        {
                            Id = loan.Id,
                            Name = loan.Name,
                            MaxAmount = loan.MaxAmount,
                            Payments = loan.Payments,
                        };
                        newLoansDTOList.Add(newloanDTO);
                    }
                };

                return Ok(newLoansDTOList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateClientLoan(LoanApplicationDTO loanApplicationDTO)
        {
            using (var scope = new TransactionScope())
            {
                try
                {
                    string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                    if (email == string.Empty)
                    {
                        return Forbid();
                    }

                    Client client = _clientRepository.FindByEmail(email);
                    if (client == null)
                    {
                        return Forbid();
                    }

                    if (loanApplicationDTO == null)
                    {
                        return StatusCode(403, "El loanApplicationDTO no puede ser null ");
                    }

                    if (loanApplicationDTO.LoanId <= 0)
                    {
                        return StatusCode(403, "El loanId no puede ser menor o igual a cero ");
                    }

                    if (loanApplicationDTO.ToAccountNumber.IsNullOrEmpty())
                    {
                        return StatusCode(403, "El ToAccountNumber no puede ser null o vacio");
                    }

                    Loan loan = _loanRepository.FindById(loanApplicationDTO.LoanId);
                    if (loan == null)
                    {
                        return StatusCode(403, "El Loan no fue encontrado");
                    }

                    if (loanApplicationDTO.Amount > loan.MaxAmount || loanApplicationDTO.Amount <= 0)
                    {
                        return StatusCode(403, "El Amount no pueder ser menor igual a cero o superar el maximo del prestamo");
                    }

                    //me traigo la lista de payments que tiene el loan especifico que estoy consultando
                    var listPayments = _loanRepository.GetAllPaymentsLoan(loanApplicationDTO.LoanId);
                    List<int> paymentsAllowed = new List<int>();
                    if (listPayments is string && listPayments != null)
                    {
                        string[] numbers = listPayments.Split(',');
                        foreach (string number in numbers)
                        {
                            paymentsAllowed.Add(int.Parse(number));
                        }
                    }
                    if (loanApplicationDTO.Payments.IsNullOrEmpty() || int.Parse(loanApplicationDTO.Payments) == 0 || !paymentsAllowed.Contains(int.Parse(loanApplicationDTO.Payments)))
                    {
                        return StatusCode(403, "Las cuotas no pueden ser null, cero o tener otro valor distinto a los del prestamo");
                    }

                    Account account = _accountRepository.FindByNumber(loanApplicationDTO.ToAccountNumber);
                    if (account == null)
                    {
                        return StatusCode(403, "La cuenta destino no existe");
                    }

                    if (_accountRepository.ClientHaveAccount(client.Id, email))
                    {
                        return StatusCode(403, "Al usuario logueado no le pertenece esa cuenta");
                    }

                    var newClientLoan = new ClientLoan
                    {
                        Amount = loanApplicationDTO.Amount * 1.2,
                        ClientId = client.Id,
                        LoanId = loan.Id,
                        Payments = loanApplicationDTO.Payments
                    };

                    _clientLoanRepository.Save(newClientLoan);

                    var newTransacciont = new Transaction
                    {
                        AccountId = account.Id,
                        Amount = loanApplicationDTO.Amount,
                        Date = DateTime.Now,
                        Description = loan.Name + " loan approved",
                        Type = TransactionType.CREDIT
                    };

                    _transactionRepository.Save(newTransacciont);

                    account.Balance += loanApplicationDTO.Amount;
                    _accountRepository.Save(account);

                    scope.Complete();
                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
        }


    }
}
