
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.converters;
using ConvertMarkdownToHTML.conversion;
using ConvertMarkdownToHTML.html.conversion;

namespace ConvertMarkdownToHTML.html.converters
{
    public class HTMLConverter : TextConverter
    {
        private string m_supportConversion = @"[&#_*!]";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<TokenConversion> GetConversions()
        {
            return new List<TokenConversion>()
            {
                new BoldConversion(),
                new ItalicConversion(),
                new ParagraphConversion(),
            };
        }

        public override string Convert(string value)
        {
            return ConvertFromText((ReadOnlySpan<char>)value);
        }

        public override string Convert(string[] value)
        {
            return ConvertFromText((ReadOnlySpan<char>)string.Join(" ", value));
        }

        private string ConvertFromText(ReadOnlySpan<char> text){
            Regex regex= new Regex(m_supportConversion);
            StringBuilder stringBuilder = new StringBuilder();

            TextConversionCache.Instance.SetConverter(this);
            for(int i = 0; i < text.Length; ++i)
            {
                if (regex.IsMatch(text[i].ToString()))
                {
                    int replaceIndex = GetTokenReplacement(i, text, out string replaceString);
                    stringBuilder.Append(TextConversionCache.Instance.Replace(replaceString));
                    i += (replaceIndex > 1) ? replaceIndex : 0;
                    continue;
                }

                stringBuilder.Append(text[i].ToString());
            }

            return stringBuilder.ToString();
        }
    }
}