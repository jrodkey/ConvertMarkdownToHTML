using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public interface IConverter<T, TokenConversion> where T : class 
    {
        T Convert(T value);

        T Convert(T[] value);

        List<TokenConversion> GetConversions();
    }
}