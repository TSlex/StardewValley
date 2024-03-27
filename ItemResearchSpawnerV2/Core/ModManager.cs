using ItemResearchSpawnerV2.Core.UI;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core {
    internal class ModManager {

        private IModHelper helper;

        public ModManager(IModHelper helper) {
            this.helper = helper;
        }

        public void OpenMenu() {
            Game1.activeClickableMenu = new MainMenu();
        }
    }
}
