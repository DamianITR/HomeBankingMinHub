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

        }
    }
}
