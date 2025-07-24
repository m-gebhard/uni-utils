namespace UniUtils.Data
{
    /// <summary>
    /// Specifies the storage location for files within the application.
    /// </summary>
    public enum EStorageLocation
    {
        /// <summary>
        /// (Application.persistentDataPath) Represents the persistent storage location, typically used for saving data that should remain across application sessions.
        /// </summary>
        Persistent,

        /// <summary>
        /// (Application.dataPath) Represents the application's data path, often used for accessing files bundled with the application.
        /// </summary>
        DataPath,

        /// <summary>
        /// (Application.temporaryCachePath) Represents a temporary storage location, typically used for storing data that does not need to persist.
        /// </summary>
        Temporary,

        /// <summary>
        /// (Application.streamingAssetsPath) Represents the streaming assets folder, often used for accessing read-only data bundled with the application.
        /// </summary>
        StreamingAssets,
    }
}