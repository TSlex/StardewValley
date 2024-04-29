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
            return GetTranstationFunc(current)();
        }

        public static Func<string> GetTranstationFunc(ModMode mode) {

            return mode switch {
                ModMode.Research => I18n.ModMode_Research,
                ModMode.BuySell => I18n.ModMode_BuySell,
                ModMode.Combined => I18n.ModMode_Combined,
                ModMode.ResearchPlus => I18n.ModMode_ResearchPlus,
                ModMode.BuySellPlus => I18n.ModMode_BuySellPlus,
                _ => throw new NotImplementedException(),
            };
        }
    }
}