using HomeBankingMindHub.Models;
using HomeBankingMindHub.Models.Emuns;

namespace HomeBankingMinHub.Models
{
    public class DBInitializer
    {

        public static void Initialize(HomeBankingContext context)
        {
            //CLIENTES
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "martin@gmail.com", FirstName="Martin", LastName="Lopez", Password="123456"},
                    new Client { Email = "juan@gmail.com", FirstName="Juan", LastName="Martinez", Password="qwerty"},
                    new Client { Email = "pedro@gmail.com", FirstName="Pedro", LastName="Perez", Password="asdfgh"},
                    new Client { Email = "victor@gmail.com", FirstName="Victor", LastName="Gonzales", Password="zxcvbn"},
                };

                context.Clients.AddRange(clients);

                //guardamos
                context.SaveChanges();
            }

            //CUENTAS
            if (!context.Account.Any())
            {
                var accountVictor = context.Clients.FirstOrDefault(c => c.Email == "victor@gmail.com");
                if (accountVictor != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 0 },
                        new Account {ClientId = accountVictor.Id, CreationDate = DateTime.Today.AddDays(1), Number = "VIN002", Balance = 100 },

                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.Add(account);
                    }
                    context.SaveChanges();

                }

                var accountMartin = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (accountMartin != null)
                {
                    var accounts = new Account[]
                    {
                        new Account {ClientId = accountMartin.Id, CreationDate = DateTime.Today, Number = "VIN003", Balance = 200 }
                    };
                    foreach (Account account in accounts)
                    {
                        context.Account.Add(account);
                    }
                    context.SaveChanges();

                }
            }

            //TRANSACCIONES
            if (!context.Transactions.Any())
            {
                var account1 = context.Account.FirstOrDefault(c => c.Number == "VIN001");

                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda mercado libre", Type = TransactionType.DEBIT },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Compra en tienda xxxx", Type = TransactionType.DEBIT },
                    };

                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }

                var account2 = context.Account.FirstOrDefault(c => c.Number == "VIN002");

                if (account2 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account2.Id, Amount = 5000, Date= DateTime.Now.AddHours(-5), Description = "Transferencia reccibida", Type = TransactionType.CREDIT },
                        new Transaction { AccountId= account2.Id, Amount = -1000, Date= DateTime.Now.AddHours(-6), Description = "Compra en tienda Supermercado Pepito", Type = TransactionType.DEBIT },
                    };

                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
            }

            //PRESTAMOS
            if (!context.Loans.Any())
            {
                //crearemos 3 prestamos Hipotecario, Personal y Automotriz
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                foreach (Loan loan in loans)
                {
                    context.Loans.Add(loan);
                }

                context.SaveChanges();

                //ahora agregaremos los clientloan (Prestamos del cliente)
                //usaremos al único cliente que tenemos y le agregaremos un préstamo de cada item
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (client1 != null)
                {
                    //ahora usaremos los 3 tipos de prestamos
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }

                    //guardamos todos los prestamos
                    context.SaveChanges();
                }

                //OTRO CLIENTE
                var client2 = context.Clients.FirstOrDefault(c => c.Email == "victor@gmail.com");
                if (client2 != null)
                {
                    //ahora usaremos los 3 tipos de prestamos
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 111000,
                            ClientId = client2.Id,
                            LoanId = loan1.Id,
                            Payments = "48"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 255000,
                            ClientId = client2.Id,
                            LoanId = loan2.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }

                    //guardamos todos los prestamos
                    context.SaveChanges();
                }
            }

            //TARJETAS
            if (!context.Cards.Any())
            {
                //buscamos al cliente
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "martin@gmail.com");
                if (client1 != null)
                {
                    //le agregamos 2 tarjetas de crédito una GOLD y una TITANIUM, de tipo DEBITO Y CREDITO RESPECTIVAMENTE
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.GOLD,
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT,
                            Color = CardColor.TITANIUM,
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.GOLD,
                            Number = "1234-5678-9012-3456",
                            Cvv = 997,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(1),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT,
                            Color = CardColor.TITANIUM,
                            Number = "7878-8888-777-4949",
                            Cvv = 998,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(1),
                        }
                    };

                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }

                //buscamos al cliente
                var client2 = context.Clients.FirstOrDefault(c => c.Email == "victor@gmail.com");
                if (client2 != null)
                {
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client2.Id,
                            CardHolder = client2.FirstName + " " + client2.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.GOLD,
                            Number = "1111-1111-1111-1111",
                            Cvv = 123,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(2),
                        },
                        new Card {
                            ClientId= client2.Id,
                            CardHolder = client2.FirstName + " " + client2.LastName,
                            Type = CardType.CREDIT,
                            Color = CardColor.SILVER,
                            Number = "2222-2222-222-2222",
                            Cvv = 321,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(1),
                        },
                        new Card {
                            ClientId= client2.Id,
                            CardHolder = client2.FirstName + " " + client2.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.SILVER,
                            Number = "3332-2333-333-2333",
                            Cvv = 456,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(7),
                        },
                        new Card {
                            ClientId= client2.Id,
                            CardHolder = client2.FirstName + " " + client2.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.SILVER,
                            Number = "4444-4444-444-444",
                            Cvv = 790,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(2),
                        }
                    };

                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }
            }



        }
    }
}
