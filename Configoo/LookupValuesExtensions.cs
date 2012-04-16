namespace Configoo
{
    public static class LookupValuesExtensions
    {
        public static Configured Value(this IHaveLookupValues values)
        {
            return new Configured(values);
        }
    }
}