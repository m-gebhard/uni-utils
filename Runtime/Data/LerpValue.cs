using System;

namespace UniUtils.Data
{
    /// <summary>
    /// A generic class for managing and interpolating a value towards a target value over time.
    /// </summary>
    /// <typeparam name="T">The type of the value to interpolate.</typeparam>
    /// <example>
    /// <code>
    /// using UnityEngine;
    ///
    /// public class ExampleUsage : MonoBehaviour
    /// {
    ///     private LerpValue&lt;float&gt; lerpAlpha;
    ///
    ///     void Start()
    ///     {
    ///         // Start with 0, use Mathf.Lerp, and set speed to 2
    ///         lerpAlpha = new LerpValue&lt;float&gt;(0f, Mathf.Lerp, 2f);
    ///         lerpAlpha.SetTarget(1f); // Targeting full alpha
    ///     }
    ///
    ///     void Update()
    ///     {
    ///         lerpAlpha.Update(Time.deltaTime);
    ///         float currentAlpha = lerpAlpha.Value;
    ///         // Use currentAlpha for fading UI, etc.
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class LerpValue<T>
    {
        /// <summary>
        /// The current interpolated value.
        /// </summary>
        private T currentValue;

        /// <summary>
        /// The target value to interpolate towards.
        /// </summary>
        private T targetValue;

        /// <summary>
        /// The interpolation function used to calculate the interpolated value.
        /// </summary>
        private readonly Func<T, T, float, T> lerpFunc;

        /// <summary>
        /// The speed at which the value interpolates towards the target.
        /// </summary>
        private float lerpSpeed;

        /// <summary>
        /// Gets the current interpolated value.
        /// </summary>
        /// <returns>The current value.</returns>
        public T Value => currentValue;

        /// <summary>
        /// Gets the target value.
        /// </summary>
        /// <returns>The target value.</returns>
        public T Target => targetValue;

        /// <summary>
        /// Gets the speed at which the value interpolates towards the target.
        /// </summary>
        /// <returns>The speed of interpolation.</returns>
        public float LerpSpeed => lerpSpeed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LerpValue{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value of the interpolation.</param>
        /// <param name="lerpFunction">The interpolation function to use.</param>
        /// <param name="speed">The speed of interpolation (default is 5f).</param>
        /// <exception cref="System.ArgumentNullException">ArgumentNullException: Thrown if <paramref name="lerpFunction"/> is null.</exception>
        public LerpValue(T initialValue, Func<T, T, float, T> lerpFunction, float speed = 5f)
        {
            currentValue = initialValue;
            targetValue = initialValue;
            lerpFunc = lerpFunction ?? throw new ArgumentNullException(nameof(lerpFunction));
            lerpSpeed = speed;
        }

        /// <summary>
        /// Sets a new target value for interpolation.
        /// </summary>
        /// <param name="newTarget">The new target value.</param>
        public void SetTarget(T newTarget)
        {
            targetValue = newTarget;
        }

        /// <summary>
        /// Immediately sets the current and target values to a specified value.
        /// </summary>
        /// <param name="newValue">The value to set.</param>
        public void ForceSet(T newValue)
        {
            currentValue = newValue;
            targetValue = newValue;
        }

        /// <summary>
        /// Sets the speed at which the value interpolates towards the target.
        /// </summary>
        /// <param name="newSpeed">The new interpolation speed.</param>
        public void SetLerpSpeed(float newSpeed)
        {
            lerpSpeed = newSpeed;
        }

        /// <summary>
        /// Updates the current value by interpolating towards the target value.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public void Update(float deltaTime)
        {
            currentValue = lerpFunc(currentValue, targetValue, deltaTime * lerpSpeed);
        }
    }
}