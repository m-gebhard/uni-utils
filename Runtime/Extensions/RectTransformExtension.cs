using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for RectTransforms.
    /// </summary>
    public static class RectTransformExtension
    {
        /// <summary>
        /// Rebuilds the layout of the rect transform to fix potential sizing issues and UI glitches.
        /// </summary>
        /// <param name="rect">The RectTransform of object.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(myRectTransform.RebuildLayout());
        /// </code>
        /// </example>
        public static IEnumerator RebuildLayout(this RectTransform rect)
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}