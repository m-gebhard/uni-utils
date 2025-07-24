using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniUtils.GameObjects;

namespace UniUtils.SceneManagement
{
    /// <summary>
    /// Manages scene loading operations in Unity.
    /// </summary>
    /// <example>
    /// <code>
    /// // Example usage of SceneLoader
    /// public class GameFlowManager : MonoBehaviour
    /// {
    ///     private void Start()
    ///     {
    ///         // Subscribe to scene loading events
    ///         SceneLoader.Instance.OnSceneChangeStart += OnSceneStart;
    ///         SceneLoader.Instance.OnSceneChangeProgressUpdate += OnProgressUpdate;
    ///         SceneLoader.Instance.OnSceneChangeFinished += OnSceneFinished;
    ///
    ///         // Start loading scene with index 2
    ///         SceneLoader.Instance.LoadScene(2, () => Debug.Log("Scene loaded successfully!"));
    ///     }
    ///
    ///     private void OnSceneStart(int sceneIndex)
    ///     {
    ///         Debug.Log("Loading started for scene: " + sceneIndex);
    ///     }
    ///
    ///     private void OnProgressUpdate(float progress)
    ///     {
    ///         Debug.Log("Loading progress: " + (progress * 100) + "%");
    ///     }
    ///
    ///     private void OnSceneFinished(int sceneIndex)
    ///     {
    ///         Debug.Log("Loading finished for scene: " + sceneIndex);
    ///     }
    /// }
    /// </code>
    /// </example>
    public class SceneLoader : PersistentSingleton<SceneLoader>
    {
        /// <summary>
        /// Event triggered when a scene change starts.
        /// </summary>
        public event Action<int> OnSceneChangeStart;

        /// <summary>
        /// Event triggered to update the progress of a scene change.
        /// </summary>
        public event Action<float> OnSceneChangeProgressUpdate;

        /// <summary>
        /// Event triggered when a scene change finishes.
        /// </summary>
        public event Action<int> OnSceneChangeFinished;

        /// <summary>
        /// The asynchronous operation for loading a scene.
        /// </summary>
        private AsyncOperation operation;

        /// <summary>
        /// Loads a scene asynchronously.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to load.</param>
        /// <param name="callback">Optional callback to invoke after the scene is loaded.</param>
        /// <returns>An IEnumerator for the asynchronous operation.</returns>
        private IEnumerator LoadSceneAsync(int sceneIndex, Action callback = null)
        {
            operation = SceneManager.LoadSceneAsync(sceneIndex);
            OnSceneChangeStart?.Invoke(sceneIndex);

            while (operation is { isDone: false })
            {
                OnSceneChangeProgressUpdate?.Invoke(operation.progress);
                yield return null;
            }

            OnSceneChangeFinished?.Invoke(sceneIndex);
            callback?.Invoke();
        }

        /// <summary>
        /// Starts the asynchronous scene loading process.
        /// </summary>
        /// <param name="sceneIndex">The index of the scene to load.</param>
        /// <param name="callback">Optional callback to invoke after the scene is loaded.</param>
        /// <returns>A Coroutine for the asynchronous operation.</returns>
        public Coroutine LoadScene(int sceneIndex, Action callback = null)
        {
            return StartCoroutine(LoadSceneAsync(sceneIndex, callback));
        }
    }
}