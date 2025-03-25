namespace ConvertMarkdownToHTML.conversion
{
    /// <summary>
    /// 
    /// </summary>
    public class EmptyTokenConversion : TokenConversion { }

    /// <summary>
    /// 
    /// </summary>
    public class TokenConversion
    {
        protected bool m_isParsing;
        protected string m_replaceStart;
        protected string m_replaceEnd;
        protected List<string> m_source;

        public TokenConversion() :
            this(string.Empty, string.Empty)
        { }

        public TokenConversion(string replaceStart, string replaceEnd)
        {
            m_replaceStart = replaceStart;
            m_replaceEnd = replaceEnd;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsParsing
        {
            get { return m_isParsing; }
            set { m_isParsing = value; }
        }

        public string Get()
        {
            m_isParsing = !m_isParsing;
            return (m_isParsing) ? m_replaceStart : m_replaceEnd;
        }
    }
}