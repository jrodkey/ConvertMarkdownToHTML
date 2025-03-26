using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.converters
{
    public abstract class TextConverter : IConverter<string, TokenConversion>
    {
        protected string m_supportedConversions = "";
        protected Dictionary<string, TokenConversion> m_conversions = new Dictionary<string, TokenConversion>();

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
        public virtual string SupportedConversion
        {
            get { return m_supportedConversions; }
            set { m_supportedConversions = value; }
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
            while ((incrementIndex < text.Length) && (text[incrementIndex] == text[index]))
            {
                sb.Append(text[incrementIndex++]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsValidMatch(char character)
        {
            return Regex.Match(character.ToString(), SupportedConversion).Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsValidMatch(char character, string pattern)
        {
            return Regex.Match(character.ToString(), pattern).Success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Replace(string name)
        {
            return (!m_conversions.ContainsKey(name)) ? string.Empty : m_conversions[name].Get();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public (string, int) GetTextReplacement(int index, ReadOnlySpan<char> text, TokenConversion? conversion = null)
        {
            return GetTextReplacement(index, text, conversion, SupportedConversion);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public (string, int) GetTextReplacement(int index, ReadOnlySpan<char> text, TokenConversion? conversion, string customRegex)
        {
            string newToken = text[index].ToString();
            int replacementLength = 1;

            if (conversion != null)
            {
                newToken = conversion.Get();
                replacementLength = 0;
            }
            else if (IsValidMatch(text[index], customRegex))
            {
                string replacementString = GetTokenReplacement(index, text);
                newToken = Replace(replacementString);
                replacementLength = replacementString.Length;
            }

            return (newToken, replacementLength);
        }
    }
}