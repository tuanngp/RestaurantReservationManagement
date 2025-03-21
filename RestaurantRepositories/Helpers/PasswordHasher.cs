using System.Security.Cryptography;

namespace Repositories.Helpers
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 20;
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = GetPbkdf2Bytes(password, salt, Iterations, HashSize);

            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            byte[] hash = GetPbkdf2Bytes(password, salt, Iterations, HashSize);

            for (var i = 0; i < HashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }

        private static byte[] GetPbkdf2Bytes(
            string password,
            byte[] salt,
            int iterations,
            int outputBytes
        )
        {
            using (
                var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256
                )
            )
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
