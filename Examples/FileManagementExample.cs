using System;
using UnityEngine;
using UniUtils.Data;
using UniUtils.Extensions;

namespace UniUtils.Examples
{
    /// <summary>
    /// Demonstrates basic encrypted file save/load operations using a custom file handling and encryption utility.
    /// </summary>
    public class FileManagementExample : MonoBehaviour
    {
        private FileHandle saveGameFileHandle;
        private static string encryptionKey = "key123";
        private string savesLocation = "saves";

        // A sample save data object to simulate storing a game state.
        private static MySaveData SaveGameData => new()
        {
            name = "SavedGame",
            playerHealth = 100f,
            playerPosition = Vector3.zero,
        };

        private void Awake()
        {
            // Initialize the file handle for saving game data
            saveGameFileHandle = new FileHandle($"{savesLocation}/MySaveData.json");

            // Attempt to load game data at startup
            MySaveData loadedData = LoadGame();

            if (loadedData != null)
            {
                this.Log($"Loaded game data: {loadedData.name}");

                // Backup the save file with a timestamp and delete the original
                string newFilePath = $"{savesLocation}/{DateTime.Now:yyyyMMdd\\THHmmss}_{saveGameFileHandle.FileName}";

                saveGameFileHandle.Copy(newFilePath, canOverwrite: true);
                saveGameFileHandle.Delete();
            }
            else
            {
                this.Log("No saved game data found");
            }
        }

        private void OnApplicationQuit()
        {
            // Automatically save game data on quit
            SaveGame();
        }

        /// <summary>
        /// Saves the current game state to disk, encrypted with a static key.
        /// </summary>
        public void SaveGame()
        {
            string cryptedData = Crypter.EncryptToBase64(SaveGameData.ToJson(), encryptionKey);
            saveGameFileHandle.WriteText(cryptedData);

            this.Log($"Game saved: {SaveGameData.name}");
        }

        /// <summary>
        /// Loads and decrypts game data from disk, if available.
        /// </summary>
        /// <returns>The deserialized save data, or null if no file exists.</returns>
        public MySaveData LoadGame()
        {
            if (!saveGameFileHandle.Exists)
                return null;

            string textData = saveGameFileHandle.ReadText();
            string decryptedData = Crypter.DecryptFromBase64(textData, encryptionKey);

            return MySaveData.FromJson(decryptedData);
        }

        /// <summary>
        /// Deletes all saved game data files.
        /// </summary>
        public void ClearSaveData()
        {
            FileManager.GetFilesInDirectory(savesLocation).ForEach(fileHandle => fileHandle.Delete());
        }
    }

    /// <summary>
    /// A simple data class representing saved game state.
    /// Inherits JSON serialization and deserialization from JsonObject.
    /// </summary>
    public class MySaveData : JsonObject<MySaveData>
    {
        public string name;
        public float playerHealth;
        public Vector3 playerPosition;
    }
}