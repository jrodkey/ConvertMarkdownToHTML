namespace ConvertMarkdownToHTML.converters.configuration
{
    public interface IConverterConfiguration<T>
    {
        Dictionary<string, T> GetConversions();
    }
}