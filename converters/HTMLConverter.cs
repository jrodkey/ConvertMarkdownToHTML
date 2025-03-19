
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public class HTMLConverter : Converter
    {
        private string m_supportConversion = @"[a-zA-Z@#*!]";

        public HTMLConverter()
        {
            Conversions.Add('*', new ItalicConversion());
            Conversions.Add('_', new BoldConversion());
            Conversions.Add('\n', new ParagraphConversion());
        }

        public object Convert(object value)
        {
            if (value is IEnumerable<string>)
            {
                ConvertFromText((ReadOnlySpan<char>)string.Join(" ", value as IEnumerable<string>));
            }

            return null;
        }

        public void ConvertFromText(ReadOnlySpan<char> text){
            Regex regex= new Regex(m_supportConversion);
            StringBuilder stringBuilder = new StringBuilder();
            foreach(char c in text){
                if (regex.IsMatch(c.ToString()))
                {
                    TokenConversion conversion = GetTokenConversion(c);
                    if (!(conversion is EmptyTokenConversion))
                    {
                        stringBuilder.Append(conversion.Get(c));
                        continue;
                    }
                }

                stringBuilder.Append(c);
            }
        }
    }
}