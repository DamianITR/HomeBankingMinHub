namespace HomeBankingMinHub.Models
{
    public class DBInitializer
    {

        public static void Initialize(HomeBankingContext context)
        {
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

        }
    }
}
