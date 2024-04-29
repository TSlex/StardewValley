using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using ItemResearchSpawnerV2.Core.Data.Enums;

namespace ItemResearchSpawnerV2 {
    internal class ModConfig
    {
        public KeybindList ShowMenuButton = KeybindList.ForSingle(SButton.R);

        public ModMode DefaultMode = ModMode.Research;

        public bool UseDefaultBalanceConfig = true;
        public bool ShowMissingItems = false;
        public bool EnableSounds = true;

        public float ResearchAmountMultiplier = 1.5f;
        public float SellPriceMultiplier = 0.9f;
        public float BuyPriceMultiplier = 1.1f;
        public float ResearchTimeSeconds = 1f;

        // ===============================================================================

        public KeybindList GetShowMenuButton() {
            return ShowMenuButton;
        }

        public ModMode GetDefaultMode() {
            return DefaultMode;
        }

        public bool GetUseDefaultBalanceConfig() {
            return UseDefaultBalanceConfig;
        }

        public bool GetShowMissingItems() {
            return ShowMissingItems;
        }

        public bool GetEnableSounds() {
            return EnableSounds;
        }

        public float GetResearchAmountMultiplier() {
            return ResearchAmountMultiplier;
        }

        public float GetSellPriceMultiplier() {
            return SellPriceMultiplier;
        }

        public float GetBuyPriceMultiplier() {
            return BuyPriceMultiplier;
        }

        public float GetResearchTimeSeconds() {
            return ResearchTimeSeconds >= 0f ? ResearchTimeSeconds : 0f;
        }

        // ===============================================================================

        public void SetShowMenuButton(KeybindList value) {
            ShowMenuButton = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetDefaultMode(ModMode value) {
            DefaultMode = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetUseDefaultBalanceConfig(bool value) {
            UseDefaultBalanceConfig = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetShowMissingItems(bool value) {
            ShowMissingItems = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetEnableSounds(bool value) {
            EnableSounds = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetResearchAmountMultiplier(float value) {
            ResearchAmountMultiplier = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetSellPriceMultiplier(float value) {
            SellPriceMultiplier = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetBuyPriceMultiplier(float value) {
            BuyPriceMultiplier = value;
            ModEntry.Instance.OnConfigChange();
        }

        public void SetResearchTimeSeconds(float value) {
            ResearchTimeSeconds = value;
            ModEntry.Instance.OnConfigChange();
        }
    }
}
