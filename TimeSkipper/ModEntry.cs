using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TimeSkipper.Core;

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


        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
            // ignore if player hasn't loaded a save yet or free to move
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !Context.CanPlayerMove)
                return;

            if (ActiveConfig.GetShowMenuButton().JustPressed()) {
                Manager.OpenMenu();
            }
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            Manager.OnDayStarted();
        }
    }
}