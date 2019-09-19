namespace IndieDevTools  
{
    /// <summary>
    /// A generic interface for copying.
    /// </summary>
    /// <typeparam name="T">The type of object you want to copy</typeparam>
    public interface ICopyable<T>
    {
        T Copy();
    }
}

