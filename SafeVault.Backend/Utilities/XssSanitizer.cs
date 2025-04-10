using System.Text.RegularExpressions;

namespace SafeVault.Backend.Utilities
{
    public static class XssSanitizer
    {
        /// <summary>
        /// Sanitizes input to remove potential XSS attempts.
        /// </summary>
        /// <param name="input">The input string to sanitize.</param>
        /// <returns>A sanitized string with potentially harmful characters removed.</returns>
        public static bool IsValidXSSInput(string input)
        {
            string originalInput = new string(input.ToCharArray());// Store the original input for logging or debugging if needed

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Remove script tags and their content
            input = Regex.Replace(input, @"<script.*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase);

            // Remove event handlers (e.g., onclick, onerror)
            input = Regex.Replace(input, @"on\w+\s*=\s*(['""]?)[^'""]*\1", string.Empty, RegexOptions.IgnoreCase);

            // Remove potentially harmful HTML tags
            input = Regex.Replace(input, @"<(iframe|object|embed|link|style|base|meta|form|img|svg|video|audio|applet|frame|frameset|layer|bgsound|blink|ilayer|isindex|marquee).*?>", string.Empty, RegexOptions.IgnoreCase);

            // Remove JavaScript URLs
            input = Regex.Replace(input, @"javascript\s*:\s*", string.Empty, RegexOptions.IgnoreCase);

            // Remove other potentially harmful characters
            input = input.Replace("<", string.Empty)
                         .Replace(">", string.Empty)
                         .Replace("\"", string.Empty)
                         .Replace("'", string.Empty)
                         .Replace(";", string.Empty)
                         .Replace("--", string.Empty);

            input = input.Trim();

            if (input != originalInput)
            {
                // Log the sanitization process if needed
                return false;
            }

            return true;
        }
    }
}
