using System.Text;
using System.Text.RegularExpressions;

namespace ConvertMarkdownToHTML.conversion
{
    /// <summary>
    /// Defines a list of the token conversions that will become
    /// active that behaves as a stack.
    /// </summary>
    public class TokenConversionRetriever : IRetriever<TokenConversion>
    {
        protected string m_supportedConversionsSingleLine;
        protected List<TokenConversion> m_conversionsList;
        protected Dictionary<string, TokenConversion> m_supportedConversions;

        public TokenConversionRetriever(Dictionary<string, TokenConversion> tokenConversions, string singleLineSupportedPattern)
        {
            m_conversionsList = new List<TokenConversion>();
            m_supportedConversions = tokenConversions;
            m_supportedConversionsSingleLine = singleLineSupportedPattern;
        }

        /// <summary>
        /// Adds the conversion by the given name to the Stack.
        /// </summary>
        public string Push(string tokenName)
        {
            if (!m_supportedConversions.ContainsKey(tokenName))
            {
                return "";
            }

            TokenConversion conversion = new TokenConversion(m_supportedConversions[tokenName]);
            return Push(conversion);
        }

        /// <summary>
        /// Adds the given conversion to the Stack.
        /// </summary>
        public string Push(TokenConversion conversion)
        {
            Convert(conversion);
            m_conversionsList.Add(conversion);
            RemoveSet(conversion.Source);
            return conversion.Get();
        }

        /// <summary>
        /// Pops the last conversion from the Stack.
        /// </summary>
        /// <returns></returns>
        public TokenConversion Pop()
        {
            if (m_conversionsList.Count == 0)
            {
                return TokenConversion.EmptyTokenConversion;
            }

            TokenConversion conversion = m_conversionsList[m_conversionsList.Count - 1];
            m_conversionsList.RemoveAt(m_conversionsList.Count - 1);
            return conversion;
        }

        /// <summary>
        /// Closes the token conversions for the end of single line.
        /// </summary>
        /// <returns></returns>
        public string EndSingleLine()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = m_conversionsList.Count - 1; i >= 0; --i)
            {
                TokenConversion conversion = m_conversionsList[i];
                if (Regex.Match(conversion.Source, m_supportedConversionsSingleLine).Success)
                {
                    stringBuilder.Append(conversion.End);
                    Remove(conversion.Source);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Closes the token conversions for the end of multiline.
        /// </summary>
        /// <returns></returns>
        public string EndMulitiLine()
        {
            StringBuilder stringBuilder = new StringBuilder();
            while (m_conversionsList.Count != 0)
            {
                TokenConversion conversion = Pop();
                if (conversion.ParsingStatus)
                {
                    stringBuilder.Append(conversion.End);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Retrieves the last entry, but it doesn't remove it.
        /// </summary>
        /// <returns></returns>
        public TokenConversion Peek()
        {
            return (m_conversionsList.Count != 0) ? m_conversionsList[m_conversionsList.Count - 1] : TokenConversion.EmptyTokenConversion;
        }

        /// <summary>
        /// Retrieves the entry by the given name, but doesn't remove it, starting at the top of the 
        //  stack and going in reverse.
        /// </summary>
        /// <param name="name">Name of the entry</param>
        /// <returns></returns>
        public TokenConversion ReversePeek(string name)
        {
            if (m_conversionsList.Count != 0)
            {
                for (int i = m_conversionsList.Count - 1; i >= 0; --i)
                {
                    TokenConversion conversion = m_conversionsList[i];
                    if (conversion != null && conversion.Source == name)
                    {
                        return conversion;
                    }
                }
            }
            
            return TokenConversion.EmptyTokenConversion;
        }

        /// <summary>
        /// Removes from the given name.
        /// </summary>
        /// <param name="name">Name of entry</param>
        public TokenConversion Remove(string name)
        {
            for (int i = 0; i < m_conversionsList.Count; ++i)
            {
                TokenConversion conversion = m_conversionsList[i];
                if (conversion.Source == name)
                {
                    m_conversionsList.RemoveAt(i);
                    return conversion;
                }
            }

            return TokenConversion.EmptyTokenConversion;
        }

        /// <summary>
        /// Removes a set with given name, if they are complete.
        /// </summary>
        /// <param name="name">Name of entries</param>
        public void RemoveSet(string name)
        {
            if (m_conversionsList.Count - 2 < 0)
            {
                return;
            }

            TokenConversion conversion1 = m_conversionsList[m_conversionsList.Count - 1];
            TokenConversion conversion2 = m_conversionsList[m_conversionsList.Count - 2];
            if (conversion1.Source == name && conversion2.Source == name)
            {
                Pop();
                Pop();
            }
        }

        /// <summary>
        /// Converts the status of the conversion depending on if there is a last entry in the stack.
        /// </summary>
        private void Convert(TokenConversion conversion)
        {
            TokenConversion reverseResult = ReversePeek(conversion.Source);
            if (reverseResult != TokenConversion.EmptyTokenConversion)
            {
                conversion.ParsingStatus = !reverseResult.ParsingStatus;
            }
        }
    }
}