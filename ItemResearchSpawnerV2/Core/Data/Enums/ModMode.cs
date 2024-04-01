namespace ItemResearchSpawnerV2.Core.Enums
{
    internal enum ModMode
    {
        Research,
        BuySell,
        Combined,
        God
    }

    internal static class ModModeExtensions
    {

        public static string GetString(this ModMode current)
        {
            return current switch
            {
                ModMode.Research => "Research (Spawn) mode",
                ModMode.BuySell => "Buy/Sell mode",
                ModMode.Combined => "Combined mode",
                ModMode.God => "God mode",
                _ => "???"
            };
        }
    }
}