using System.Globalization;

namespace Pocotheosis.MemberTypes
{
    static class Extensions
    {
        public static string ToToken(this bool value)
        {
            return value ? "true" : "false";
        }

        public static string ICFormat(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}
