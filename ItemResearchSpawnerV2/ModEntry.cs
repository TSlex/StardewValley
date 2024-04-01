using ItemResearchSpawnerV2.Core;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;


namespace ItemResearchSpawnerV2 {
    public class ModEntry : Mod {

        private ModConfig _config;
        private ModManager _manager;

        public override void Entry(IModHelper helper) {

            try {
                _config = helper.ReadConfig<ModConfig>();
            }
            catch (Exception e) {
                _config = new ModConfig();
                helper.WriteConfig(_config);
                Monitor.LogOnce("Failed to load config.json, replaced with default one");
            }

            // -----------------------------------------------

            _manager = new ModManager(helper, _config, Monitor, ModManifest);

            // -----------------------------------------------

            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !Context.CanPlayerMove)
                return;

            // print button presses to the console window
            // this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            if (_config.ShowMenuButton.JustPressed()) {
                _manager.OpenMenu();
            }
        }
    }
}
