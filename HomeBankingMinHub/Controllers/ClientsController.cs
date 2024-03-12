using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.DTOs;
using HomeBankingMindHub.Models.Emuns;
using HomeBankingMindHub.Repositories.Interfaces;
using HomeBankingMindHub.Services;
using HomeBankingMindHub.Shared;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ICardRepository _cardRepository;
        private IClientService _clientService;
        private IAccountService _accountService;
        public ClientsController
            (IClientRepository clientRepository,
            IAccountRepository accountRepository,
            ICardRepository cardRepository,
            IClientService clientService,
            IAccountService accountService)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
            _clientService = clientService;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_clientService.GetAllClientsDTO());
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
                return Ok(_clientService.GetClientDTOById(id));
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

                return Ok(_clientService.GetClientDTOByEmail(email));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            using (var scope = new TransactionScope())
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

                    if (_clientService.ExistClientByEmail(client.Email))
                    {
                        return StatusCode(403, "Email está en uso");
                    }

                    //se crea el cliente nuevo
                    var newClient = _clientService.CreateClient(client);

                    //guardo el cliente nuevo en el contexto
                    _clientService.SaveClient(newClient);

                    //busco el id del cliente recien creado para obtener el id con el que se guardo porque lo tengo que asociar al account
                    long? idNewClient = _clientService.GetIdNewClientFromEmail(newClient.Email);

                    //al cliente nuevo recien creado automaticamente le agrego una cuenta
                    string newNumberAccount = _accountService.CreateNewNumberAccount();
                    Account newAccount = _accountService.CreateAccountWithIdClientAndAccountNumber(idNewClient, newNumberAccount);

                    //guardo la cuenta en el contexto
                    _accountService.SaveAccount(newAccount);

                    scope.Complete();

                    return Created("Cliente creado", newClient);

                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
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

                Client client = _clientService.GetClientByEmail(email);

                //si tiene mas de 3 cuantas lanzo una exception
                _accountService.GetCountAccountsByClient(client.Id);

                // creo un numero de cuenta que no exista en la DB
                string newNumberAccount = _accountService.CreateNewNumberAccount();

                Account newAccount = _accountService.CreateAccountWithIdClientAndAccountNumber(client.Id, newNumberAccount);

                _accountService.SaveAccount(newAccount);

                return Created("Cuenta creada", newAccount);

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

                Client client = _clientService.GetClientByEmail(email);

                IEnumerable<Account> accountsClient = _accountService.GetAccountsByClient(client.Id);

                //transformo los accounts a accountsDTO
                List<AccountDTO> accountsDTO = new List<AccountDTO>();

                foreach (Account account in accountsClient)
                {
                    AccountDTO newAccountDTO = new AccountDTO(account);

                    accountsDTO.Add(newAccountDTO);
                }

                return Ok(accountsDTO);
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

                Client client = _clientService.GetClientByEmail(email);

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
