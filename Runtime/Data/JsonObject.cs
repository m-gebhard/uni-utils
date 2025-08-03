using System;
using UnityEngine;

namespace UniUtils.Data
{
    /// <summary>
    /// Represents a generic JSON object that can be serialized and deserialized using Unity's JsonUtility.
    /// The derived class is required to have the [Serializable] attribute.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    [Serializable]
    public abstract class JsonObject<T>
    {
        /// <summary>
        /// Serializes the current object to a JSON string.
        /// </summary>
        /// <param name="prettyPrint">If true, formats the JSON string with better readability.</param>
        /// <returns>A JSON string representation of the current object.</returns>
        /// <example>
        /// <code>
        /// [Serializable]
        /// public class PlayerData : JSONObject&lt;PlayerData&gt;
        /// {
        ///     public string playerName;
        ///     public int score;
        /// }
        ///
        /// PlayerData data = new PlayerData { playerName = "Alex", score = 150 };
        /// string json = data.ToJson();
        /// Debug.Log(json); // Output: {"playerName":"Alex","score":150}
        /// </code>
        /// </example>
        public string ToJson(bool prettyPrint = false)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }

        /// <example>
        /// <code>
        /// // Define a serializable data model
        /// [Serializable]
        /// public class PlayerData : JsonObject&lt;PlayerData&gt;
        /// {
        ///     public string playerName;
        ///     public int score;
        /// }
        ///
        /// // Create an instance and serialize it
        /// PlayerData data = new PlayerData
        /// {
        ///     playerName = "Alex",
        ///     score = 150
        /// };
        ///
        /// string json = data.ToJson();
        /// Debug.Log(json);
        /// // Output: {"playerName":"Alex","score":150}
        ///
        /// // You can also deserialize it back:
        /// PlayerData deserialized = PlayerData.FromJson(json);
        /// Debug.Log(deserialized.playerName);
        /// // Output: Alex
        /// </code>
        /// </example>
        public static T FromJson(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// Overrides the ToString method to return the JSON representation of the object.
        /// </summary>
        /// <returns>A JSON string representation of the current object.</returns>
        public override string ToString() => ToJson();
    }
}