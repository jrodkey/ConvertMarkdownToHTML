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
        /// Gets the configuration of the text converter.
        /// </summary>
        public TextConverterConfiguration Config { get; } = new TextConverterConfiguration();

        /// <summary>
        /// Converts a string to a supported text format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string Convert(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts an string array to a supported text format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual string Convert(string[] value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of token conversion objects.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, TokenConversion> GetConversions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processed when a single line was parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public virtual (string, int) GetNewlineSingleSpaced(int index, ReadOnlySpan<char> text)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processed when multiple lines have been parsed.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns></returns>
        public virtual (string, int) GetNewlineDoubleSpaced(int index, ReadOnlySpan<char> text)
        {
            throw new NotImplementedException();
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

            // Otherwise, use the customized matching with the groups.
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
        /// Gets a string replacement.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <returns>Replacement string and the spaces that it takes</returns>
        public (string, int) GetReplacement(int index, ReadOnlySpan<char> text)
        {
            return GetReplacement(index, text, string.Empty, Config.GeneralConversionsPattern);
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
                GetReplacement(index, text, conversionKey, Config.GeneralConversionsPattern);
        }

        /// <summary>
        /// Gets a string replacement using a conversion key and a customized pattern to compare against.
        /// </summary>
        /// <param name="index">Index value</param>
        /// <param name="text">Complete text</param>
        /// <param name="conversionKey">Token key</param>
        /// <param name="customRegex">Regex pattern</param>
        /// <returns></returns>
        public (string, int) GetReplacement(int index, ReadOnlySpan<char> text, string conversionKey, string customRegex)
        {
            string newToken = text[index].ToString();
            int replacementLength = 0;

            if (IsValidMatch(text[index], customRegex))
            {
                string replacementString = GetToken(index, text);
                newToken = GetConversion(replacementString);
                replacementLength = replacementString.Length - 1;
            }

            return (newToken, replacementLength);
        }
    }
}