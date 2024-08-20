using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace Owls_BE.Helper
{
    public partial class TextTag
    {
        public static string ConvertToSlug(string str)
        {
            str = str.ToLowerInvariant();

            str = RemoveDiacritics(str);

            str = Regex.Replace(str, @"[\s\-]+", "-");

            return str;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
