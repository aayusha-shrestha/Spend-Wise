using System.Security.Cryptography;
using SpendWise.Model;

namespace SpendWise.Utilities
{
    public static class Utils
    {
        private const char _segmentDelimiter = ':';

        public static string GenerateHash(string input)
        {
            var saltSize = 16;
            var iterations = 100_000;
            var keySize = 32;
            HashAlgorithmName algorithm = HashAlgorithmName.SHA256;
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

            return string.Join(
                _segmentDelimiter,
                Convert.ToHexString(hash),
                Convert.ToHexString(salt),
                iterations,
                algorithm
            );
        }

        public static bool VerifyHash(string input, string hashString)
        {
            string[] segments = hashString.Split(_segmentDelimiter);
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            HashAlgorithmName algorithm = new(segments[3]);
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations,
                algorithm,
                hash.Length
            );

            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }

        // Handle currency formatting
        public static string GetFormattedAmount(decimal amount, Currency currency)
        {
            switch (currency)
            {
                case Currency.NPR:
                    return $"Rs {amount}";
                case Currency.USD:
                    return $"${amount}";
                default:
                    return $"{amount}";
            }
        }

        // File path where the users.json file is stored in the app's local data directory
        public static string GetAppUsersFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, "users.json");
        }

        // File path where the userId_transactions.json file is stored in the app's local data directory
        public static string GetTransactionsFilePath(Guid userId)
        {
            return  Path.Combine(FileSystem.AppDataDirectory, userId.ToString() + "_transactions.json");
        }

        // File path where the userId_debts.json file is stored in the app's local data directory
        public static string GetDebtsFilePath(Guid userId)
        {
            return Path.Combine(FileSystem.AppDataDirectory, userId.ToString() + "_debts.json");
        }

        // File path where the userId_tags.json file is stored in the app's local data directory
        public static string GetTagsFilePath(Guid userId)
        {
            return Path.Combine(FileSystem.AppDataDirectory, userId.ToString() + "_tags.json");
        }
    }
}
