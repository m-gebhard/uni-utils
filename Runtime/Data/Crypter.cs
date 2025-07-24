using System;
using System.Text;

namespace UniUtils.Data
{
    /// <summary>
    /// Provides methods for encrypting and decrypting strings using a simple XOR-based algorithm.
    /// </summary>
    public class Crypter
    {
        /// <summary>
        /// Encrypts a string using a XOR-based algorithm and returns the encrypted data as a byte array.
        /// </summary>
        /// <param name="input">The input string to encrypt.</param>
        /// <param name="encryptionKey">The encryption key used for the XOR operation.</param>
        /// <returns>A byte array containing the encrypted data.</returns>
        /// <example>
        /// <code>
        /// string original = "Secret123";
        /// string key = "key!";
        /// byte[] encrypted = Crypter.Encrypt(original, key);
        /// // encrypted contains binary data
        /// </code>
        /// </example>
        public static byte[] Encrypt(string input, string encryptionKey)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

            byte[] encryptedBytes = new byte[inputBytes.Length];
            for (int i = 0; i < inputBytes.Length; i++)
            {
                encryptedBytes[i] = (byte)(inputBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return encryptedBytes;
        }

        /// <summary>
        /// Encrypts a string using a XOR-based algorithm and returns the encrypted data as a Base64-encoded string.
        /// </summary>
        /// <param name="input">The input string to encrypt.</param>
        /// <param name="encryptionKey">The encryption key used for the XOR operation.</param>
        /// <returns>A Base64-encoded string containing the encrypted data.</returns>
        /// <example>
        /// <code>
        /// string secret = "MyPassword";
        /// string key = "abc123";
        /// string encryptedBase64 = Crypter.EncryptToBase64(secret, key);
        /// Debug.Log(encryptedBase64); // Outputs something like: "GhoaFh1R..."
        /// </code>
        /// </example>
        public static string EncryptToBase64(string input, string encryptionKey)
        {
            byte[] encryptedBytes = Encrypt(input, encryptionKey);
            return Convert.ToBase64String(encryptedBytes);
        }

        /// <summary>
        /// Decrypts a byte array using a XOR-based algorithm and returns the decrypted string.
        /// </summary>
        /// <param name="encryptedBytes">The byte array containing the encrypted data.</param>
        /// <param name="encryptionKey">The encryption key used for the XOR operation.</param>
        /// <returns>The decrypted string.</returns>
        /// <example>
        /// <code>
        /// string text = "HelloWorld";
        /// string key = "k";
        /// byte[] encrypted = Crypter.Encrypt(text, key);
        /// string decrypted = Crypter.Decrypt(encrypted, key);
        /// Debug.Log(decrypted); // "HelloWorld"
        /// </code>
        /// </example>
        public static string Decrypt(byte[] encryptedBytes, string encryptionKey)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

            byte[] decryptedBytes = new byte[encryptedBytes.Length];
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

        /// <summary>
        /// Decrypts a Base64-encoded string using a XOR-based algorithm and returns the decrypted string.
        /// </summary>
        /// <param name="encryptedBase64">The Base64-encoded string containing the encrypted data.</param>
        /// <param name="encryptionKey">The encryption key used for the XOR operation.</param>
        /// <returns>The decrypted string.</returns>
        /// <example>
        /// <code>
        /// string key = "secret";
        /// string encrypted = Crypter.EncryptToBase64("HiddenMessage", key);
        /// string decrypted = Crypter.DecryptFromBase64(encrypted, key);
        /// Debug.Log(decrypted); // Outputs: "HiddenMessage"
        /// </code>
        /// </example>
        public static string DecryptFromBase64(string encryptedBase64, string encryptionKey)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
            return Decrypt(encryptedBytes, encryptionKey);
        }
    }
}