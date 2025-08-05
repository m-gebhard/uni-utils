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

        /// <summary>
        /// Truncates the input string to a specified maximum length without cutting off words.
        /// Appends a truncation indicator if necessary.
        /// </summary>
        /// <param name="text">The input string to be truncated.</param>
        /// <param name="maxLength">The maximum length of the truncated string (not counting the indicator).</param>
        /// <param name="indicator">The string to append if truncation occurs. Defaults to "...".</param>
        /// <returns>
        /// A truncated string that ends on a word boundary (space/tab/newline) with the truncation indicator appended.
        /// Returns the original string if it's shorter than or equal to maxLength.
        /// Returns an empty string if the input is null or maxLength is negative.
        /// </returns>
        /// <example>
        /// <code>
        /// string t1 = "This is a long string".Truncate(10);
        /// // t1 == "This is a…"
        ///
        /// string t2 = "Short".Truncate(10);
        /// // t2 == "Short"
        ///
        /// string t3 = null.Truncate(5);
        /// // t3 == ""
        ///
        /// string t4 = "Supercalifragilistic".Truncate(5);
        /// // t4 == "Super…"
        /// </code>
        /// </example>
        public static string Truncate(this string text, int maxLength, string indicator = "...")
        {
            if (string.IsNullOrEmpty(text) || maxLength < 0)
                return string.Empty;

            if (text.Length <= maxLength)
                return text;

            string slice = text[..maxLength];

            // Find the last whitespace character in the slice
            int lastSpace = slice.LastIndexOfAny(new[] { ' ', '\t', '\r', '\n' });

            if (lastSpace > 0)
            {
                // Cut at the last word boundary
                slice = slice[..lastSpace];
            }

            return slice + indicator;
        }
    }
}