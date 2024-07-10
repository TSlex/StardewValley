using ItemResearchSpawnerV2.Core.UI;
using Microsoft.Xna.Framework;

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

        public static bool HasPriceBehaviour(this ModMode mode) {
            return mode switch {
                ModMode.Research => false,
                ModMode.BuySell => true,
                ModMode.Combined => true,
                ModMode.ResearchPlus => false,
                ModMode.BuySellPlus => true,
                _ => false,
            };
        }

        public static Func<string> GetTranstationFunc(this ModMode mode) {

            return mode switch {
                ModMode.Research => I18n.ModMode_Research,
                ModMode.BuySell => I18n.ModMode_BuySell,
                ModMode.Combined => I18n.ModMode_Combined,
                ModMode.ResearchPlus => I18n.ModMode_ResearchPlus,
                ModMode.BuySellPlus => I18n.ModMode_BuySellPlus,
                _ => throw new NotImplementedException(),
            };
        }

        public static Color GetColor(this ModMode current)
        {
            if (ModManager.Instance.Config.UseCustomUIColor) {
                return ModManager.Instance.Config.CustomUIColor;
            }

            return current switch
            {
                ModMode.Research => UIConstants.ResearchModeColor,
                ModMode.BuySell => UIConstants.BuySellModeColor,
                ModMode.Combined => UIConstants.CombinedModeColor,
                ModMode.ResearchPlus => UIConstants.ResearchPlusModeColor,
                ModMode.BuySellPlus => UIConstants.BuySellPlusModeColor,
                _ => throw new NotImplementedException(),
            };
        }
    }
}