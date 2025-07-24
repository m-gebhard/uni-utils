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
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// Deserializes the given JSON string to an object of type T.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>An object of type T deserialized from the JSON string.</returns>
        /// <example>
        /// <code>
        /// [Serializable]
        /// public class PlayerData : JSONObject&lt;PlayerData&gt;
        /// {
        ///     public string playerName;
        ///     public int score;
        /// }
        ///
        /// string json = "{\"playerName\":\"Alex\",\"score\":150}";
        /// PlayerData data = new PlayerData().FromJson(json);
        /// Debug.Log($"{data.playerName} scored {data.score}"); // Output: Alex scored 150
        /// </code>
        /// </example>
        public T FromJson(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}