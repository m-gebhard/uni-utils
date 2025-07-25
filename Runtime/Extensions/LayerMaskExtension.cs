using System;
using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the LayerMask class.
    /// </summary>
    public static class LayerMaskExtension
    {
        /// <summary>
        /// Creates a LayerMask from an array of layer indices.
        /// </summary>
        /// <param name="layerIndices">An array of layer indices.</param>
        /// <returns>A LayerMask representing the specified layers.</returns>
        /// <exception cref="System.ArgumentException">Thrown when a layer index is out of the valid range (0-31).</exception>
        /// <example>
        /// <code>
        /// // Create a LayerMask for layers 0, 2, and 5
        /// LayerMask mask = LayerMaskExtension.CreateLayerMaskFromIndices(0, 2, 5);
        /// </code>
        /// </example>
        public static LayerMask CreateLayerMaskFromIndices(params int[] layerIndices)
        {
            int areaMask = 0;

            foreach (int layerIndex in layerIndices)
            {
                if (layerIndex is >= 0 and < 32)
                {
                    areaMask |= 1 << layerIndex;
                }
                else
                {
                    throw new ArgumentException($"Layer index {layerIndex} is out of range (0-31)");
                }
            }

            return areaMask;
        }

        /// <summary>
        /// Creates a LayerMask from an array of layer names.
        /// </summary>
        /// <param name="layerNames">An array of layer names to convert into a LayerMask.</param>
        /// <returns>A LayerMask representing the specified layers.</returns>
        /// <example>
        /// <code>
        /// // Create a LayerMask from layer names
        /// LayerMask mask = LayerMaskExtension.NamesToLayer("Default", "UI", "Player");
        /// </code>
        /// </example>
        public static LayerMask CreateLayerMaskFromNames(params string[] layerNames)
        {
            int mask = 0;
            foreach (string layer in layerNames)
            {
                int layerIndex = LayerMask.NameToLayer(layer);
                if (layerIndex == -1)
                {
                    nameof(LayerMaskExtension).LogWarning($"Layer '{layer}' does not exist.");
                    continue;
                }

                mask |= 1 << layerIndex;
            }

            return mask;
        }

        /// <summary>
        /// Retrieves the index of a single layer from the LayerMask.
        /// </summary>
        /// <param name="mask">The LayerMask to extract the layer index from.</param>
        /// <returns>
        /// The index of the single layer in the LayerMask.
        /// Returns 0 and logs an error if the LayerMask contains no layers or multiple layers.
        /// </returns>
        /// <example>
        /// <code>
        /// LayerMask mask = LayerMaskExtension.CreateLayerMaskFromIndices(8);
        /// int index = mask.GetSingleLayerIndex();  // index will be 8
        /// </code>
        /// </example>
        public static int GetSingleLayerIndex(this LayerMask mask)
        {
            int bits = mask.value;
            if (bits == 0 || (bits & (bits - 1)) != 0)
            {
                nameof(LayerMaskExtension).LogError("LayerMask must contain exactly one layer.");
                return 0;
            }

            int index = 0;
            while ((bits >>= 1) > 0)
                index++;

            return index;
        }
    }
}