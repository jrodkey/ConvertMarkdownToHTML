
namespace ConvertMarkdownToHTML.converters
{
    public interface IConverter<T, TokenConversion> where T : class 
    {
        T Convert(T value);

        T Convert(T[] value);

        Dictionary<T, TokenConversion> GetConversions();
    }
}