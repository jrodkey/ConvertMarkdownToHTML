using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters.configuration
{
    /// <summary>
    /// Defines the configuration of the Markdown to HTML converter.
    /// </summary>
    public class HTMLConverterConfiguration : IConverterConfiguration<TokenConversion>
    {
        private TokenConversionRetriever m_conversionRetievers;

        public HTMLConverterConfiguration()
        {
            m_conversionRetievers = new TokenConversionRetriever(GetConversions(), NewLineReplacementSet);
        }

        /// <summary>
        /// Gets the paragraph character.
        /// </summary>
        public string Paragraph { get => "$"; }

        /// <summary>
        /// Gets the ampersand character.
        /// </summary>
        public string Ampersand { get => "&"; }

        /// <summary>
        /// Gets the copy character.
        /// </summary>
        public string Copy { get => "&copy;"; }

        /// <summary>
        /// Gets the new line character.
        /// </summary>
        public string LineBreak { get => "\n"; }

        /// <summary>
        /// Gets the new line regex set.
        /// </summary>
        public string NewLineSet { get => @"[\n\r]"; }

        /// <summary>
        /// Gets the valid characters for a new line replacement regex set.
        /// </summary>
        public string NewLineReplacementSet { get => @"[#]"; }

        /// <summary>
        /// Gets the valid characters regex set.
        /// </summary>
        public string ValidCharacterSet { get => @"[A-Za-z]"; }

        /// <summary>
        /// Gets the supported conversions regex set.
        /// </summary>
        public string SupportedConversionSet { get => @"[_*#<\n\r]"; }

        /// <summary>
        /// Gets the valid characters for a backtrace regex pattern.
        /// </summary>
        public string ValidBacktracePattern { get => $"({ValidCharacterSet})|({Copy})"; }

        /// <summary>
        /// Gets the supported generalized regex pattern.
        /// </summary>
        public string GeneralConversionsPattern { get => $"({SupportedConversionSet})|({Copy})|({Ampersand})"; }

        /// <summary>
        /// Gets the token conversion retriever.
        /// </summary>
        public TokenConversionRetriever Retriever { get { return m_conversionRetievers; } }

        /// <summary>
        /// Gets the conversions used for the token retriever.
        /// </summary>
        public Dictionary<string, TokenConversion> GetConversions()
        {
            return new Dictionary<string, TokenConversion>()
            {
                { "#",      new TokenConversion("#", "<h1>", "</h1>") },
                { "##",     new TokenConversion("##", "<h2>", "</h2>") },
                { "###",    new TokenConversion("###", "<h3>", "</h3>") },
                { "__",     new TokenConversion("__", "<strong>", "</strong>") },
                { "_",      new TokenConversion("_", "<em>", "</em>") },
                { "*",      new TokenConversion("*", "<em>", "</em>") },
                { "**",     new TokenConversion("**", "<strong>", "</strong>") },
                { "::",     new TokenConversion("::", "<p>", "</p>") },
                { "$",      new TokenConversion("$", "<p>", "</p>") },
                { "&",      new TokenConversion("&", "&amp;") },
                { "&copy;", new TokenConversion("&copy;", "&copy;") },
                { "<",      new TokenConversion("<", "&lt;") },
            };
        }
    }
}