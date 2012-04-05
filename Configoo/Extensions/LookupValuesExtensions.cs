namespace Configoo
{
    public static class LookupValuesExtensions
    {
        public static Configured Value(this ILookupValues values)
        {
            return new Configured(values);
        }
    }
}