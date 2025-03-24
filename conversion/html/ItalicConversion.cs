using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.conversion
{
    public class ItalicConversion : TokenConversion
    {
        public ItalicConversion()
        {
            m_source = new List<string>{ "_", "*" };
            m_replaceStart = "<em>";
            m_replaceEnd = "</em>";
        }
    }
}