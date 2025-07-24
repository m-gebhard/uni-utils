using System.Globalization;
using UnityEngine;

namespace UniUtils.Data
{
    /// <summary>
    /// Provides utility methods for formatting values.
    /// </summary>
    public static class Formatters
    {
        /// <summary>
        /// Formats a float value representing time in seconds to a string in the format "MM:SS:FF".
        /// </summary>
        /// <param name="time">The time value in seconds.</param>
        /// <returns>A formatted time string in the format "MM:SS:FF".</returns>
        /// <example>
        /// <code>
        /// float playTime = 125.37f; // 2 minutes, 5 seconds, 370 milliseconds
        /// string formatted = Formatters.FormatFloatToTimeString(playTime);
        /// Debug.Log(formatted); // Output: "02:05:37"
        /// </code>
        /// </example>
        public static string FormatFloatToTimeString(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

            return $"{minutes:D2}:{seconds:D2}:{milliseconds / 10:D2}";
        }

        /// <summary>
        /// Formats a double value to a USD currency string.
        /// </summary>
        /// <param name="value">The double value to format.</param>
        /// <returns>A formatted USD currency string.</returns>
        /// <example>
        /// <code>
        /// double price = 49.99;
        /// string formatted = Formatters.FormatDoubleToUsdString(price);
        /// Debug.Log(formatted); // Output: "$49.99"
        /// </code>
        /// </example>
        public static string FormatDoubleToUsdString(double value)
        {
            return value.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}