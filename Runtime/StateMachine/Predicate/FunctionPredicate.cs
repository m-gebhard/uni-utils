using System;

namespace UniUtils.FSM
{
    /// <summary>
    /// A concrete implementation of <see cref="IPredicate"/> that uses a function delegate to evaluate a condition.
    /// </summary>
    /// <example>
    /// Example usage:
    /// <code>
    /// bool isHealthLow = false;
    /// IPredicate lowHealthPredicate = new FunctionPredicate(() =&gt; isHealthLow);
    ///
    /// if (lowHealthPredicate.Evaluate())
    /// {
    ///     Debug.Log("Health is low!");
    /// }
    /// </code>
    /// </example>
    public class FunctionPredicate : IPredicate
    {
        private readonly Func<bool> predicate;

        public FunctionPredicate(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Evaluate() => predicate();
    }
}