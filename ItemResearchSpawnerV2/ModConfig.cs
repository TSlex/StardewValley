using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using ItemResearchSpawnerV2.Core.Data.Enums;
using Microsoft.Xna.Framework;
using ItemResearchSpawnerV2.Core;
using System.Text.Json.Serialization;

namespace ItemResearchSpawnerV2 {

    internal class ModConfig {

        [JsonIgnore]
        public KeybindList ShowMenuButton = KeybindList.ForSingle(SButton.R);

        public ModMode DefaultMode = ModMode.Research;

        public bool UseDefaultBalanceConfig = true;
        public bool ShowMissingItems = false;
        public bool EnableSounds = true;

        public float ResearchAmountMultiplier = 1.5f;
        public float SellPriceMultiplier = 0.9f;
        public float BuyPriceMultiplier = 1.1f;
        public float ResearchTimeSeconds = 1f;

        public bool UseCustomUIColor = false;
        public Color CustomUIColor = Color.Gold;

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

        public bool GetUseCustomUIColor() {
            return UseCustomUIColor;
        }

        public Color GetCustomUIColor() {
            return CustomUIColor;
        }

        // ===============================================================================

        public void SetShowMenuButton(KeybindList value) {
            ShowMenuButton = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetDefaultMode(ModMode value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            DefaultMode = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetUseDefaultBalanceConfig(bool value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            UseDefaultBalanceConfig = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetShowMissingItems(bool value) {
            ShowMissingItems = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetEnableSounds(bool value) {
            EnableSounds = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetResearchAmountMultiplier(float value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            ResearchAmountMultiplier = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetSellPriceMultiplier(float value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            SellPriceMultiplier = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetBuyPriceMultiplier(float value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            BuyPriceMultiplier = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetResearchTimeSeconds(float value) {
            if (!Context.IsMainPlayer || !ModManager.Instance.SaveDataLoaded) {
                return;
            }

            ResearchTimeSeconds = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetUseCustomUIColor(bool value) {
            UseCustomUIColor = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetCustomUIColor(Color value) {
            CustomUIColor = value;
            //ModEntry.Instance.OnConfigChange();
        }

        public void SetCustomUIColor(int R = -1, int G = -1, int B = -1) {
            var currentColor = CustomUIColor;

            var newR = R >= 0 ? R : currentColor.R;
            var newG = G >= 0 ? G : currentColor.G;
            var newB = B >= 0 ? B : currentColor.B;

            CustomUIColor = new Color(newR, newG, newB);
            //ModEntry.Instance.OnConfigChange();
        }
    }
}
