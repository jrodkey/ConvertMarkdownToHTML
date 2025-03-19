namespace ConvertMarkdownToHTML.conversion
{
    public interface ITokenConversion
    {
        string Get(char character);
    }

    public abstract class TokenConversion
    {
        public bool IsParsing { get; set; } = false;

        public virtual string Get(char character)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyTokenConversion : TokenConversion{

    }
}