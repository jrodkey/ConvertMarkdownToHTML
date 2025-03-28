namespace ConvertMarkdownToHTML.conversion
{
    /// <summary>
    /// Defines an entity stack retriever.
    /// </summary>
    public interface IRetriever<T>
    {
        /// <summary>
        /// Removes from the given name.
        /// </summary>
        /// <param name="name">Name of entry</param>
        public T Remove(string name);

        /// <summary>
        /// Adds the given entry to the Stack.
        /// </summary>
        /// <param name="entry"></param>
        public string Push(T entry);

        /// <summary>
        /// Pops the last entry from the Stack.
        /// </summary>
        /// <returns></returns>
        public T Pop();

        /// <summary>
        /// Retrieves the entry by the given name, but doesn't remove it, starting at the top of the 
        //  stack and going in reverse.
        /// </summary>
        /// <param name="name">Name of the entry</param>
        /// <returns></returns>
        public T ReversePeek(string name);
    }
}