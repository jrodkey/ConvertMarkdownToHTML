namespace ConvertMarkdownToHTML.conversion
{
    public class EmptyTokenConversion : TokenConversion{

    }

    public abstract class TokenConversion
    {
        protected bool m_isParsing;
        protected string m_replaceStart;
        protected string m_replaceEnd;
        protected List<string> m_source;

        public TokenConversion() :
            this(new List<string>(), string.Empty, string.Empty){}

        public TokenConversion(List<string> source, string replaceStart, string replaceEnd)
        {
            m_source = source;
            m_replaceStart = replaceStart;
            m_replaceEnd = replaceEnd;
        }

        public virtual bool Prerequisite(string text)
        {
            foreach (var source in m_source)
            {
                if (text == source)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual string Get()
        {
             m_isParsing = !m_isParsing;
             return (m_isParsing) ? m_replaceStart : m_replaceEnd;
        }
    }
}