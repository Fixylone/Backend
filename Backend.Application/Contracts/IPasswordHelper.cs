namespace Backend.Application.Contracts
{
    /// <summary>
    /// The password helper interface. Contains methods for working with passwords.
    /// </summary>
    public interface IPasswordHelper
    {
        /// <summary>
        /// Creates the password hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>The password hash and salt.</returns>
        (byte[] passwordHash, byte[] passwordSalt) CreateHash(string password);

        /// <summary>
        /// Verifies the password hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hash">The stored hash.</param>
        /// <param name="salt">The stored salt.</param>
        /// <returns><c>true</c> if password hash is valid, <c>false</c> otherwise.</returns>
        bool VerifyHash(string password, byte[] hash, byte[] salt);

        /// <summary>
        /// Generates the random alphanumeric string.
        /// </summary>
        /// <param name="length">The string length.</param>
        /// <returns>The random alphanumeric string.</returns>
        string GenerateRandomString(int length);
    }
}
