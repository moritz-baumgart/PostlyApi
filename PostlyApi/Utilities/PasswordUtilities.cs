using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace PostlyApi.Utilities
{
    public class PasswordUtilities
    {
        /// <summary>
        /// Hashes the given password using argon2.
        /// </summary>
        /// <param name="password">The password to be hashed.</param>
        /// <returns>The password hash as byte array.</returns>
        public static byte[] ComputePasswordHash(string password)
        {
            var argon = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Iterations = 3,
                DegreeOfParallelism = 8,
                MemorySize = 1024 * 512
            };
            return argon.GetBytes(32);
        }

        /// <summary>
        /// Verifies if a given password is equal to a given password hash.
        /// </summary>
        /// <param name="password">The password the be verified.</param>
        /// <param name="passwordHash">The password hash, which the password should be compared to.</param>
        /// <returns>True if the password is verified, false else otherwise.</returns>
        public static bool VerifyPassword(string password, byte[] passwordHash)
        {
            return ComputePasswordHash(password).SequenceEqual(passwordHash);
        }
    }
}
