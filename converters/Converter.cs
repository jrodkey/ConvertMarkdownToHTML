using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public interface IConverter
    {
        object Convert(object value);
    }

    public abstract class Converter : IConverter
    {
        protected Dictionary<char, TokenConversion> Conversions { get; set;} = new Dictionary<char, TokenConversion>();

        public virtual object Convert(object value)
        {
            throw new NotImplementedException();
        }

        public TokenConversion GetTokenConversion(char token)
        {
            if (Conversions.ContainsKey(token))
            {
                return Conversions[token];
            }

            return new EmptyTokenConversion();
        }
    }
}