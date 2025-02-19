using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using ItemResearchSpawnerV2.Core.Data.Enums;
using Microsoft.Xna.Framework;
using ItemResearchSpawnerV2.Core;
using System.Text.Json.Serialization;

namespace ItemResearchSpawnerV2 {

    internal static class ModConfigConstraints {
        public static float ResearchAmountMultipliterMin => 0.0f;
        public static float ResearchAmountMultipliterMax => 10f;

        public static float SellPriceMultiplierMin => 0.0f;
        public static float SellPriceMultiplierMax => 10f;

        public static float BuyPriceMultiplierMin => 0.0f;
        public static float BuyPriceMultiplierMax => 10f;

        public static float ResearchTimeSecondsMin => 0.0f;
        public static float ResearchTimeSecondsMax => 60f;
    }

    internal class ModConfig {

        public string ShowMenuButton = "R";

        public ModMode DefaultMode = ModMode.Research;

        public bool UseDefaultBalanceConfig = true;
        public bool ShowMissingItems = false;
        public bool EnableSounds = true;

        public float ResearchAmountMultiplier = 1.5f;
        public float SellPriceMultiplier = 0.8f;
        public float BuyPriceMultiplier = 1.2f;
        public float ResearchTimeSeconds = 1f;

        public bool AutoResearch = false;

        public bool ShareProgression = false;
        public bool DisableNonHostCommands = false;

        public bool UseCustomUIColor = false;
        public Color CustomUIColor = Color.Gold;

        // ===============================================================================

        public KeybindList GetShowMenuButton() {
            return KeybindList.Parse(ShowMenuButton);
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

        public bool GetAutoResearch() {
            return AutoResearch;
        }

        public bool GetShareProgression() {
            return ShareProgression;
        }

        public bool GetDisableNonHostCommands() {
            return DisableNonHostCommands;
        }

        public bool GetUseCustomUIColor() {
            return UseCustomUIColor;
        }

        public Color GetCustomUIColor() {
            return CustomUIColor;
        }

        // ===============================================================================

        public void SetShowMenuButton(KeybindList value) {
            ShowMenuButton = value.ToString();
        }

        public void SetDefaultMode(ModMode value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            DefaultMode = value;
        }

        public void SetUseDefaultBalanceConfig(bool value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            UseDefaultBalanceConfig = value;
        }

        public void SetShowMissingItems(bool value) {
            ShowMissingItems = value;
        }

        public void SetEnableSounds(bool value) {
            EnableSounds = value;
        }

        public void SetResearchAmountMultiplier(float value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            var checkedValue = value >= ModConfigConstraints.ResearchAmountMultipliterMin ? value : ModConfigConstraints.ResearchAmountMultipliterMin;
            checkedValue = checkedValue <= ModConfigConstraints.ResearchAmountMultipliterMax ? checkedValue : ModConfigConstraints.ResearchAmountMultipliterMax;

            ResearchAmountMultiplier = checkedValue;
        }

        public void SetSellPriceMultiplier(float value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            var checkedValue = value >= ModConfigConstraints.SellPriceMultiplierMin ? value : ModConfigConstraints.SellPriceMultiplierMin;
            checkedValue = checkedValue <= ModConfigConstraints.SellPriceMultiplierMax ? checkedValue : ModConfigConstraints.SellPriceMultiplierMax;

            SellPriceMultiplier = checkedValue;
        }

        public void SetBuyPriceMultiplier(float value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            var checkedValue = value >= ModConfigConstraints.BuyPriceMultiplierMin ? value : ModConfigConstraints.BuyPriceMultiplierMin;
            checkedValue = checkedValue <= ModConfigConstraints.BuyPriceMultiplierMax ? checkedValue : ModConfigConstraints.BuyPriceMultiplierMax;

            BuyPriceMultiplier = checkedValue;
        }

        public void SetResearchTimeSeconds(float value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            var checkedValue = value >= ModConfigConstraints.ResearchTimeSecondsMin ? value : ModConfigConstraints.ResearchTimeSecondsMin;
            checkedValue = checkedValue <= ModConfigConstraints.ResearchTimeSecondsMax ? checkedValue : ModConfigConstraints.ResearchTimeSecondsMax;

            ResearchTimeSeconds = checkedValue;
        }

        public void SetAutoResearch(bool value) {
            AutoResearch = value;
        }

        public void SetShareProgression(bool value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            ShareProgression = value;
        }

        public void SetDisableNonHostCommands(bool value) {
            if (!Context.IsMainPlayer) {
                return;
            }

            DisableNonHostCommands = value;
        }

        public void SetUseCustomUIColor(bool value) {
            UseCustomUIColor = value;
        }

        public void SetCustomUIColor(Color value) {
            CustomUIColor = value;
        }

        public void SetCustomUIColor(int R = -1, int G = -1, int B = -1) {
            var currentColor = CustomUIColor;

            var newR = R >= 0 ? R : currentColor.R;
            var newG = G >= 0 ? G : currentColor.G;
            var newB = B >= 0 ? B : currentColor.B;

            CustomUIColor = new Color(newR, newG, newB);
        }
    }
}
