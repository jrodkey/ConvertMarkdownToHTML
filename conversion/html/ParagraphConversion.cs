using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.conversion
{
    public class ParagraphConversion : TokenConversion
    {
        public ParagraphConversion()
        {
            m_replaceStart = "<p>";
            m_replaceEnd = "</p>";
        }
    }
}