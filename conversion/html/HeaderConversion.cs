using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.conversion
{
    public class HeaderConversion : TokenConversion
    {
        public HeaderConversion()
        {
            m_source = new List<string>{ "#" };
            m_replaceStart = "<h1>";
            m_replaceEnd = "</h1>";
        }
    }
}