using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTOs;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
        public ClientsController(IClientRepository clientRepository, IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();

                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList(),
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color.ToString(),
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type.ToString()
                        }).ToList()
                    };

                    clientsDTO.Add(newClientDTO);
                }

                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString()
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current")]
        [Authorize(Policy = "ClientOnly")]
        public IActionResult GetCurrent()
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

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color.ToString(),
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type.ToString(),
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                //validamos datos antes
                // Email
                if (String.IsNullOrEmpty(client.Email))
                {
                    return StatusCode(403, "El correo electrónico no puede estar vacío.");
                }

                // Contraseña
                if (String.IsNullOrEmpty(client.Password))
                {
                    return StatusCode(403, "La contraseña no puede estar vacía.");
                }

                // Nombre
                if (String.IsNullOrEmpty(client.FirstName))
                {
                    return StatusCode(403, "El nombre no puede estar vacío.");
                }

                // Apellido
                if (String.IsNullOrEmpty(client.LastName))
                {
                    return StatusCode(403, "El apellido no puede estar vacío.");
                }

                //buscamos si ya existe el usuario
                //Client user = _clientRepository.FindByEmail(client.Email);

                if (_clientRepository.ExistByEmail(client.Email))
                {
                    return StatusCode(403, "Email está en uso");
                }

                //encripto la password
                String clientPasswordHashed = Encryptor.EncryptPassword(client.Password);

                //se crea el cliente nuevo
                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = clientPasswordHashed,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                //guardo el cliente nuevo en el contexto
                _clientRepository.Save(newClient);

                //busco el id del cliente recien creado para obtener el id con el que se guardo porque lo tengo que asociar al account
                long? idNewClient = _clientRepository.GetIdClientFromEmail(newClient.Email);

                if (idNewClient != null)
                {
                    //al cliente nuevo recien creado automaticamente le agrego una cuenta
                    string newNumberAccount;

                    do
                    {
                        newNumberAccount = GeneratorNumbers.CreateNewNumberAccount();
                    }
                    while (_accountRepository.ExistNumberAccount(newNumberAccount));

                    var newAccount = new Account
                    {
                        Number = newNumberAccount,
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        ClientId = (long)idNewClient,
                    };

                    //guardo la cuenta en el contexto
                    _accountRepository.Save(newAccount);

                    return Created("", newClient);
                }
                else
                {
                    return Forbid();
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/accounts")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult CreateAccount()
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

                var totalAccounts = _accountRepository.GetCountAccountsByClient(client.Id);

                if (totalAccounts >= 3)
                {
                    return Forbid();
                }
                else
                {
                    // creo un numero de cuenta que no exista en la DB
                    string newNumberAccount;
                    do
                    {
                        newNumberAccount = GeneratorNumbers.CreateNewNumberAccount();
                    }
                    while (_accountRepository.ExistNumberAccount(newNumberAccount));

                    var newAccount = new Account
                    {
                        Number = newNumberAccount,
                        CreationDate = DateTime.Now,
                        Balance = 0,
                        ClientId = client.Id,
                    };

                    _accountRepository.Save(newAccount);
                    return Created();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("current/accounts")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult GetCurrentAccounts()
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

                var accountsClient = _accountRepository.GetAccountsByClient(client.Id);

                if (accountsClient == null)
                {
                    return Forbid();
                }
                else
                {
                    var accountsDTO = new List<AccountDTO>();

                    foreach (Account account in accountsClient)
                    {
                        var newAccountDTO = new AccountDTO
                        {
                            Id = account.Id,
                            Number = account.Number,
                            CreationDate = account.CreationDate,
                            Balance = account.Balance,
                            Transactions = account.Transactions.Select(transaction => new TransactionDTO
                            {
                                Id = transaction.Id,
                                Type = transaction.Type.ToString(),
                                Amount = transaction.Amount,
                                Description = transaction.Description,
                                Date = transaction.Date
                            }).ToList()
                        };

                        accountsDTO.Add(newAccountDTO);
                    }

                    return Ok(accountsDTO);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("current/cards")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult CreateCards([FromBody] SimpleCardDTO simplifiedCardDTO)
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

                if (_cardRepository.GetCountCardsByClient(client.Id) >= 6)
                {
                    return Forbid();
                }

                CardType cardType = (CardType)Enum.Parse(typeof(CardType), simplifiedCardDTO.Type);
                CardColor cardColor = (CardColor)Enum.Parse(typeof(CardColor), simplifiedCardDTO.Color);


                if (_cardRepository.ExistSpecificCard(client.Id, cardType, cardColor))
                {
                    return StatusCode(403, "Solamente podes tener una tarjeta de cada tipo");
                }
                else
                {
                    // creo un numero de tarjeta que no exista en la DB
                    string newNumberCard;
                    do
                    {
                        newNumberCard = GeneratorNumbers.CreateNewNumberCard();
                    }
                    while (_cardRepository.ExistNumberCard(newNumberCard));

                    var newCard = new Card
                    {
                        CardHolder = client.FirstName + " " + client.LastName,
                        Type = cardType,
                        Color = cardColor,
                        Number = newNumberCard,
                        Cvv = GeneratorNumbers.CreateNewNumberCvv(),
                        FromDate = DateTime.Now,
                        ThruDate = DateTime.Now.AddYears(5),
                        ClientId = client.Id,
                    };

                    _cardRepository.Save(newCard);
                    return Created();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("current/cards")]
        [Authorize(Policy = "ClientOnly")]

        public IActionResult GetCurrentCards()
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

                var cardsClient = _cardRepository.GetCardsByClient(client.Id);

                if (cardsClient == null)
                {
                    return Forbid();
                }
                else
                {
                    var cardsDTOList = new List<CardDTO>();

                    foreach (Card card in cardsClient)
                    {
                        var newCardDTO = new CardDTO
                        {
                            Id = card.Id,
                            CardHolder = client.FirstName + " " + client.LastName,
                            Type = card.Type.ToString(),
                            Color = card.Color.ToString(),
                            Number = card.Number,
                            Cvv = card.Cvv,
                            FromDate = card.FromDate,
                            ThruDate = card.ThruDate
                        };

                        cardsDTOList.Add(newCardDTO);
                    }

                    return Ok(cardsDTOList);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
