using System.Text;
using System.Text.RegularExpressions;
using ConvertMarkdownToHTML.conversion;
using ConvertMarkdownToHTML.converters.configuration;

namespace ConvertMarkdownToHTML.converters
{
    /// <summary>
    /// Defines the foundational operations of a text converter class.
    /// </summary>
    public abstract class TextConverter : IConverter<string, TokenConversion>
    {
        /// <summary>
        /// Spacing between a line and multiple lines.
        /// </summary>
        public enum LineSpacing
        {
            None = 0,
            Single,
            Multiline
        }

        /// <summary>
        /// Gets the configuration of the text converter.
        /// </summary>
        public HTMLConverterConfiguration Config { get; } = new HTMLConverterConfiguration();

        /// <summary>
        /// Converts a string to a supported text format.
        /// </summary>
        /// <returns></returns>
        
        public virtual string Convert(string value)
        {
            return ConvertFromText((ReadOnlySpan<char>)value);
        }

        /// <summary>
        /// Converts an string array to a supported text format.
        /// </summary>
        /// <returns></returns>
        public virtual string Convert(string[] value)
        {
            return ConvertFromText((ReadOnlySpan<char>)string.Join(" ", value));
        }

        /// <summary>
        /// Converts the given text document to supported text format.
        /// </summary>
        /// <returns></returns>
        protected virtual string ConvertFromText(ReadOnlySpan<char> text)
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

                if (results.Item2 > 1)
                {
                    // Depending on your text format, if there is no a space or a valid character after 
                    // the advancement, then backtrack by one and continue.
                    if (i > 0 && text[i] != 32 && IsValidMatch(text[i], Config.ValidBacktracePattern))
                    {
                        --i;
                    }
                    i += results.Item2 - 1;
                }
            }

            stringBuilder.Append(Config.Retriever.EndMulitiLine());
            return stringBuilder.ToString();
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

        /// <summary>
        /// Processed when a single line was parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public virtual (string, int) GetNewlineSingleSpaced(int index, ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Config.Retriever.EndSingleLine());

            (string, int) lines = GetLineCount(index, text);
            stringBuilder.Append(lines.Item1);

            return (stringBuilder.ToString(), lines.Item2);
        }

        /// <summary>
        /// Processed when multiple lines have been parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public virtual (string, int) GetNewlineDoubleSpaced(int index, ReadOnlySpan<char> text)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(Config.Retriever.EndMulitiLine());

            (string, int) lines = GetLineCount(index, text);
            stringBuilder.Append(lines.Item1);

            (string, int) results = GetNewDoublelineReplacement(index + lines.Item2, text);
            stringBuilder.Append(results.Item1);
            return (stringBuilder.ToString(), results.Item2 + lines.Item2);
        }

        /// <summary>
        /// Gets a custom token.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string, if the given text matches with the various group.</returns>
        public virtual string GetCustomToken(int index, ReadOnlySpan<char> text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given the index of the start of a line, returns the number of consecutive lines.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Each line's representation and number of lines found.</returns>
        public (string, int) GetLineCount(int index, ReadOnlySpan<char> text)
        {
            string linesRepresentation = string.Empty;
            int lineCount = 0;
            while (true)
            {
                if (IsValidMatch(text[index + lineCount], Config.NewLineSet))
                {
                    linesRepresentation += text[index + lineCount];
                    lineCount += 1;
                    continue;
                }
                break;
            }

            return (linesRepresentation, lineCount);
        }

        /// <summary>
        /// Retrieves the entire string and complete length of the token that needs
        /// to be converted.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public string GetToken(int index, ReadOnlySpan<char> text)
        {
            int incrementIndex = index + 1;
            StringBuilder sb = new StringBuilder();
            sb.Append(text[index]);

            // Attempt to match generally.
            while ((incrementIndex < text.Length) && (text[incrementIndex] == text[index]))
            {
                sb.Append(text[incrementIndex++]);
            }

            // Otherwise, use the customized matching with the regex groups.
            if (sb.Length <= 1)
            {
                string result = GetCustomToken(index, text);
                if (result.Length > 0)
                {
                    sb.Clear();
                    sb.Append(result);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compares the given character and the stored pattern.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsValidMatch(char character)
        {
            return Regex.Match(character.ToString(), Config.GeneralConversionsPattern).Success;
        }

        /// <summary>
        /// Compares the given character and against the given pattern.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public bool IsValidMatch(char character, string pattern)
        {
            return Regex.Match(character.ToString(), pattern).Success;
        }

        /// <summary>
        /// Determines if it is a single line or multiline at index.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public LineSpacing IsNewline(int index, ReadOnlySpan<char> text)
        {
            if (!text[index].Equals('\n') && !text[index].Equals('\r'))
            {
                return LineSpacing.None;
            }

            if (text.Length > index + 3 &&
                    IsValidMatch(text[index], Config.NewLineSet) &&
                    IsValidMatch(text[index + 1], Config.NewLineSet) &&
                    IsValidMatch(text[index + 2], Config.NewLineSet) &&
                    IsValidMatch(text[index + 3], Config.NewLineSet))
            {
                return LineSpacing.Multiline;
            }

            if (text.Length > index + 1 &&
                IsValidMatch(text[index], Config.NewLineSet) &&
                IsValidMatch(text[index + 1], Config.NewLineSet))
            {
                return LineSpacing.Single;
            }

            return LineSpacing.None;
        }

        /// <summary>
        /// Retrieves the next character.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public virtual (string, int) GetNextCharacter(int index, ReadOnlySpan<char> text)
        {
            (string, int) results = ("", 0);
            switch (IsNewline(index, text))
            {
                case LineSpacing.Single:
                    results = GetNewlineSingleSpaced(index, text);
                    break;
                case LineSpacing.Multiline:
                    results = GetNewlineDoubleSpaced(index, text);
                    break;
                default:
                    results = GetReplacement(index, text);
                    break;
            }

            return results;
        }

        /// <summary>
        /// Gets a conversion by the given token and adds it to the stack.
        /// </summary>
        /// <param name="name">Token name</param>
        /// <returns></returns>
        public string GetConversion(string name)
        {
            return Config.Retriever.Push(name);
        }

        /// <summary>
        /// Gets a string replacement using a conversion key.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <param name="conversionKey"Token key</param>
        /// <returns></returns>
        public (string, int) GetReplacement(int index, ReadOnlySpan<char> text, string conversionKey)
        {
            return (!string.IsNullOrEmpty(conversionKey)) ?
                (Config.Retriever.Push(conversionKey), 0) :
                GetReplacement(index, text, conversionKey);
        }

        /// <summary>
        /// Gets a string replacement using a conversion key and a customized pattern to compare against.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public (string, int) GetReplacement(int index, ReadOnlySpan<char> text)
        {
            string newToken = text[index].ToString();
            int replacementLength = 0;

            if (IsValidMatch(text[index], Config.GeneralConversionsPattern))
            {
                string replacementString = GetToken(index, text);
                newToken = GetConversion(replacementString);
                replacementLength = replacementString.Length;
            }

            return (newToken, replacementLength);
        }
    }
}