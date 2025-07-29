using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniUtils.Editor;
using UniUtils.Extensions;
using UniUtils.GameObjects;

namespace UniUtils.Examples
{
    /// <summary>
    /// A simple music player that continuously plays a shuffled list of audio tracks
    /// with smooth cross-fade transitions between each track.
    /// </summary>
    public class MusicPlayerExample : EphemeralSingleton<MusicPlayerExample>
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> tracks;
        [SerializeField] private float trackTransitionDuration = 3f;

        public AudioClip CurrentTrack => audioSource.clip;

        private Coroutine playCoroutine;

        public void StartPlayback()
        {
            if (playCoroutine != null) StopCoroutine(playCoroutine);
            playCoroutine = StartCoroutine(PlayTracksRoutine(tracks, trackTransitionDuration));
        }

        public void StopPlayback()
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                audioSource.Stop();
                audioSource.clip = null;
            }

            playCoroutine = null;
        }

        /// <summary>
        /// Plays a shuffled list of audio tracks continuously with smooth cross-fade transitions between each track.
        /// </summary>
        /// <param name="playlist">The list of audio clips to play.</param>
        /// <param name="volume">The target playback volume for each track. Defaults to 1.</param>
        /// <returns>
        /// An <see cref="IEnumerator"/> that plays each track in a loop with smooth transitions,
        /// intended to be run as a coroutine.
        /// </returns>
        /// <remarks>
        /// Each track is selected in order after shuffling the initial list.
        /// A smooth cross-fade is applied by starting the next track slightly before the current one ends.
        /// </remarks>
        private IEnumerator PlayTracksRoutine(
            List<AudioClip> playlist,
            float volume = 1f
        )
        {
            this.Log($"Starting playback of {playlist.Count} tracks.");

            // Shuffle the tracks and select a random starting track
            playlist = new List<AudioClip>(playlist).Shuffle();
            AudioClip currentTrack = playlist.Random();

            while (true)
            {
                this.Log($"Play track: {currentTrack.name}.");
                // Transition to the next track
                // (Fades out the current track and fades in the new one)
                yield return audioSource.TransitionToClip(
                    currentTrack,
                    transitionDuration: trackTransitionDuration,
                    targetVolume: volume
                );
                // Wait for the track to finish (minus half the transition duration to allow for a smooth transition)
                yield return audioSource.WaitUntilFinished(endTimeOffset: trackTransitionDuration / 2f);
                // Get the next track in the list according to the current track
                currentTrack = playlist.Next(currentTrack);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Custom inspector for the <see cref="MusicPlayerExample"/> MonoBehaviour.
    /// Adds runtime buttons to control playback and displays the current track name in the editor.
    /// </summary>
    [CustomEditor(typeof(MusicPlayerExample))]
    public class MusicPlayerExampleEditor : CustomEditorDrawer<MusicPlayerExample>
    {
        protected override List<(string, Action)> EditorButtons => new()
        {
            ("Play", () => Target.StartPlayback()),
            ("Stop", () => Target.StopPlayback()),
        };

        protected override void DrawProperties()
        {
            EditorGUILayout.LabelField("Current Track", Target.CurrentTrack ? Target.CurrentTrack.name : "None");
        }
    }
#endif
}