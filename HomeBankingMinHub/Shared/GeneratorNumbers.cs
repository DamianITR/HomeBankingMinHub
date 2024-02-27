using System.Security.Cryptography;

namespace HomeBankingMindHub.Shared
{
    public static class GeneratorNumbers
    {
        public static int Generate(int lowerBound, int upperBound)
        {
            return RandomNumberGenerator.GetInt32(lowerBound, upperBound);
        }
    }
}
