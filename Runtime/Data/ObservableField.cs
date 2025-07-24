using System;
using System.Collections.Generic;

namespace UniUtils.Data
{
    /// <summary>
    /// Represents an observable field that notifies subscribers when its value changes.
    /// </summary>
    /// <example>
    /// <typeparam name="T">The type of the value being observed.</typeparam>
    /// <code>
    /// ObservableField&lt;int&gt; observableInt = new ObservableField&lt;int&gt;(10);
    /// observableInt.OnChange += newValue =&gt; Debug.Log($"Value changed to {newValue}");
    /// 
    /// observableInt.Value = 20; // Triggers OnChange and outputs: Value changed to 20
    /// observableInt.Value = 20; // No event triggered because value is the same
    /// </code>
    /// </example>
    public class ObservableField<T>
    {
        /// <summary>
        /// The current value of the observable field.
        /// </summary>
        private T observeValue;

        /// <summary>
        /// Action to be invoked when the value changes.
        /// </summary>
        public event Action<T> OnChange;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableField{T}"/> class with an optional initial value.
        /// </summary>
        /// <param name="initialValue">The initial value of the observable field.</param>
        public ObservableField(T initialValue = default)
        {
            observeValue = initialValue;
        }

        /// <summary>
        /// Gets or sets the value of the observable field.
        /// </summary>
        public T Value
        {
            get => observeValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, observeValue)) return;

                observeValue = value;
                OnChange?.Invoke(value);
            }
        }
    }
}