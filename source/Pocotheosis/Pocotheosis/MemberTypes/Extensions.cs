namespace Pocotheosis.MemberTypes
{
    static class Extensions
    {
        public static string ToToken(this bool value)
        {
            return value ? "true" : "false";
        }
    }
}
