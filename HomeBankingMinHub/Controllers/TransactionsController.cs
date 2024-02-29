using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        [HttpPost()]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult CreateTransaction([FromBody] TransferDTO transferDTO)
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

                //chequeo datos del DTO
                if (transferDTO == null)
                {
                    return Forbid();
                }

                if (transferDTO.FromAccountNumber.IsNullOrEmpty())
                {
                    return StatusCode(403, "Se necesita la cuenta de origen");
                }

                if (transferDTO.ToAccountNumber.IsNullOrEmpty())
                {
                    return StatusCode(403, "Se necesita la cuenta de destino");
                }

                if (transferDTO.FromAccountNumber.Equals(transferDTO.ToAccountNumber))
                {
                    return StatusCode(403, "La cuenta origen no puede ser igual a la cuenta destino");
                }

                if (transferDTO.Amount <= 0)
                {
                    return StatusCode(403, "El monto no puede ser cero o menor a cero");
                }

                if (transferDTO.Description.IsNullOrEmpty())
                {
                    return StatusCode(403, "La descripcion no puede estar vacia o null");
                }

                //Verifico que exista la cuenta de origen
                Account originAccount = _accountRepository.FindByNumber(transferDTO.FromAccountNumber);
                if (originAccount == null)
                {
                    return StatusCode(403, "No existe la cuenta de origen");
                }

                //Verifico que la cuenta de origen pertenezca al cliente autenticado
                if (!_accountRepository.ClientHaveAccount(originAccount.Id, email))
                {
                    return StatusCode(403, "La cuenta no pertenece a un cliente autenticado");
                }

                //Verifico que exista la cuenta de destino
                Account destinationAccount = _accountRepository.FindByNumber(transferDTO.ToAccountNumber);
                if (destinationAccount == null)
                {
                    return StatusCode(403, "No existe la cuenta destino");
                }

                //Verifico que la cuenta de origen tenga el monto disponible.
                if (transferDTO.Amount > originAccount.Balance)
                {
                    return StatusCode(403, "La cuenta de origen no dispone de fondos suficientes para realizar dicha operacion");
                }

                //creo las dos transacciones
                var transactions = new Transaction[]
                {
                        new Transaction {   AccountId= originAccount.Id,
                                            Amount = - transferDTO.Amount,
                                            Date= DateTime.Now, Description = transferDTO.Description,
                                            Type = TransactionType.DEBIT
                                        },
                        new Transaction {   AccountId= destinationAccount.Id,
                                            Amount = transferDTO.Amount,
                                            Date= DateTime.Now,
                                            Description = transferDTO.Description,
                                            Type = TransactionType.CREDIT
                                        },
                };

                //guardo transacciones
                foreach (var transaction in transactions)
                {
                    _transactionRepository.Save(transaction);

                }

                //seteo los nuevos balances de cada cuenta
                originAccount.Balance -= transferDTO.Amount;
                destinationAccount.Balance += transferDTO.Amount;

                //actualizo base de datos
                _accountRepository.Save(originAccount);
                _accountRepository.Save(destinationAccount);

                return Created();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}