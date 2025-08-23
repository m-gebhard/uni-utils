using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for Audio sources.
    /// </summary>
    public static class AudioSourceExtension
    {
        /// <summary>
        /// Fades the volume of the given AudioSource from a start volume to a target volume over a specified duration.
        /// </summary>
        /// <param name="audioSource">The AudioSource to fade.</param>
        /// <param name="duration">The duration over which to fade the volume.</param>
        /// <param name="targetVolume">The target volume to reach at the end of the fade.</param>
        /// <param name="startVolume">The starting volume. Defaults to 0.</param>
        /// <param name="onFinished">An optional callback to invoke when the fade is complete.</param>
        /// <returns>An IEnumerator that can be used to run the fade operation in a coroutine.</returns>
        /// <example>
        /// <code>
        /// // Usage example:
        /// IEnumerator FadeExample(AudioSource audioSource)
        /// {
        ///     // Fade volume to 0 over 2 seconds
        ///     yield return audioSource.FadeVolume(2f, 0f);
        ///
        ///     // Fade back to full volume over 1.5 seconds
        ///     yield return audioSource.FadeVolume(1.5f, 1f);
        /// }
        /// </code>
        /// <example>
        /// </example>
        /// <code>
        /// // Usage example:
        /// StartCoroutine(audioSource.FadeVolume(1.5f, 1f));
        /// </code>
        /// </example>
        public static IEnumerator FadeVolume(
            this AudioSource audioSource,
            float duration,
            float targetVolume,
            float? startVolume = null,
            Action onFinished = null)
        {
            float initialVolume = startVolume ?? audioSource.volume;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, currentTime / duration);

                yield return null;
            }

            onFinished?.Invoke();
        }

        /// <summary>
        /// Plays an AudioClip on the given AudioSource and waits until it finishes playing,
        /// with an optional offset to end playback early and a callback when finished.
        /// </summary>
        /// <param name="audioSource">The AudioSource to play the clip on.</param>
        /// <param name="clip">The AudioClip to play.</param>
        /// <param name="endTimeOffset">
        /// The time (in seconds) to offset from the end of the clip. Defaults to 0.
        /// If the offset is greater than or equal to the clip length, the method exits early.
        /// </param>
        /// <param name="onFinished">
        /// An optional callback to invoke when the clip finishes playing.
        /// </param>
        /// <returns>
        /// An IEnumerator that can be used to run the operation in a coroutine.
        /// </returns>
        /// <example>
        /// <code>
        /// // Usage example:
        /// IEnumerator PlayClip(AudioSource audioSource, AudioClip clip)
        /// {
        ///     yield return audioSource.PlayAndWaitUntilFinished(clip, 0.2f, () => Debug.Log("Clip finished!"));
        /// }
        /// // StartCoroutine(PlayClip(audioSource, clip));
        /// </code>
        /// </example>
        public static IEnumerator PlayAndWaitUntilFinished(
            this AudioSource audioSource,
            AudioClip clip,
            float endTimeOffset = 0f,
            Action onFinished = null
        )
        {
            if (!clip || clip.length <= endTimeOffset)
            {
                onFinished?.Invoke();
                yield break;
            }

            audioSource.clip = clip;
            audioSource.Play();

            while (audioSource.clip == clip && audioSource.time < clip.length - endTimeOffset)
            {
                if (!audioSource.isPlaying)
                    yield break;

                yield return null;
            }

            onFinished?.Invoke();
        }

        /// <summary>
        /// Waits until the current clip on the AudioSource finishes playing,
        /// with an optional offset to end early and a callback when finished.
        /// </summary>
        /// <param name="audioSource">The AudioSource playing the clip.</param>
        /// <param name="endTimeOffset">
        /// The time (in seconds) to offset from the end of the clip. Defaults to 0.
        /// If the offset is greater than or equal to the clip length, the method exits early.
        /// </param>
        /// <param name="onFinished">
        /// An optional callback to invoke when the clip finishes playing.
        /// </param>
        /// <returns>
        /// An IEnumerator to be used in a coroutine.
        /// </returns>
        /// <example>
        /// <code>
        /// yield return audioSource.WaitUntilFinished(0.1f, () => Debug.Log("Done"));
        /// </code>
        /// </example>
        public static IEnumerator WaitUntilFinished(
            this AudioSource audioSource,
            float endTimeOffset = 0f,
            Action onFinished = null
        )
        {
            AudioClip clip = audioSource.clip;

            if (!clip || !audioSource.isPlaying || clip.length <= endTimeOffset)
            {
                onFinished?.Invoke();
                yield break;
            }

            while (audioSource.clip == clip && audioSource.time < clip.length - endTimeOffset)
            {
                if (!audioSource.isPlaying)
                    yield break;

                yield return null;
            }

            onFinished?.Invoke();
        }


        /// <summary>
        /// Transitions the AudioSource to a new AudioClip by fading out the current clip (if playing),
        /// stopping it, and then fading in the new clip.
        /// </summary>
        /// <param name="audioSource">The AudioSource to transition.</param>
        /// <param name="clip">The new AudioClip to play.</param>
        /// <param name="targetVolume">
        /// The target volume to fade to when playing the new clip. Defaults to the current volume of the AudioSource.
        /// </param>
        /// <param name="transitionDuration">
        /// The total duration of the transition (fade out + fade in). Defaults to 1 second.
        /// </param>
        /// <param name="onFinished">
        /// An optional callback to invoke when the transition is finished.
        /// </param>
        /// <returns>
        /// An IEnumerator that can be used to run the transition in a coroutine.
        /// </returns>
        /// <example>
        /// <code>
        /// // Usage example:
        /// IEnumerator TransitionExample(AudioSource audioSource, AudioClip newClip)
        /// {
        ///     yield return audioSource.TransitionToClip(newClip, 0.8f, 2f);
        /// }
        /// // StartCoroutine(TransitionExample(audioSource, newClip));
        /// </code>
        /// </example>
        public static IEnumerator TransitionToClip(
            this AudioSource audioSource,
            AudioClip clip,
            float? targetVolume = null,
            float transitionDuration = 1f,
            Action onFinished = null
        )
        {
            if (!clip)
                yield break;

            targetVolume ??= audioSource.volume;
            float fadeDuration = transitionDuration / 2f;

            if (audioSource.isPlaying)
            {
                yield return audioSource.FadeVolume(fadeDuration, 0f);
                audioSource.Stop();
            }

            audioSource.clip = clip;
            audioSource.volume = 0f;
            audioSource.Play();

            yield return audioSource.FadeVolume(fadeDuration, targetVolume.Value);

            onFinished?.Invoke();
        }

        /// <summary>
        /// Plays an AudioClip with random pitch and volume variations for added audio diversity.
        /// </summary>
        /// <param name="audioSource">The AudioSource to play the clip on.</param>
        /// <param name="clip">The AudioClip to play.</param>
        /// <param name="audioVolume">The base volume at which to play the clip. Defaults to 1.</param>
        /// <param name="maxPitchVariation">The maximum variation in pitch. Defaults to 0.1.</param>
        /// <param name="maxVolumeVariation">The maximum variation in volume. Defaults to 0.1.</param>
        /// <example>
        /// <code>
        /// // Usage example:
        /// void PlayClipWithVariance(AudioSource audioSource, AudioClip clip)
        /// {
        ///     audioSource.PlayOneShotWithVariance(clip, 1f, 0.2f, 0.15f);
        /// }
        /// </code>
        /// </example>
        public static void PlayOneShotWithVariance(
            this AudioSource audioSource,
            AudioClip clip,
            float audioVolume = 1f,
            float maxPitchVariation = 0.1f,
            float maxVolumeVariation = 0.1f)
        {
            float pitchBefore = audioSource.pitch;

            audioSource.pitch = Random.Range(1f - maxPitchVariation, 1f + maxPitchVariation);
            float volumeScale = Random.Range(1f - maxVolumeVariation, 1f + maxVolumeVariation);

            audioSource.PlayOneShot(clip, audioVolume * volumeScale);
            audioSource.pitch = pitchBefore;
        }
    }
}