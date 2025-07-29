namespace UniUtils.FSM
{
    /// <summary>
    /// Represents a predicate that negates the result of another predicate.
    /// </summary>
    public class NotPredicate : IPredicate
    {
        private readonly IPredicate original;

        public NotPredicate(IPredicate original)
        {
            this.original = original;
        }

        /// <summary>
        /// Evaluates the predicate by negating the result of the original predicate.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the original predicate evaluates to <c>false</c>; otherwise, <c>false</c>.
        /// </returns>
        public bool Evaluate() => !original.Evaluate();
    }
}