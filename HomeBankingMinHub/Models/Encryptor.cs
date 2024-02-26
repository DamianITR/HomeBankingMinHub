using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace HomeBankingMindHub.Models
{
    public static class Encryptor
    {
        public static string EncryptPassword(string password)
        {
            string blackBox = "laClaveSecreta";
            byte[] salt = Encoding.UTF8.GetBytes(blackBox);
            string clientPasswordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
            return clientPasswordHashed;
        }

    }
}
