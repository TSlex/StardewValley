using ItemResearchSpawner.Components;
using ItemResearchSpawner.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;

        public override void Entry(IModHelper helper)
        {
            _config = helper.ReadConfig<ModConfig>();

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsPlayerFree)
            {
                return;
            }

            if (_config.ShowMenuKey.JustPressed())
            {
                Game1.activeClickableMenu = GetSpawnMenu();
            }
        }

        private IClickableMenu GetSpawnMenu()
        {
            return new SpawnMenu(Monitor);
        }
    }
}