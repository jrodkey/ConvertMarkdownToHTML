
using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.converters;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.converters
{
    public class HTMLConverter : TextConverter
    {
        private const string SupportConversion = @"[_*#!\n\r]";
        private Regex m_regex;

        /// <summary>
        /// 
        /// </summary>
        public HTMLConverter() :
            base()
        {
            m_regex = new Regex(SupportConversion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, TokenConversion> GetConversions()
        {
            return new Dictionary<string, TokenConversion>()
            {
                { "#",   new TokenConversion("<h1>", "</h1>") },
                { "##",  new TokenConversion("<h2>", "</h2>") },
                { "###", new TokenConversion("<h3>", "</h3>") },
                { "__",  new TokenConversion("<strong>", "</strong>") },
                { "_",   new TokenConversion("<em>", "</em>") },
                { "*",   new TokenConversion("<em>", "</em>") },
                { "**",  new TokenConversion("<em>", "</em>") },
                { "\n",  new TokenConversion("<p>", "</p>") },
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

            (string, int) startingResult = StartingToken(text, m_conversions["\n"]);
            stringBuilder.Append(startingResult.Item1);
            for (int i = startingResult.Item2; i < text.Length; ++i)
            {
                (string, int) results = ReplaceToken(i, text);
                if (results.Item2 > 1)
                {
                    i += Math.Max(results.Item2 - 1, 0);
                }

                stringBuilder.Append(results.Item1);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        private bool IsValidMatch(char character, string customRegex = SupportConversion)
        {
            if (customRegex.Equals(SupportConversion))
            {
                return m_regex.IsMatch(character.ToString());
            }

            Regex regex = new Regex(customRegex);
            return regex.IsMatch(character.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private (string, int) StartingToken(ReadOnlySpan<char> text, TokenConversion conversion)
        {
            return ReplaceToken(0, text, conversion, @"[#\n\r]");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private (string, int) ReplaceToken(int index, ReadOnlySpan<char> text, TokenConversion? conversion, string customRegex)
        {
            string newToken = text[index].ToString();
            int replacementLength = 1;

            if (IsValidMatch(text[index], customRegex))
            {
                string replacementString = GetTokenReplacement(index, text);
                newToken = Replace(replacementString);
                replacementLength = replacementString.Length;
            }
            else if (conversion != null)
            {
                newToken = conversion.Get();
                replacementLength = 0;
            }

            return (newToken, replacementLength);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private (string, int) ReplaceToken(int index, ReadOnlySpan<char> text, TokenConversion? conversion = null)
        {
            return ReplaceToken(index, text, conversion, SupportConversion);
        }
    }
}