using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Ilaro.Admin.Core;

namespace Ilaro.Admin.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string phrase)
        {
            return string.IsNullOrEmpty(phrase);
        }

        public static bool IsNullOrWhiteSpace(this string phrase)
        {
            return string.IsNullOrWhiteSpace(phrase);
        }

        public static bool HasValue(this string phrase)
        {
            return phrase.IsNullOrEmpty() == false;
        }

        public static string Slug(this string phrase)
        {
            if (phrase.IsNullOrEmpty())
            {
                return "";
            }
            string str = phrase.RemoveAccent().ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public static string SlugFileName(this string fileName)
        {
            var ext = Path.GetExtension(fileName);
            fileName = Path.GetFileNameWithoutExtension(fileName).Slug() + ext;

            return fileName;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Convert the string to Pascal case.
        /// </summary>
        /// <param name="the_string">the string to turn into Pascal case</param>
        /// <returns>a string formatted as Pascal case</returns>
        public static string ToPascalCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
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
            if (the_string == null || the_string.Length < 2) return the_string;

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
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
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Start with the first character.
            string result = the_string.Substring(0, 1).ToUpper();

            // Add the remaining characters.
            for (int i = 1; i < the_string.Length; i++)
            {
                if (Char.IsUpper(the_string[i])) result += " ";
                result += the_string[i];
            }

            return result;
        }

        public static string SplitCamelCase(this string input)
        {
            return Regex.Replace(Regex.Replace(input, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static string Pluralize(this string value, int count = 0)
        {
            if (count == 1)
            {
                return value;
            }

            return PluralizationService
                .CreateService(new CultureInfo("en-US"))
                .Pluralize(value);
        }

        public static string ToStringSafe(this object value)
        {
            if (value == null)
            {
                return String.Empty;
            }
            return value.ToString();
        }

        public static string ToStringSafe(
            this object value, 
            Property property, 
            string defaultFormat = "", 
            CultureInfo culture = null)
        {
            if (value == null)
            {
                return String.Empty;
            }

            culture = culture ?? CultureInfo.InvariantCulture;

            if (property.TypeInfo.DataType == DataType.Numeric)
            {
                try
                {
                    return Convert.ToDecimal(value).ToString(property.Format, culture);
                }
                catch { }
            }

            if (property.TypeInfo.DataType == DataType.DateTime)
            {
                return ((DateTime)value).ToString(property.Format ?? defaultFormat, culture);
            }

            return value.ToString();
        }

        public static string Fill(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string Undecorate(this string text)
        {
            return text.TrimStart('[').TrimEnd(']');
        }

        public static string GetValueOrDefault(this string text, string defaultValue)
        {
            return text.IsNullOrWhiteSpace() ?
                defaultValue :
                text;
        }
    }
}