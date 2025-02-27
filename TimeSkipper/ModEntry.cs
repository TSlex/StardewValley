using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TimeSkipper.Api;
using TimeSkipper.Core;
using TimeSkipper.Core.Data.Enums;

namespace TimeSkipper {

    public class ModEntry : Mod {

        internal static ModEntry Instance;

        internal ModConfig Config;
        internal ModManager Manager;
        internal IModHelper Helper;

        internal ModConfig ActiveConfig => Config;

        public override void Entry(IModHelper helper) {

            Instance ??= this;

            // -----------------------------------------------

            I18n.Init(helper.Translation);

            // -----------------------------------------------

            Helper = helper;
            ReadConfig();

            // -----------------------------------------------

            Manager = new ModManager(helper, Config, Monitor, ModManifest);

            // -----------------------------------------------

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.SaveLoaded += OnLoad;
            helper.Events.Display.Rendered += OnRendered;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {

            if (!Context.IsWorldReady || !Manager.SkippingActive) {
                return;
            }

            if ((Game1.farmEvent != null || Game1.farmEventOverride != null) && Manager.SleepSchedule == SleepSchedule.farm_event_mode) {
                Manager.ResetSkippingState();
            }

            if (Game1.CurrentEvent != null && !Game1.CurrentEvent.skippable) {
                Manager.ResetSkippingState();
            }

            if (Game1.activeClickableMenu != null && Game1.activeClickableMenu is StardewValley.Menus.DialogueBox dialogue) {
                dialogue?.receiveLeftClick(0, 0);
            }

            //if (Context.IsWorldReady && Manager.SkippingActive && Manager.SleepSchedule == SleepSchedule.farm_event_mode) {
            //    if (Game1.farmEvent != null || Game1.farmEventOverride != null) {
            //        Manager.ResetSkippingState();
            //    }
            //}

            //if (Game1.CurrentEvent != null && !Game1.CurrentEvent.skippable && Manager.SkippingActive) {
            //    Manager.ResetSkippingState();
            //}

            //if (Context.IsWorldReady && Manager.SkippingActive && Game1.activeClickableMenu != null) {
            //    var menu = Game1.activeClickableMenu;

            //    if (menu is StardewValley.Menus.DialogueBox dialogue) {
            //        dialogue?.receiveLeftClick(0, 0);
            //    }
            //}
        }

        // =======================================================================================================

        private void ReadConfig() {
            try {
                Config = Helper.ReadConfig<ModConfig>();
            }
            catch (Exception e) {
                Config = new ModConfig();

                Helper.WriteConfig(Config);
                Monitor.LogOnce("Failed to load config.json, replaced with default one");
            }
        }

        public void ResetConfig() {
            Config = new ModConfig();
            Helper.WriteConfig(Config);
        }

        public void SaveConfig() {
            Helper.WriteConfig(Config);
        }

        // =======================================================================================================

        private void OnLoad(object sender, SaveLoadedEventArgs e) {
            ModManager.Instance.ResetSkippingState();
        }

        private void OnRendered(object sender, RenderedEventArgs e) {
            if (!Context.IsWorldReady)
                return;
            ModManager.Instance.OnRendered();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
            InitConfigMenu();
        }


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {

            if (ActiveConfig.GetShowMenuButton().IsDown() && Manager.SkippingActive) {
                Manager.ResetSkippingState();
            }

            // ignore if player hasn't loaded a save yet or free to move
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !Context.CanPlayerMove)
                return;

            else if (ActiveConfig.GetSkipOneDayButton().JustPressed()) {
                Manager.SkipOneDay();
            }

            else if (ActiveConfig.GetShowMenuButton().JustPressed()) {
                Manager.OnOpenMenu();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            Manager.OnDayStarted();
        }

        // ---------------------------------------------------------------------------------------

        private void InitConfigMenu() {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: ModManifest,
                reset: () => ResetConfig(),
                save: () => SaveConfig()
            );

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Main());

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetShowMenuButton(),
                setValue: keybind => ActiveConfig.SetShowMenuButton(keybind),
                name: () => I18n.Config_OpenMenuKeyName(),
                tooltip: () => I18n.Config_OpenMenuKeyDesc()
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetSkipOneDayButton(),
                setValue: keybind => ActiveConfig.SetSkipOneDayButton(keybind),
                name: () => I18n.Config_SkipOneDayKeyName(),
                tooltip: () => I18n.Config_SkipOneDayKeyDesc()
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetSleepSheduleMaxDays(),
                setValue: value => ActiveConfig.SetSleepSheduleMaxDays((int) value),
                name: () => I18n.Config_ScheduleMaxDaysName(),
                tooltip: () => I18n.Config_ScheduleMaxDaysDesc(),
                min: 7,
                max: 112,
                interval: 1
            );

            // ------------------------------------------------------------

            configMenu.AddSectionTitle(ModManifest, () => I18n.Config_Section_Misc());

            configMenu.AddParagraph(ModManifest, () => I18n.Config_Section_MiscNote());

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetDisableSavingWhileSkipping(),
                setValue: value => ActiveConfig.SetDisableSavingWhileSkipping(value),
                name: () => I18n.Config_DisableSavingName(),
                tooltip: () => I18n.Config_DisableSavingDesc()
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                getValue: () => ActiveConfig.GetDisableFading(),
                setValue: value => ActiveConfig.SetDisableFading(value),
                name: () => I18n.Config_DisableFadingName(),
                tooltip: () => I18n.Config_DisableFadingDesc()
            );
        }
    }
}