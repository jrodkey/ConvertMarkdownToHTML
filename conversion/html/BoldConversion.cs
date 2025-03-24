using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.conversion
{
    public class BoldConversion : TokenConversion
    {
        public BoldConversion()
        {
            m_source = new List<string>{ "__" };
            m_replaceStart = "<strong>";
            m_replaceEnd = "</strong>";
        }
    }
}