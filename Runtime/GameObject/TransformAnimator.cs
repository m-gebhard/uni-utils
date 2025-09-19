using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace UniUtils.GameObjects
{
    /// <summary>
    /// A component that animates the position, rotation, and scale of a target Transform
    /// based on configurable animation settings.
    /// </summary>
    public class TransformAnimator : MonoBehaviour
    {
        #region Fields

        [Header("Setup")]
        [SerializeField, Tooltip("Target Transform to animate. If null, will use this GameObject's transform.")]
        protected Transform target;
        [SerializeField, Tooltip("Enable or disable the entire animator.")]
        protected bool isEnabled = true;
        [Space]
        [SerializeField, Tooltip("Configuration for position animation.")]
        protected TransformAnimatorComponentData positionConfig;
        [Space]
        [SerializeField, Tooltip("Configuration for rotation animation.")]
        protected TransformAnimatorComponentData rotationConfig;
        [Space]
        [SerializeField, Tooltip("Configuration for scale animation.")]
        protected TransformAnimatorComponentData scaleConfig;

        [Header("Events")]
        public UnityEvent OnPositionTransitionCompleted;
        public UnityEvent OnRotationTransitionCompleted;
        public UnityEvent OnScaleTransitionCompleted;
        public event Action OnPositionTransitionCompletedAction;
        public event Action OnRotationTransitionCompletedAction;
        public event Action OnScaleTransitionCompletedAction;

        public TransformAnimatorComponentData PositionConfig => positionConfig;
        public TransformAnimatorComponentData RotationConfig => rotationConfig;
        public TransformAnimatorComponentData ScaleConfig => scaleConfig;

        protected float positionProgress;
        protected float rotationProgress;
        protected float scaleProgress;

        protected bool isPositionForward = true;
        protected bool isRotationForward = true;
        protected bool isScaleForward = true;

        protected bool hasCompletedPositionCycle;
        protected bool hasCompletedRotationCycle;
        protected bool hasCompletedScaleCycle;

        protected float positionDelayTimer;
        protected float rotationDelayTimer;
        protected float scaleDelayTimer;

        protected Vector3 positionTarget;
        protected Vector3 rotationTarget;
        protected Vector3 scaleTarget;

        #endregion

        /// <summary>
        /// Initializes the target Transform and sets its initial position, rotation, and scale
        /// based on the animation configurations. Also calculates randomized targets for animations.
        /// </summary>
        protected virtual void Awake()
        {
            if (target == null) target = transform;

            if (positionConfig.isEnabled) target.localPosition = positionConfig.startValue;
            if (rotationConfig.isEnabled) target.localEulerAngles = rotationConfig.startValue;
            if (scaleConfig.isEnabled) target.localScale = scaleConfig.startValue;

            positionTarget = GetRandomizedTarget(positionConfig.endValue, positionConfig.targetVariance);
            rotationTarget = GetRandomizedTarget(rotationConfig.endValue, rotationConfig.targetVariance);
            scaleTarget = GetRandomizedTarget(scaleConfig.endValue, scaleConfig.targetVariance);

            positionProgress = isPositionForward ? 0f : 1f;
            rotationProgress = isRotationForward ? 0f : 1f;
            scaleProgress = isScaleForward ? 0f : 1f;
        }

        /// <summary>
        /// Updates the animation for position, rotation, and scale on each frame.
        /// </summary>
        protected virtual void Update()
        {
            if (!isEnabled || !target) return;

            if (positionConfig.isEnabled) AnimatePosition();
            if (rotationConfig.isEnabled) AnimateRotation();
            if (scaleConfig.isEnabled) AnimateScale();
        }

        #region Setters

        /// <summary>
        /// Enables or disables the animator.
        /// </summary>
        /// <param name="state">True to enable, false to disable.</param>
        public virtual void SetIsEnabled(bool state) => isEnabled = state;

        /// <summary>
        /// Resets the progress and delay timers for position, rotation, and scale animations.
        /// </summary>
        public virtual void ResetProgress()
        {
            positionProgress = isPositionForward ? 0f : 1f;
            rotationProgress = isRotationForward ? 0f : 1f;
            scaleProgress = isScaleForward ? 0f : 1f;

            positionDelayTimer = 0f;
            rotationDelayTimer = 0f;
            scaleDelayTimer = 0f;

            hasCompletedPositionCycle = false;
            hasCompletedRotationCycle = false;
            hasCompletedScaleCycle = false;
        }

        /// <summary>
        /// Sets the configuration for position animation.
        /// </summary>
        /// <param name="config">The configuration data for position animation.</param>
        public virtual void SetPositionConfig(TransformAnimatorComponentData config) => positionConfig = config;

        /// <summary>
        /// Sets the configuration for rotation animation.
        /// </summary>
        /// <param name="config">The configuration data for rotation animation.</param>
        public virtual void SetRotationConfig(TransformAnimatorComponentData config) => rotationConfig = config;

        /// <summary>
        /// Sets the configuration for scale animation.
        /// </summary>
        /// <param name="config">The configuration data for scale animation.</param>
        public virtual void SetScaleConfig(TransformAnimatorComponentData config) => scaleConfig = config;

        #endregion

        #region Helpers & Animation

        /// <summary>
        /// Calculates a randomized target value by applying a random offset to the base end value.
        /// </summary>
        /// <param name="baseEnd">The base end value to which the random offset will be applied.</param>
        /// <param name="offset">The range of random offset to apply to each axis.</param>
        /// <returns>A new Vector3 with randomized values within the specified offset range.</returns>
        protected static Vector3 GetRandomizedTarget(Vector3 baseEnd, Vector3 offset)
        {
            return new Vector3(
                baseEnd.x + Random.Range(-offset.x, offset.x),
                baseEnd.y + Random.Range(-offset.y, offset.y),
                baseEnd.z + Random.Range(-offset.z, offset.z)
            );
        }

        /// <summary>
        /// Animates the position of the target Transform based on the configuration.
        /// </summary>
        protected virtual void AnimatePosition()
        {
            if (positionConfig.speed <= 0f) return;

            if (positionDelayTimer > 0f)
            {
                positionDelayTimer -= Time.deltaTime;
                return;
            }

            positionProgress += (isPositionForward ? 1 : -1) * Time.deltaTime * positionConfig.speed;
            positionProgress = Mathf.Clamp01(positionProgress);

            float eval = positionConfig.animationCurve.Evaluate(positionProgress);
            target.localPosition = Vector3.Lerp(positionConfig.startValue, positionTarget, eval);

            if (isPositionForward && positionProgress >= 1f)
            {
                if (!hasCompletedPositionCycle)
                {
                    OnPositionTransitionCompleted?.Invoke();
                    OnPositionTransitionCompletedAction?.Invoke();
                }

                if (positionConfig.canLoop)
                {
                    isPositionForward = false;
                    positionDelayTimer = positionConfig.loopDelay;
                    hasCompletedPositionCycle = false;
                }
                else
                {
                    hasCompletedPositionCycle = true;
                }
            }

            if (!isPositionForward && positionProgress <= 0f)
            {
                if (!hasCompletedPositionCycle)
                {
                    OnPositionTransitionCompleted?.Invoke();
                    OnPositionTransitionCompletedAction?.Invoke();
                }

                if (positionConfig.canLoop)
                {
                    positionTarget = GetRandomizedTarget(positionConfig.endValue, positionConfig.targetVariance);
                    isPositionForward = true;
                    positionDelayTimer = positionConfig.loopDelay;
                    hasCompletedPositionCycle = false;
                }
                else
                {
                    hasCompletedPositionCycle = true;
                }
            }
        }

        /// <summary>
        /// Animates the rotation of the target Transform based on the configuration.
        /// </summary>
        protected virtual void AnimateRotation()
        {
            if (rotationConfig.speed <= 0f) return;

            if (rotationDelayTimer > 0f)
            {
                rotationDelayTimer -= Time.deltaTime;
                return;
            }

            rotationProgress += (isRotationForward ? 1 : -1) * Time.deltaTime * rotationConfig.speed;
            rotationProgress = Mathf.Clamp01(rotationProgress);

            float eval = rotationConfig.animationCurve.Evaluate(rotationProgress);

            Quaternion startQ = Quaternion.Euler(rotationConfig.startValue);
            Quaternion targetQ = Quaternion.Euler(rotationTarget);
            target.localRotation = Quaternion.Slerp(startQ, targetQ, eval);

            if (isRotationForward && rotationProgress >= 1f)
            {
                if (!hasCompletedRotationCycle)
                {
                    OnRotationTransitionCompleted?.Invoke();
                    OnRotationTransitionCompletedAction?.Invoke();
                }

                if (rotationConfig.canLoop)
                {
                    isRotationForward = false;
                    rotationDelayTimer = rotationConfig.loopDelay;
                    hasCompletedRotationCycle = false;
                }
                else
                {
                    hasCompletedRotationCycle = true;
                }
            }

            if (!isRotationForward && rotationProgress <= 0f)
            {
                if (!hasCompletedRotationCycle)
                {
                    OnRotationTransitionCompleted?.Invoke();
                    OnRotationTransitionCompletedAction?.Invoke();
                }

                if (rotationConfig.canLoop)
                {
                    rotationTarget = GetRandomizedTarget(rotationConfig.endValue, rotationConfig.targetVariance);
                    isRotationForward = true;
                    rotationDelayTimer = rotationConfig.loopDelay;
                    hasCompletedRotationCycle = false;
                }
                else
                {
                    hasCompletedRotationCycle = true;
                }
            }
        }

        /// <summary>
        /// Animates the scale of the target Transform based on the configuration.
        /// </summary>
        protected virtual void AnimateScale()
        {
            if (scaleConfig.speed <= 0f) return;

            if (scaleDelayTimer > 0f)
            {
                scaleDelayTimer -= Time.deltaTime;
                return;
            }

            scaleProgress += (isScaleForward ? 1 : -1) * Time.deltaTime * scaleConfig.speed;
            scaleProgress = Mathf.Clamp01(scaleProgress);

            float eval = scaleConfig.animationCurve.Evaluate(scaleProgress);
            target.localScale = Vector3.Lerp(scaleConfig.startValue, scaleTarget, eval);

            if (isScaleForward && scaleProgress >= 1f)
            {
                if (!hasCompletedScaleCycle)
                {
                    OnScaleTransitionCompleted?.Invoke();
                    OnScaleTransitionCompletedAction?.Invoke();
                }

                if (scaleConfig.canLoop)
                {
                    isScaleForward = false;
                    scaleDelayTimer = scaleConfig.loopDelay;
                    hasCompletedScaleCycle = false;
                }
                else
                {
                    hasCompletedScaleCycle = true;
                }
            }

            if (!isScaleForward && scaleProgress <= 0f)
            {
                if (!hasCompletedScaleCycle)
                {
                    OnScaleTransitionCompleted?.Invoke();
                    OnScaleTransitionCompletedAction?.Invoke();
                }

                if (scaleConfig.canLoop)
                {
                    scaleTarget = GetRandomizedTarget(scaleConfig.endValue, scaleConfig.targetVariance);
                    isScaleForward = true;
                    scaleDelayTimer = scaleConfig.loopDelay;
                    hasCompletedScaleCycle = false;
                }
                else
                {
                    hasCompletedScaleCycle = true;
                }
            }
        }

        #endregion

        /// <summary>
        /// Represents the configuration data for animating a specific transform component.
        /// </summary>
        [Serializable]
        public class TransformAnimatorComponentData
        {
            [Tooltip("Enable or disable this component animation.")]
            public bool isEnabled;
            [Space]
            [Tooltip("Animation speed in units per second.")] [Range(0f, 30f)]
            public float speed = 1f;
            [Tooltip("Should the animation loop back and forth.")]
            public bool canLoop;
            [Tooltip("Delay between loops in seconds.")] [Range(0f, 30f)]
            public float loopDelay;
            [Space]
            [Tooltip("Starting value of the transform component.")]
            public Vector3 startValue;
            [Tooltip("Ending value of the transform component.")]
            public Vector3 endValue;
            [Tooltip("Random variance offset to apply each new forward iteration to the end value.")]
            public Vector3 targetVariance;
            [Tooltip("Animation curve to control interpolation.")]
            public AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
}