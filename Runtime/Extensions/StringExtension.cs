using System.Text.RegularExpressions;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for Strings.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Swaps occurrences of two specified substrings within the given text.
        /// </summary>
        /// <param name="text">The original text where the swap will occur.</param>
        /// <param name="swapA">The first substring to swap.</param>
        /// <param name="swapB">The second substring to swap.</param>
        /// <returns>A new string with the specified substrings swapped.</returns>
        /// <example>
        /// <code>
        /// string result = "hello world".Swap("hello", "world");
        /// // result == "world hello"
        /// </code>
        /// </example>
        public static string Swap(this string text, string swapA, string swapB)
        {
            return Regex.Replace(text, Regex.Escape(swapA) + "|" + Regex.Escape(swapB),
                m => m.Value == swapA ? swapB : swapA);
        }

        /// <summary>
        /// Sanitizes the input string by replacing spaces with underscores and removing special characters.
        /// </summary>
        /// <param name="text">The input string to be sanitized.</param>
        /// <param name="allowedPattern">
        /// A regex pattern defining the set of allowed characters.
        /// Defaults to "A-Za-z0-9_", which allows alphanumeric characters and underscores.
        /// </param>
        /// <returns>A sanitized string with spaces replaced by underscores and disallowed characters removed.</returns>
        /// <example>
        /// <code>
        /// string sanitized = "Hello, World!".Sanitize();
        /// // sanitized == "Hello_World"
        /// </code>
        /// </example>
        public static string Sanitize(this string text, string allowedPattern = "A-Za-z0-9_")
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = text.Replace(" ", "_");
            text = Regex.Replace(text, @$"[^{allowedPattern}]", "");

            return text;
        }
    }
}