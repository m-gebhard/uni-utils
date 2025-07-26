using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniUtils.Extensions;

namespace UniUtils.Data
{
    /// <summary>
    /// Provides utility methods for processing items in batches with support for callbacks and error handling.
    /// </summary>
    public static class Batcher
    {
        /// <summary>
        /// Executes an action on a list of items in batches, with optional callbacks for various stages of processing.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="itemsList">The list of items to process.</param>
        /// <param name="actionOnItem">The action to execute on each item.</param>
        /// <param name="itemsPerBatch">The number of items to process per batch. Defaults to 5.</param>
        /// <param name="batchDelay">The delay (in seconds) between processing batches. Defaults to 0.1f.</param>
        /// <param name="onItemProcessStart">
        /// Optional callback invoked before processing each item;
        /// receives the item as <c>T</c>.
        /// </param>
        /// <param name="onItemProcessFinished">
        /// Optional callback invoked after processing each item;
        /// receives the item as <c>T</c> and a <c>bool</c> indicating success.
        /// </param>
        /// <param name="onBatchStart">
        /// Optional callback invoked before processing each batch;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onBatchFinished">
        /// Optional callback invoked when a batch is completed;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onFinished">
        /// Optional callback invoked when all items have been processed;
        /// receives the full list of processed items as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onError">
        /// Optional callback invoked when an error occurs during item processing;
        /// receives the item that failed (<c>T</c>) and the thrown <c>Exception</c>.
        /// </param>
        /// <returns>An enumerator that can be used to execute the batches over time.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="actionOnItem"/> is null.</exception>
        /// <example>
        /// <code>
        /// IReadOnlyList&lt;int&gt; numbers = new IReadOnlyList&lt;int&gt;() { 1, 2, 3, 4, 5, 6, 7 };
        ///
        /// StartCoroutine(Batcher.ProcessInBatches(
        ///     numbers,
        ///     n =&gt; Debug.Log($&quot;Processing: {n}&quot;),
        ///     itemsPerBatch: 3,
        ///     batchDelay: 0.5f,
        ///     onBatchStart: batch =&gt; Debug.Log($&quot;Batch starting: {string.Join(&quot;, &quot;, batch)}&quot;),
        ///     onBatchFinished: batch =&gt; Debug.Log($&quot;Batch finished: {string.Join(&quot;, &quot;, batch)}&quot;),
        ///     onFinished: all =&gt; Debug.Log(&quot;All items processed.&quot;)
        /// ));
        /// </code>
        /// </example>
        public static IEnumerator ProcessInBatches<T>(
            IReadOnlyList<T> itemsList,
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
            if (actionOnItem == null)
                throw new ArgumentNullException(nameof(actionOnItem), "Action on item cannot be null.");

            if (itemsList == null || itemsList.Count == 0) yield break;

            IReadOnlyList<IReadOnlyList<T>> preBatchedLists = itemsList
                .ToList()
                .Chunk(itemsPerBatch);

            yield return ProcessBatches(
                preBatchedLists,
                actionOnItem,
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
        /// Executes an action on pre-batched lists of items, with optional callbacks for various stages of processing.
        /// </summary>
        /// <typeparam name="T">The type of items in the batches.</typeparam>
        /// <param name="preBatchedLists">The pre-batched lists of items to process.</param>
        /// <param name="actionOnItem">The action to execute on each item.</param>
        /// <param name="batchDelay">The delay (in seconds) between processing batches. Defaults to 0.1f.</param>
        /// <param name="onItemProcessStart">
        /// Optional callback invoked before processing each item;
        /// receives the item as <c>T</c>.
        /// </param>
        /// <param name="onItemProcessFinished">
        /// Optional callback invoked after processing each item;
        /// receives the item as <c>T</c> and a <c>bool</c> indicating success.
        /// </param>
        /// <param name="onBatchStart">
        /// Optional callback invoked before processing each batch;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onBatchFinished">
        /// Optional callback invoked when a batch is completed;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onFinished">
        /// Optional callback invoked when all items have been processed;
        /// receives the full list of processed items as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onError">
        /// Optional callback invoked when an error occurs during item processing;
        /// receives the item that failed (<c>T</c>) and the thrown <c>Exception</c>.
        /// </param>
        /// <returns>An enumerator that can be used to execute the batches over time.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="actionOnItem"/> is null.</exception>
        /// <example>
        /// <code>
        /// IReadOnlyList&lt;IReadOnlyList&lt;int&gt;&gt; batches = new IReadOnlyList&lt;IReadOnlyList&lt;int&gt;&gt;()
        /// {
        ///     new IReadOnlyList&lt;int&gt;() { 1, 2, 3 },
        ///     new IReadOnlyList&lt;int&gt;() { 4, 5, 6 },
        ///     new IReadOnlyList&lt;int&gt;() { 7 }
        /// };
        ///
        /// StartCoroutine(Batcher.ProcessPreBatched(
        ///     batches,
        ///     n =&gt; Debug.Log($&quot;Processing: {n}&quot;),
        ///     batchDelay: 0.5f,
        ///     onBatchStart: batch =&gt; Debug.Log($&quot;Batch starting: {string.Join(&quot;, &quot;, batch)}&quot;),
        ///     onBatchFinished: batch =&gt; Debug.Log($&quot;Batch finished: {string.Join(&quot;, &quot;, batch)}&quot;),
        ///     onFinished: all =&gt; Debug.Log(&quot;All items processed.&quot;)
        /// ));
        /// </code>
        /// </example>
        public static IEnumerator ProcessPreBatched<T>(
            IReadOnlyList<IReadOnlyList<T>> preBatchedLists,
            Action<T> actionOnItem,
            float batchDelay = 0.1f,
            Action<T> onItemProcessStart = null,
            Action<T, bool> onItemProcessFinished = null,
            Action<IReadOnlyList<T>> onBatchStart = null,
            Action<IReadOnlyList<T>> onBatchFinished = null,
            Action<IReadOnlyList<T>> onFinished = null,
            Action<T, Exception> onError = null
        )
        {
            if (actionOnItem == null)
                throw new ArgumentNullException(nameof(actionOnItem), "Action on item cannot be null.");

            yield return ProcessBatches(
                preBatchedLists,
                actionOnItem,
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
        /// Executes an action on a series of batches, with optional callbacks for various stages of processing.
        /// </summary>
        /// <typeparam name="T">The type of items in the batches.</typeparam>
        /// <param name="actionOnItem">The action to execute on each item.</param>
        /// <param name="batches">The list of batches to process.</param>
        /// <param name="batchDelay">The delay (in seconds) between processing batches.</param>
        /// <param name="onItemProcessStart">
        /// Optional callback invoked before processing each item;
        /// receives the item as <c>T</c>.
        /// </param>
        /// <param name="onItemProcessFinished">
        /// Optional callback invoked after processing each item;
        /// receives the item as <c>T</c> and a <c>bool</c> indicating success.
        /// </param>
        /// <param name="onBatchStart">
        /// Optional callback invoked before processing each batch;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onBatchFinished">
        /// Optional callback invoked when a batch is completed;
        /// receives the batch as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onFinished">
        /// Optional callback invoked when all items have been processed;
        /// receives the full list of processed items as <c>IReadOnlyList&lt;T&gt;</c>.
        /// </param>
        /// <param name="onError">
        /// Optional callback invoked when an error occurs during item processing;
        /// receives the item that failed (<c>T</c>) and the thrown <c>Exception</c>.
        /// </param>
        /// <returns>An enumerator that can be used to execute the batches over time.</returns>
        private static IEnumerator ProcessBatches<T>(
            IReadOnlyList<IReadOnlyList<T>> batches,
            Action<T> actionOnItem,
            float batchDelay,
            Action<T> onItemProcessStart,
            Action<T, bool> onItemProcessFinished,
            Action<IReadOnlyList<T>> onBatchStart,
            Action<IReadOnlyList<T>> onBatchFinished,
            Action<IReadOnlyList<T>> onFinished,
            Action<T, Exception> onError
        )
        {
            if (batches == null || batches.Count == 0) yield break;

            List<T> allProcessedItems = null;
            if (onFinished != null)
            {
                allProcessedItems = new List<T>();
            }

            foreach (IReadOnlyList<T> batch in batches)
            {
                onBatchStart?.Invoke(batch);

                foreach (T item in batch)
                {
                    bool wasProcessed = ProcessItem(
                        item,
                        actionOnItem,
                        onItemProcessStart,
                        onItemProcessFinished,
                        onError
                    );

                    if (wasProcessed)
                    {
                        allProcessedItems?.Add(item);
                    }
                }

                onBatchFinished?.Invoke(batch);

                if (batchDelay > 0f)
                {
                    yield return WaitHelper.WaitForSeconds(batchDelay);
                }
            }

            onFinished?.Invoke(allProcessedItems);
        }

        /// <summary>
        /// Processes a single item by executing the provided action and handling any errors that occur.
        /// </summary>
        /// <typeparam name="T">The type of the item to process.</typeparam>
        /// <param name="item">The item to process.</param>
        /// <param name="actionOnItem">The action to execute on the item.</param>
        /// <param name="onItemProcessStart">
        /// Optional callback invoked before processing the item; receives the item as <c>T</c>.
        /// </param>
        /// <param name="onItemProcessFinished">
        /// Optional callback invoked after processing the item; receives the item as <c>T</c> and a <c>bool</c> indicating success.
        /// </param>
        /// <param name="onError">
        /// Optional callback invoked when an error occurs during item processing; receives the item that failed (<c>T</c>) and the thrown <c>Exception</c>.
        /// </param>
        /// <returns><c>true</c> if the item was successfully processed; otherwise, <c>false</c>.</returns>
        private static bool ProcessItem<T>(
            T item,
            Action<T> actionOnItem,
            Action<T> onItemProcessStart,
            Action<T, bool> onItemProcessFinished,
            Action<T, Exception> onError
        )
        {
            onItemProcessStart?.Invoke(item);
            bool isProcessed = false;

            try
            {
                actionOnItem(item);
                isProcessed = true;
            }
            catch (Exception e)
            {
                onError?.Invoke(item, e);
            }

            onItemProcessFinished?.Invoke(item, isProcessed);

            return isProcessed;
        }
    }
}