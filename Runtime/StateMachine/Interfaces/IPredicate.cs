namespace UniUtils.StateMachine
{
    /// <summary>
    /// Represents a predicate that can be evaluated to determine a boolean condition.
    /// </summary>
    public interface IPredicate
    {
        public bool Evaluate();
    }
}