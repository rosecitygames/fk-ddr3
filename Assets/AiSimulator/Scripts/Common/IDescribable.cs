namespace IndieDevTools
{
    /// <summary>
    /// An interface used to describe objects.
    /// </summary>
    public interface IDescribable : IUpdatable<IDescribable>
    {
        string DisplayName { get; set; }
        string Description { get; set; }
    }
}