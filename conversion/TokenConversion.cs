namespace ConvertMarkdownToHTML.conversion
{
    /// <summary>
    /// Defines a source with its converted counterpart.
    /// </summary>
    public class TokenConversion
    {
        /// <summary>
        /// Defined as an empty token conversion.
        /// </summary>
        public static TokenConversion EmptyTokenConversion = new TokenConversion();

        protected bool m_parsingStatus;
        protected string m_replaceStart = string.Empty;
        protected string m_replaceEnd = string.Empty;
        protected string m_source = string.Empty;

        public TokenConversion(TokenConversion conversion) :
            this (conversion.Source, conversion.Start, conversion.End)
        {}

        public TokenConversion() :
            this(string.Empty, string.Empty, string.Empty)
        { }

        public TokenConversion(string source, string replaceStart) :
            this(source, replaceStart, string.Empty)
        { }

        public TokenConversion(string source, string replaceStart, string replaceEnd)
        {
            m_source = source;
            m_replaceStart = replaceStart;
            m_replaceEnd = replaceEnd;
            m_parsingStatus = true;
        }

        /// <summary>
        /// Gets or sets the flag of the status value.
        /// </summary>
        public bool ParsingStatus
        {
            get { return m_parsingStatus; }
            set { m_parsingStatus = value; }
        }

        /// <summary>
        /// Gets the source value.
        /// </summary>
        public string Source
        {
            get { return m_source; }
        }

        /// <summary>
        /// Gets the start replace value.
        /// </summary>
        public string Start
        {
            get { return m_replaceStart; }
        }

        /// <summary>
        /// Gets the end replace value.
        /// </summary>
        public string End
        {
            get { return m_replaceEnd; }
        }

        /// <summary>
        /// Gets the current value and advances to the next.
        /// </summary>
        /// <returns></returns>
        public string Get()
        {
            if (string.IsNullOrEmpty(m_replaceEnd))
            {
                return m_replaceStart;
            }

            return (m_parsingStatus) ? m_replaceStart : m_replaceEnd;
        }
    }
}