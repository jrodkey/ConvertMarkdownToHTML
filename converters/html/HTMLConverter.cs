
using ConvertMarkdownToHTML.converters;

namespace ConvertMarkdownToHTML.html.converters
{
    /// <summary>
    /// Converter class that takes a Markdown document and converts it to HTML.
    /// </summary>
    public class HTMLConverter : TextConverter
    {
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
    }
}