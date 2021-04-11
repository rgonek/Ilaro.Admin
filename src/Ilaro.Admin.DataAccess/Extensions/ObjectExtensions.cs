namespace Ilaro.Admin.DataAccess.Extensions
{
    public static class ObjectExtensions
    {
        public static bool Is(this object value, int expected)
        {
            if (value is int)
            {
                return (value as int?) == expected;
            }
            return false;
        }
    }
}