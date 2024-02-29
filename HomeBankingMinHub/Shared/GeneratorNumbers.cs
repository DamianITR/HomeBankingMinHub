using System.Security.Cryptography;

namespace HomeBankingMindHub.Shared
{
    public static class GeneratorNumbers
    {
        public static string CreateNewNumberAccount()
        {
            int lowerBound = 0; //numero incluido
            int upperBound = 100000000; //numero excliudo, maximo 8 digitos para este proyecto
            return "VIN-" + RandomNumberGenerator.GetInt32(lowerBound, upperBound).ToString("D8");
        }

        public static string CreateNewNumberCard()
        {
            int lowerBound = 0;
            int upperBound = 10000;
            return RandomNumberGenerator.GetInt32(lowerBound, upperBound).ToString("D4")
                    + "-" + RandomNumberGenerator.GetInt32(lowerBound, upperBound).ToString("D4")
                    + "-" + RandomNumberGenerator.GetInt32(lowerBound, upperBound).ToString("D4")
                    + "-" + RandomNumberGenerator.GetInt32(lowerBound, upperBound).ToString("D4");
        }

        public static int CreateNewNumberCvv()
        {
            return int.Parse(RandomNumberGenerator.GetInt32(0, 1000).ToString("D3"));
        }
    }
}
