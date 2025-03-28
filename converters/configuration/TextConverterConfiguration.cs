using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters.configuration
{
    /// <summary>
    /// Spacing between a line and multiple lines.
    /// </summary>
    public enum LineSpacing
    {
        None = 0,
        Single,
        Multiline
    }

    /// <summary>
    /// Defines the configuration of the text converter.
    /// </summary>
    public class TextConverterConfiguration : IConverterConfiguration<TokenConversion>
    {
        private TokenConversionRetriever m_conversionRetievers;

        public TextConverterConfiguration()
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
        /// Gets the new line set.
        /// </summary>
        public string NewLineSet { get => @"[\n\r]"; }

        /// <summary>
        /// Gets the valid characters for a new line replacement pattern.
        /// </summary>
        public string NewLineReplacementSet { get => @"[#;]"; }

        /// <summary>
        /// Gets the supported conversions set.
        /// </summary>
        public string SupportedConversionSet { get => @"([_*#<\n\r])"; }

        /// <summary>
        /// Gets the supported generalized pattern.
        /// </summary>
        public string GeneralConversionsPattern { get => $"{SupportedConversionSet}|({Copy})|({Ampersand})"; }

        /// <summary>
        /// Gets the token conversion retriever.
        /// </summary>
        public TokenConversionRetriever Retriever { get { return m_conversionRetievers; } }

        /// <summary>
        /// Gets the supported conversions for the specified file.
        /// </summary>
        /// <returns></returns>
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