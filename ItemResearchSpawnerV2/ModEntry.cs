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

            I18n.Init(helper.Translation);

            // -----------------------------------------------

            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.Saving += OnSave;
            helper.Events.GameLoop.SaveLoaded += OnLoad;
        }

        public void OnSave(object sender, SavingEventArgs saveLoadedEventArgs) {
            if (!Context.IsMainPlayer) {
                return;
            }

            _manager.OnSave();
        }

        public void OnLoad(object sender, SaveLoadedEventArgs saveLoadedEventArgs) {
            if (!Context.IsMainPlayer) {
                return;
            }

            _manager.OnLoad();
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {

            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || !Context.IsPlayerFree || !Context.CanPlayerMove)
                return;

            if (Game1.player.ActiveItem != null) {
                Monitor.Log(GetItemUniqueKey(Game1.player.ActiveItem));
            }

            // print button presses to the console window
            // this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);

            if (_config.ShowMenuButton.JustPressed()) {
                _manager.OpenMenu();
            }
        }

        public static string GetItemUniqueKey(Item item) {
            return $"{item.Name}:" + $"{item.ParentSheetIndex} | {item.Category}";
        }
    }
}
