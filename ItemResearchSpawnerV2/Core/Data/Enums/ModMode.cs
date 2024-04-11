namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum ModMode {
        Research,
        BuySell,
        Combined,
        ResearchPlus,
        BuySellPlus
    }

    internal static class ModModeExtensions {

        public static string GetString(this ModMode current) {
            return current switch {
                ModMode.Research => "Research (Spawn) mode",
                ModMode.BuySell => "Buy/Sell mode",
                ModMode.Combined => "Combined mode",
                ModMode.ResearchPlus => "Mr.Qi mode",
                ModMode.BuySellPlus => "Jojo mode",
                _ => "???"
            };
        }
    }
}