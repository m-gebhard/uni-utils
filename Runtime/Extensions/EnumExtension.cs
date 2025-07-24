using System;
using Random = UnityEngine.Random;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extensions for enums.
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// Returns a random value from the specified enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>A random value from the enum.</returns>
        /// <example>
        /// <code>
        /// // Example enum
        /// enum Colors { Red, Green, Blue }
        ///
        /// // Get a random color
        /// Colors randomColor = EnumExtension.RandomValue&lt;Colors&gt;();
        /// Debug.Log(randomColor);
        /// </code>
        /// </example>
        public static T RandomValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(Random.Range(0, values.Length));
        }
    }
}