
using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.converters;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.converters
{
    public class HTMLConverter : TextConverter
    {
        // Custom Regex patterns
        private const string NewlinePattern = @"[\n\r]";
        private const string NewlineReplacementPattern = @"[#]";
        private const string SupportedConversionsPattern = @"[_*#\n\r]";

        /// <summary>
        /// 
        /// </summary>
        public HTMLConverter() :
            base()
        {
            m_supportedConversions = SupportedConversionsPattern;
            m_conversions = new Dictionary<string, TokenConversion>()
            {
                { "#",   new TokenConversion("<h1>", "</h1>") },
                { "##",  new TokenConversion("<h2>", "</h2>") },
                { "###", new TokenConversion("<h3>", "</h3>") },
                { "__",  new TokenConversion("<strong>", "</strong>") },
                { "_",   new TokenConversion("<em>", "</em>") },
                { "*",   new TokenConversion("<em>", "</em>") },
                { "**",  new TokenConversion("<em>", "</em>") },
                { "::",  new TokenConversion("<p>", "</p>") },
                { "$",   new TokenConversion("<p>", "</p>") },
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Convert(string value)
        {
            return ConvertFromText((ReadOnlySpan<char>)value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Convert(string[] value)
        {
            return ConvertFromText((ReadOnlySpan<char>)string.Join(" ", value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ConvertFromText(ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (text == null || text.Length == 0)
            {
                return string.Empty;
            }

            (string, int) results = GetNewlineReplacement(0, text); ;
            stringBuilder.Append(results.Item1);
            for (int i = results.Item2; i < text.Length; ++i)
            {
                results = (IsNewline(i, text)) ? GetNewline(i, text) : GetTextReplacement(i, text);
                if (results.Item2 > 1)
                {
                    i += results.Item2 - 1;
                }

                stringBuilder.Append(results.Item1);
            }

            stringBuilder.Append(EOL());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="isCRLF"></param>
        /// <returns></returns>
        private (string, int) GetNewline(int index, ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            (string, int) results = GetTextReplacement(index, text, m_conversions["$"]);
            stringBuilder.Append(results.Item1);

            index += 3;

            results = GetNewlineReplacement(index, text);
            stringBuilder.Append(results.Item1);
            return (stringBuilder.ToString(), 4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="isCRLF"></param>
        /// <returns></returns>
        private (string, int) GetNewlineReplacement(int index, ReadOnlySpan<char> text)
        {
            return (text.Length > index && IsValidMatch(text[index], NewlineReplacementPattern)) ?
                GetTextReplacement(index, text) : GetTextReplacement(index, text, m_conversions["$"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="isCRLF"></param>
        /// <returns></returns>
        private bool IsNewline(int index, ReadOnlySpan<char> text, bool isCRLF = true)
        {
            if (isCRLF)
            {
                return (text.Length > index + 3 &&
                    IsValidMatch(text[index], NewlinePattern) &&
                    IsValidMatch(text[index + 1], NewlinePattern) &&
                    IsValidMatch(text[index + 2], NewlinePattern) &&
                    IsValidMatch(text[index + 3], NewlinePattern));
            }

            return (text.Length > index + 1 &&
                IsValidMatch(text[index], NewlinePattern) &&
                IsValidMatch(text[index + 1], NewlinePattern));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string EOL()
        {
            return Replace("$");
        }
    }
}