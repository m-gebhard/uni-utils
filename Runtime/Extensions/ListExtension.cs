using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniUtils.Data;
using Random = System.Random;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for lists of floats and generic lists.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Splits a list into smaller chunks of a specified size.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The source list to split into chunks.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>A list of smaller lists, each containing up to <paramref name="chunkSize"/> items.</returns>
        public static List<List<T>> Chunk<T>(this List<T> list, int chunkSize)
        {
            List<List<T>> result = new List<List<T>>();
            for (int i = 0; i < list.Count; i += chunkSize)
            {
                result.Add(list.GetRange(i, Math.Min(chunkSize, list.Count - i)));
            }

            return result;
        }

        /// <summary>
        /// Flattens a list of lists into a single list by concatenating all inner lists.
        /// </summary>
        /// <typeparam name="T">The type of elements in the lists.</typeparam>
        /// <param name="listOfLists">The list of lists to flatten.</param>
        /// <returns>A single list containing all elements from the inner lists.</returns>
        /// <example>
        /// <code>
        /// List&lt;List&lt;int&gt;&gt; nested = new()
        /// {
        ///     new List&lt;int&gt; { 1, 2 },
        ///     new List&lt;int&gt; { 3, 4 },
        ///     new List&lt;int&gt; { 5 }
        /// };
        /// List&lt;int&gt; flat = nested.Flatten();
        /// // flat is { 1, 2, 3, 4, 5 }
        /// </code>
        /// </example>
        public static List<T> Flatten<T>(this List<List<T>> listOfLists)
        {
            return listOfLists.SelectMany(x => x).ToList();
        }

        /// <summary>
        /// Executes a batch operation on a list of items, processing them in groups with optional delays and callbacks.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="itemsList">The list of items to process in batches.</param>
        /// <param name="actionOnItem">The action to execute on each item in the batch.</param>
        /// <param name="itemsPerBatch">The number of items to process in each batch. Default is 5.</param>
        /// <param name="batchDelay">The delay (in seconds) between processing each batch. Default is 0.1f.</param>
        /// <param name="onItemProcessStart">
        /// Optional callback invoked before processing each item; receives the item as <c>T</c>.
        /// </param>
        /// <param name="onItemProcessFinished">
        /// Optional callback invoked after processing each item; receives the item as <c>T</c> and a <c>bool</c> indicating success.
        /// </param>
        /// <param name="onBatchStart">
        /// Optional callback invoked at the start of each batch; receives the current batch as a <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onBatchFinished">
        /// Optional callback invoked at the end of each batch; receives the processed batch as a <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onFinished">
        /// Optional callback invoked after all batches are processed; receives the entire list as a <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onError">
        /// Optional callback invoked when an error occurs during item processing; receives the item that failed (<c>T</c>) and the thrown <c>Exception</c>.
        /// </param>
        /// <returns>An enumerator that can be used to execute the batch operation.</returns>
        /// <example>
        /// <code>
        /// List&lt;int&gt; numbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        /// IEnumerator batch = numbers.ProcessInBatches(
        ///     item =&gt; Debug.Log($&quot;Processing: {item}&quot;),
        ///     itemsPerBatch: 3,
        ///     batchDelay: 0.5f,
        ///     onBatchStart: batchItems =&gt; Debug.Log($&quot;Batch started: {string.Join(&quot;, &quot;, batchItems)}&quot;),
        ///     onBatchFinished: batchItems =&gt; Debug.Log($&quot;Batch finished: {string.Join(&quot;, &quot;, batchItems)}&quot;),
        ///     onFinished: allItems =&gt; Debug.Log(&quot;All batches complete!&quot;)
        /// );
        /// // Use StartCoroutine(batch) in a MonoBehaviour to execute.
        /// </code>
        /// </example>
        public static IEnumerator ProcessInBatches<T>(
            this List<T> itemsList,
            Action<T> actionOnItem,
            int itemsPerBatch = 5,
            float batchDelay = 0.1f,
            Action<T> onItemProcessStart = null,
            Action<T, bool> onItemProcessFinished = null,
            Action<IReadOnlyList<T>> onBatchStart = null,
            Action<IReadOnlyList<T>> onBatchFinished = null,
            Action<IReadOnlyList<T>> onFinished = null,
            Action<T, Exception> onError = null
        )
        {
            return Batcher.ProcessInBatches(
                itemsList,
                actionOnItem,
                itemsPerBatch,
                batchDelay,
                onItemProcessStart,
                onItemProcessFinished,
                onBatchStart,
                onBatchFinished,
                onFinished,
                onError
            );
        }

        /// <summary>
        /// Replaces the first occurrence of a specified item in the list with a new item.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list in which the replacement will occur.</param>
        /// <param name="oldItem">The item to be replaced.</param>
        /// <param name="newItem">The item to replace the old item with.</param>
        /// <returns>The original list with the specified item replaced.</returns>
        /// <example>
        /// <code>
        /// List&lt;string&gt; fruits = new() { "Apple", "Banana", "Cherry" };
        /// fruits.Replace("Banana", "Orange");
        /// // fruits is now { "Apple", "Orange", "Cherry" }
        /// </code>
        /// </example>
        public static List<T> Replace<T>(this List<T> list, T oldItem, T newItem)
        {
            if (list == null || list.Count == 0)
                return list;

            int index = list.IndexOf(oldItem);
            if (index >= 0)
            {
                list[index] = newItem;
            }

            return list;
        }

        /// <summary>
        /// Sorts the elements of the list in place using the specified comparison.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="comparison">The comparison to use for sorting the elements.</param>
        /// <returns>The sorted list.</returns>
        /// <example>
        /// <code>
        /// List&lt;int&gt; numbers = new() { 5, 3, 8, 1 };
        /// numbers.SortInPlace((a, b) =&gt; a.CompareTo(b));
        /// // numbers is now sorted: 1, 3, 5, 8
        /// </code>
        /// </example>
        public static List<T> SortInPlace<T>(this List<T> list, Comparison<T> comparison)
        {
            if (list == null || list.Count <= 1)
            {
                return list;
            }

            list.Sort(comparison);
            return list;
        }

        /// <summary>
        /// Shuffles the elements of a list in place using the Fisher-Yates algorithm.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        /// <example>
        /// <code>
        /// List&lt;string&gt; names = new() { "Alice", "Bob", "Charlie" };
        /// names.Shuffle();
        /// // names order is now randomized, e.g., "Charlie", "Alice", "Bob"
        /// </code>
        /// </example>
        public static List<T> Shuffle<T>(this List<T> list)
        {
            Random rng = new();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            return list;
        }

        /// <summary>
        /// Creates a shallow copy of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to clone.</param>
        /// <returns>A new list containing the same elements as the original list.</returns>
        /// <example>
        /// <code>
        /// List&lt;float&gt; original = new() { 1.0f, 2.0f, 3.0f };
        /// List&lt;float&gt; copy = original.Clone();
        /// // copy contains the same elements as original
        /// </code>
        /// </example>
        public static List<T> Clone<T>(this List<T> list)
        {
            return new List<T>(list);
        }

        /// <summary>
        /// Returns a random element from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select a random element from.</param>
        /// <returns>A random element from the list.</returns>
        /// <exception cref="System.ArgumentException">System.ArgumentException: Thrown when the list is empty.</exception>
        /// <example>
        /// <code>
        /// List&lt;int&gt; values = new() { 10, 20, 30 };
        /// int randomValue = values.Random();
        /// // randomValue is one of 10, 20, or 30
        /// </code>
        /// </example>
        public static T Random<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException($"List<{typeof(T)}> must not be empty.");
            }
            else if (list.Count == 1)
            {
                return list[0];
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a list of unique random elements from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select random elements from.</param>
        /// <param name="count">The number of unique random elements to return.</param>
        /// <returns>A list containing unique random elements from the original list.</returns>
        /// <exception cref="System.ArgumentException">Thrown if count is greater than the list count or if count is negative.</exception>
        /// <example>
        /// <code>
        /// List&lt;int&gt; numbers = new() { 1, 2, 3, 4, 5 };
        /// List&lt;int&gt; randomSubset = numbers.Random(3);
        /// // randomSubset contains 3 unique elements randomly selected from numbers
        /// </code>
        /// </example>
        public static List<T> Random<T>(this List<T> list, int count)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException($"List<{typeof(T)}> must not be empty.");
            }

            if (count < 0 || count > list.Count)
            {
                throw new ArgumentException($"Count must be between 0 and the size of the list ({list.Count}).");
            }

            if (count == 0)
            {
                return new List<T>();
            }

            if (list.Count == 1)
            {
                return new List<T> { list[0] };
            }

            List<T> copy = new List<T>(list);
            Random rng = new();

            int n = copy.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (copy[n], copy[k]) = (copy[k], copy[n]);
            }

            return copy.GetRange(0, count);
        }

        /// <summary>
        /// Returns the next element in the list after the specified item.
        /// If the item is not found, returns the first element if the list is not empty, otherwise returns the default value for the type.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to search.</param>
        /// <param name="item">The item to find the next element of.</param>
        /// <returns>The next element in the list after the specified item, or the first element if the item is not found and the list is not empty, otherwise the default value for the type.</returns>
        /// <example>
        /// <code>
        /// List&lt;string&gt; colors = new() { "Red", "Green", "Blue" };
        /// string nextColor = colors.Next("Green");
        /// // nextColor is "Blue"
        /// string nextOfUnknown = colors.Next("Yellow");
        /// // nextOfUnknown is "Red" (first element) because "Yellow" not found
        /// </code>
        /// </example>
        public static T Next<T>(this List<T> list, T item)
        {
            int index = list.IndexOf(item);

            if (index == -1)
            {
                return list.Count > 0
                    ? list[0]
                    : default;
            }

            int nextIndex = (index + 1) % list.Count;
            return list[nextIndex];
        }

        /// <summary>
        /// Linearly interpolates between elements in a list of floats based on a parameter t.
        /// </summary>
        /// <param name="list">The list of floats to interpolate.</param>
        /// <param name="t">The interpolation parameter, typically between 0 and 1.</param>
        /// <returns>The interpolated float value.</returns>
        /// <exception cref="System.ArgumentException">System.ArgumentException: Thrown when the list is empty.</exception>
        /// <example>
        /// <code>
        /// List&lt;float&gt; points = new() { 0f, 10f, 20f };
        /// float value = points.Lerp(0.25f);
        /// // value is interpolated between 0 and 10, roughly 2.5
        /// </code>
        /// </example>
        public static float Lerp(this List<float> list, float t)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException("List<float> must not be empty.");
            }
            else if (list.Count == 1)
            {
                return list[0];
            }

            int startIndex = (int)(t * (list.Count - 1));
            int endIndex = startIndex + 1;

            float startValue = list[startIndex];
            float endValue = list[endIndex];

            float fraction = (t - (float)startIndex / (list.Count - 1)) * (list.Count - 1);

            return Mathf.Lerp(startValue, endValue, fraction);
        }


        /// <summary>
        /// Finds the nearest transform in the list to the specified transform.
        /// </summary>
        /// <param name="transformList">The list of transforms to search.</param>
        /// <param name="compareTransform">The transform to compare against.</param>
        /// <returns>The nearest transform to the specified transform.</returns>
        /// <example>
        /// <code>
        /// List&lt;Transform&gt; enemies = new List&lt;Transform&gt;();
        /// Transform player = someGameObject.transform;
        /// Transform nearestEnemy = enemies.Nearest(player);
        /// // nearestEnemy is the closest enemy transform to the player
        /// </code>
        /// </example>
        public static Transform Nearest(this List<Transform> transformList, Transform compareTransform)
        {
            Transform nearest = null;
            float shortest = float.MaxValue;

            foreach (Transform t in transformList)
            {
                float dist = Vector3.Distance(compareTransform.position, t.position);

                if (dist < shortest)
                {
                    shortest = dist;
                    nearest = t;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Finds the furthest transform in the list from the specified transform.
        /// </summary>
        /// <param name="transformList">The list of transforms to search.</param>
        /// <param name="compareTransform">The transform to compare against.</param>
        /// <returns>The furthest transform from the specified transform.</returns>
        /// <example>
        /// <code>
        /// List&lt;Transform&gt; enemies = new List&lt;Transform&gt;();
        /// Transform player = someGameObject.transform;
        /// Transform furthestEnemy = enemies.Furthest(player);
        /// // furthestEnemy is the furthest enemy transform from the player
        /// </code>
        /// </example>
        public static Transform Furthest(this List<Transform> transformList, Transform compareTransform)
        {
            Transform furthest = null;
            float furthestDist = 0;

            foreach (Transform t in transformList)
            {
                float dist = Vector3.Distance(compareTransform.position, t.position);

                if (dist > furthestDist)
                {
                    furthestDist = dist;
                    furthest = t;
                }
            }

            return furthest;
        }

        /// <summary>
        /// Calculates the center point of a list of Vector3s.
        /// </summary>
        /// <param name="list">The list of Vector3s to calculate the center point of.</param>
        /// <returns>The center point of the list of Vector3s.</returns>
        /// <example>
        /// <code>
        /// List&lt;Vector3&gt; positions = new()
        /// {
        ///     new Vector3(1, 0, 0),
        ///     new Vector3(0, 1, 0),
        ///     new Vector3(0, 0, 1)
        /// };
        /// Vector3 centerPoint = positions.Center();
        /// // centerPoint is approximately (0.33, 0.33, 0.33)
        /// </code>
        /// </example>
        public static Vector3 Center(this List<Vector3> list)
        {
            Vector3 average = list.Aggregate(Vector3.zero, (current, v) => current + v);
            average /= list.Count;

            return average;
        }
    }
}