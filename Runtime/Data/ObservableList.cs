using System;
using System.Collections;
using System.Collections.Generic;

namespace UniUtils.Data
{
    /// <summary>
    /// Represents a list that notifies subscribers when items are added or removed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <example>
    /// <code>
    /// ObservableList&lt;string&gt; observableList = new ObservableList&lt;string&gt;();
    /// observableList.OnAdd += item =&gt; Debug.Log($"Added: {item}");
    /// observableList.OnRemove += item =&gt; Debug.Log($"Removed: {item}");
    /// 
    /// observableList.Add("Hello");  // Output: Added: Hello
    /// observableList.Add("World");  // Output: Added: World
    /// observableList.Remove("Hello");  // Output: Removed: Hello
    /// observableList.Clear();  // Output: Removed: World
    /// </code>
    /// </example>
    public class ObservableList<T> : IEnumerable<T>
    {
        /// <summary>
        /// The underlying list storing the elements.
        /// </summary>
        private readonly List<T> list = new();

        /// <summary>
        /// Event triggered when an item is added to the list.
        /// </summary>
        public event Action<T> OnAdd;

        /// <summary>
        /// Event triggered when an item is removed from the list.
        /// </summary>
        public event Action<T> OnRemove;

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index] => list[index];

        /// <summary>
        /// Gets the number of elements contained in the list.
        /// </summary>
        public int Count => list.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator for the list.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds an item to the list and triggers the OnAdd event.
        /// </summary>
        /// <param name="item">The item to add to the list.</param>
        public void Add(T item)
        {
            list.Add(item);
            OnAdd?.Invoke(item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the list and triggers the OnRemove event.
        /// </summary>
        /// <param name="item">The item to remove from the list.</param>
        /// <returns>true if the item is successfully removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            bool removed = list.Remove(item);
            if (removed) OnRemove?.Invoke(item);

            return removed;
        }

        /// <summary>
        /// Removes all items from the list and triggers the OnRemove event for each item.
        /// </summary>
        public void Clear()
        {
            foreach (T item in list)
            {
                OnRemove?.Invoke(item);
            }

            list.Clear();
        }
    }
}