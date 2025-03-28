
using System.Text;
using ConvertMarkdownToHTML.converters;
using ConvertMarkdownToHTML.conversion;

namespace ConvertMarkdownToHTML.html.converters
{
    /// <summary>
    /// Converter class that takes a Mardown document and converts it to HTML.
    /// </summary>
    public class HTMLConverter : TextConverter
    {
        /// <summary>
        /// Converts the given text document.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Convert(string value)
        {
            return ConvertFromText((ReadOnlySpan<char>)value);
        }

        /// <summary>
        /// Converts the given text document.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string Convert(string[] value)
        {
            return ConvertFromText((ReadOnlySpan<char>)string.Join(" ", value));
        }

        /// <summary>
        /// Converts the given text document.
        /// </summary>
        /// <param name="text">Text document</param>
        /// <returns></returns>
        private string ConvertFromText(ReadOnlySpan<char> text)
        {
            if (text == null || text.Length == 0)
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();
            (string, int) results = GetNewDoublelineReplacement(0, text);
            stringBuilder.Append(results.Item1);
            for (int i = results.Item2; i < text.Length; ++i)
            {
                results = GetNextCharacter(i, text);
                stringBuilder.Append(results.Item1);

                i += results.Item2;
                if (results.Item2 > 1)
                {
                    // If more than one character needs to be skipped, then we determine if 
                    // the next character is valid. If it is not, backup by one iteratively.
                    if (i > 0 && text[i] != 32 && !IsValidMatch(text[i], Config.NewLineReplacementSet))
                    {
                        --i;
                    }


                    /* while (i > 0)
                    {
                        if (text[i] == 32 || IsValidMatch(text[i], Config.NewLineReplacementPattern))
                        {
                            break;
                        }
                        --i;
                    } */
                }
            }

            stringBuilder.Append(Config.Retriever.EndMulitiLine());
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Processed when a single line was parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string and the spaces that it takes</returns>
        public override (string, int) GetNewlineSingleSpaced(int index, ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Config.Retriever.EndSingleLine());
            stringBuilder.Append(Config.LineBreak);
            return (stringBuilder.ToString(), 1);
        }

        /// <summary>
        /// Processed when multiple lines have been parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string and the spaces that it takes</returns>
        public override (string, int) GetNewlineDoubleSpaced(int index, ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Config.Retriever.EndMulitiLine());

            int lineCount = 0;
            while (true)
            {
                if (IsValidMatch(text[index + lineCount], Config.NewLineSet) &&
                        IsValidMatch(text[index + lineCount + 1], Config.NewLineSet))
                {
                    stringBuilder.Append(Config.LineBreak);
                    lineCount += 2;
                    continue;
                }

                break;
            }

            (string, int) results = GetNewDoublelineReplacement(index + lineCount, text);
            stringBuilder.Append(results.Item1);
            return (stringBuilder.ToString(), lineCount);
        }

        /// <summary>
        /// Gets a custom token.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string, if the given text matches with the various group.</returns>
        public override string GetCustomToken(int index, ReadOnlySpan<char> text)
        {
            // Copy character
            if (CheckToken(Config.Copy, index, text))
            {
                return Config.Copy;
            }

            // Ampersand character
            if (CheckToken(Config.Ampersand, index, text))
            {
                return Config.Ampersand;
            }

            return "";
        }

        // <summary>
        /// Verifies validity between the token and the given text.
        /// </summary>
        /// <param name="token">name of token</param>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>True, if the two match.</returns>
        public bool CheckToken(string token, int index, ReadOnlySpan<char> text)
        {
            int length = token.Length;
            for (int i = 0; i < length; ++i)
            {
                if (!text[index + i].Equals(token[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a replacement once a single line has been parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string and the spaces that it takes</returns>
        private (string, int) GetNewSinglelineReplacement(int index, ReadOnlySpan<char> text)
        {
            return GetReplacement(index, text);
        }

        /// <summary>
        /// Returns a replacement once muliple lines have been parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string and the spaces that it takes</returns>
        private (string, int) GetNewDoublelineReplacement(int index, ReadOnlySpan<char> text)
        {
            return (text.Length > index && IsValidMatch(text[index], Config.NewLineReplacementSet)) ?
                GetReplacement(index, text) : GetReplacement(index, text, "$");
        }
    }
}