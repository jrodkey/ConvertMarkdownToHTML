using ConvertMarkdownToHTML.converters;

namespace ConvertMarkdownToHTML.conversion
{
    public class TextConversionCache
    {
        public static EmptyTokenConversion Empty = new EmptyTokenConversion();

        private List<TokenConversion> m_conversions;

        private TextConversionCache()
        {
            m_conversions = new List<TokenConversion>();
        }

        private static Lazy<TextConversionCache> m_lazyInstance = new Lazy<TextConversionCache>(() => new TextConversionCache());
        public static TextConversionCache Instance { get { return m_lazyInstance.Value;}}

        public void SetConverter(TextConverter textConverter)
        {
            m_conversions.Clear();
            m_conversions = textConverter.GetConversions();
        }

        public string Replace(string name)
        {
            foreach (var conversion in m_conversions)
            {
                if (conversion.Prerequisite(name))
                {
                    return conversion.Get();
                }
            }

            return string.Empty;
        }
    }
}