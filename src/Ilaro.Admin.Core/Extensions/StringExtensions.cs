using Pluralize.NET;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Ilaro.Admin.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string phrase) => string.IsNullOrEmpty(phrase);

        public static bool IsNullOrWhiteSpace(this string phrase) => string.IsNullOrWhiteSpace(phrase);

        public static bool HasValue(this string phrase) => phrase.IsNullOrEmpty() == false;

        public static string Slug(this string phrase)
        {
            if (phrase.IsNullOrEmpty())
                return string.Empty;

            var str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it
            str = Regex.Replace(str, @"\s", "-"); // hyphens

            return str;
        }

        public static string SlugFileName(this string fileName) =>
            Path.GetFileNameWithoutExtension(fileName).Slug() + Path.GetExtension(fileName);

        public static string RemoveAccent(this string txt) =>
            Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(txt));

        /// <summary>
        /// Convert the string to Pascal case.
        /// </summary>
        /// <param name="the_string">the string to turn into Pascal case</param>
        /// <returns>a string formatted as Pascal case</returns>
        public static string ToPascalCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null)
                return the_string;
            if (the_string.Length < 2)
                return the_string.ToUpper();

            // Split the string into words.
            var words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            var result = string.Empty;
            foreach (var word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word[1..];
            }

            return result;
        }

        /// <summary>
        /// Convert the string to camel case.
        /// </summary>
        /// <param name="the_string">the string to turn into Camel case</param>
        /// <returns>a string formatted as Camel case</returns>
        public static string ToCamelCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null || the_string.Length < 2)
                return the_string;

            // Split the string into words.
            var words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            var result = words[0].ToLower();
            for (var i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i][1..];
            }

            return result;
        }

        /// <summary>
        /// Capitalize the first character and add a space before
        /// each capitalized letter (except the first character).
        /// </summary>
        /// <param name="the_string">the string to turn into Proper case</param>
        /// <returns>a string formatted as Proper case</returns>
        public static string ToProperCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null)
                return the_string;
            if (the_string.Length < 2)
                return the_string.ToUpper();

            // Start with the first character.
            var result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (var i = 1; i < the_string.Length; i++)
            {
                if (char.IsUpper(the_string[i])) result += " ";
                result += the_string[i];
            }

            return result;
        }

        public static string SplitCamelCase(this string input) =>
            Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");

        public static string Pluralize(this string value) =>
            new Pluralizer().Pluralize(value);

        public static string ToStringSafe(this object value) =>
            value == null ? string.Empty : value.ToString();

        public static string ToShortDateString(
            this object value,
            Property property)
        {
            if (value == null)
                return string.Empty;

            if (property.TypeInfo.DataType == DataType.DateTime)
            {
                var dateTime = (DateTime)value;
                if (property.Format.HasValue())
                    return dateTime.ToString(property.Format);
                return dateTime.ToShortDateString();
            }

            return value.ToString();
        }

        public static string ToShortTimeString(
            this object value,
            Property property)
        {
            if (value == null)
                return string.Empty;

            if (property.TypeInfo.DataType == DataType.DateTime)
            {
                var dateTime = (DateTime)value;
                if (property.Format.HasValue())
                    return dateTime.ToString(property.Format);
                return dateTime.ToShortTimeString();
            }

            return value.ToString();
        }

        public static string Fill(this string format, params object[] args) =>
            string.Format(format, args);

        public static string Undecorate(this string text) =>
            text.TrimStart('[').TrimEnd(']');

        public static string GetValueOrDefault(this string text, string defaultValue) =>
            text.IsNullOrWhiteSpace()
                ? defaultValue
                : text;
    }
}