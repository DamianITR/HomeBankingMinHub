using HomeBankingMindHub.Models.DTOs;
using HomeBankingMinHub.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace HomeBankingMindHub.Models
{
    public static class Encryptor
    {
        public static string EncryptPassword(Client client)
        {
            string blackBox = "laClaveSecreta";
            byte[] salt = Encoding.UTF8.GetBytes(blackBox);
            string clientPasswordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: client.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
            return clientPasswordHashed;
        }

        public static string EncryptPassword(ClientLoginDTO client)
        {
            string blackBox = "laClaveSecreta";
            byte[] salt = Encoding.UTF8.GetBytes(blackBox);
            string clientPasswordHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: client.Password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));
            return clientPasswordHashed;
        }
    }
}
