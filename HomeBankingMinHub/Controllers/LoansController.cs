using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Services;
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
        private ILoanService _loanService;
        private IClientService _clientService;
        private IAccountService _accountService;
        private IClientLoanService _clientLoanService;
        private ITransactionService _transactionService;

        public LoansController
            (
            ILoanService loanService,
            IClientService clientService,
            IAccountService accountService,
            IClientLoanService clientLoanService,
            ITransactionService transactionService
            )
        {
            _loanService = loanService;
            _clientService = clientService;
            _accountService = accountService;
            _clientLoanService = clientLoanService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanService.GetAll();
                var newLoansDTOList = new List<LoanDTO>();
                {
                    foreach (var loan in loans)
                    {
                        var newloanDTO = new LoanDTO(loan);
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

                    Client client = _clientService.GetClientByEmail(email);
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

                    Loan loan = _loanService.FindLoanById(loanApplicationDTO.LoanId);
                    if (loan == null)
                    {
                        return StatusCode(403, "El Loan no fue encontrado");
                    }

                    if (loanApplicationDTO.Amount > loan.MaxAmount || loanApplicationDTO.Amount <= 0)
                    {
                        return StatusCode(403, "El Amount no pueder ser menor igual a cero o superar el maximo del prestamo");
                    }

                    //me traigo la lista de payments que tiene el loan especifico que estoy consultando
                    List<string> listPayments = _loanService.GetAllPaymentsLoan(loanApplicationDTO.LoanId);

                    if (loanApplicationDTO.Payments.IsNullOrEmpty() || int.Parse(loanApplicationDTO.Payments) == 0 || !listPayments.Contains(loanApplicationDTO.Payments))
                    {
                        return StatusCode(403, "Las cuotas no pueden ser null, cero o tener otro valor distinto a los del prestamo");
                    }

                    Account account = _accountService.FindByNumber(loanApplicationDTO.ToAccountNumber);
                    if (account == null)
                    {
                        return StatusCode(403, "La cuenta destino no existe");
                    }

                    if (_accountService.ClientHaveAccount(client.Id, email))
                    {
                        return StatusCode(403, "Al usuario logueado no le pertenece esa cuenta");
                    }

                    ClientLoan newClientLoan = _clientLoanService.CreateClientLoan(loanApplicationDTO.Amount, client.Id, loan.Id, loanApplicationDTO.Payments);

                    _clientLoanService.SaveClientLoan(newClientLoan);

                    Transaction newTransacciont = _transactionService.CreateTransaction(account.Id, loanApplicationDTO.Amount, loan.Name);

                    _transactionService.SaveTransaction(newTransacciont);

                    account.Balance += loanApplicationDTO.Amount;
                    _accountService.SaveAccount(account);

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