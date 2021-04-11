namespace Ilaro.Admin.Common.Extensions
{
    internal static class DotNetToMomentDateTimeFormat
    {
        internal static string Convert(string format)
        {
            return format.Replace("d", "D") // days
                .Replace("f", "S") // miliseconds
                .Replace("F", "S") // miliseconds
                .Replace("tt", "a") // am pm
                .Replace("t", "a") // am pm
                .Replace("y", "Y") // years
                .Replace("z", "Z"); // offset from UTC
        }
    }
}