using EmployeeServiceDesk.Application.ServiceInterface;
using System.Security.Cryptography;

namespace EmployeeServiceDesk.Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 600_000;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Password cannot be empty.",
                    nameof(password));
            }

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Iterations}." +
                   $"{Convert.ToBase64String(salt)}." +
                   $"{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(
            string hashedPassword,
            string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) ||
                string.IsNullOrEmpty(providedPassword))
            {
                return false;
            }

            try
            {
                string[] parts = hashedPassword.Split('.');

                if (parts.Length != 3 ||
                    !int.TryParse(parts[0], out int iterations) ||
                    iterations < 1)
                {
                    return false;
                }

                byte[] salt =
                    Convert.FromBase64String(parts[1]);

                byte[] expectedHash =
                    Convert.FromBase64String(parts[2]);

                if (expectedHash.Length == 0)
                {
                    return false;
                }

                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    providedPassword,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(
                    actualHash,
                    expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}