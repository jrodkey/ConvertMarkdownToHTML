using System.Text;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public abstract class TextConverter : IConverter<string, TokenConversion>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string Convert(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string Convert(string[] value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual List<TokenConversion> GetConversions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetTokenReplacement(int index, ReadOnlySpan<char> text, out string key)
        {
            int incrementIndex = index + 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(text[index]);
            while((text[incrementIndex] == text[index]) && (incrementIndex < text.Length))
            {
                sb.Append(text[incrementIndex++]);
            }
            
            key = sb.ToString();
            return incrementIndex;
        }
    }
}