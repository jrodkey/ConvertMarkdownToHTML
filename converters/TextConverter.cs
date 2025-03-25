using System.Text;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public abstract class TextConverter : IConverter<string, TokenConversion>
    {
        protected Dictionary<string, TokenConversion> m_conversions;

        public TextConverter()
        {
            m_conversions = GetConversions();
        }

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
        public virtual Dictionary<string, TokenConversion> GetConversions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetTokenReplacement(int index, ReadOnlySpan<char> text)
        {
            int incrementIndex = index + 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(text[index]);
            while ((text[incrementIndex] == text[index]) && (incrementIndex < text.Length))
            {
                sb.Append(text[incrementIndex++]);
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Replace(string name)
        {
            if (!m_conversions.ContainsKey(name))
            {
                return string.Empty;
            }

            return m_conversions[name].Get();
        }
    }
}