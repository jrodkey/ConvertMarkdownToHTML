using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.conversion
{
    public class ItalicConversion : TokenConversion
    {
        public ItalicConversion()
        {
            m_replaceStart = "<em>";
            m_replaceEnd = "</em>";
        }
    }
}