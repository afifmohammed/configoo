namespace Configoo
{
    public static class LookupValuesSyntaticSugar
    {
        public static IConfigurationSugar Value(this IHaveLookupValues values)
        {
            return new SyntaticSugar(values);
        }
    }
}