using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using TimeSkipper.Core.UI;

namespace TimeSkipper.Core {
    internal class ModManager {

        public static ModManager Instance;

        public readonly IModHelper Helper;
        public readonly IMonitor Monitor;
        public readonly IManifest Manifest;
        public bool SkippingDay = false;

        public int DaysToSkip = 1;

        public int LastTileX = -1;
        public int LastTileY = -1;
        public int LastFacingDirection = 0;
        public string LastLocationName = "";

        public ScreenFade ScreenFade;

        public ModConfig Config;

        public readonly Texture2D UITexture;
        public static Texture2D UITextureInstance => Instance.UITexture;

        public ModManager(IModHelper helper, ModConfig config, IMonitor monitor, IManifest manifest) {

            // ---------------------------------------------------------------------

            Instance ??= this;
            if (Instance != this) {
                monitor.Log($"Another instance of {nameof(ModManager)} exists!", LogLevel.Warn);
                return;
            }

            // ---------------------------------------------------------------------

            Helper = helper;
            Config = config;
            Monitor = monitor;
            Manifest = manifest;

            UITexture = helper.ModContent.Load<Texture2D>(UIConstants.UISheet);

            // ---------------------------------------------------------------------

            ScreenFade = Helper.Reflection.GetField<ScreenFade>(typeof(Game1), "screenFade").GetValue();
        }

        public void OpenMenu() {
            //Monitor.Log($"{Game1.player.Name} tried to open time skipper menu.", LogLevel.Debug);
            Game1.activeClickableMenu = new TimeSkipperMenu();
            //SkipDay();
        }

        public void OnDayStarted() {
            if (SkippingDay) {
                SkippingDay = false;

                ScreenFade.FadeClear(0f);
                //Game1.warpFarmer(LastLocationName, LastTileX, LastTileY, 2);
                Game1.player.canMove = true;
            }
        }

        public void SkipDay() {
            if (SkippingDay) {
                return;
            }

            LastTileX = (int) Game1.player.Tile.X;
            LastTileY = (int) Game1.player.Tile.Y;
            LastFacingDirection = Game1.player.getFacingDirection();
            LastLocationName = Game1.currentLocation.ToString().Split('.')[Game1.currentLocation.ToString().Split('.').Length - 1];

            SkippingDay = true;
            PrepareNextDay();
            ScreenFade.FadeScreenToBlack(1.1f);
        }

        public void PrepareNextDay() {
            Game1.currentMinigame = null;
            Game1.newDay = true;
            Game1.newDaySync = new NewDaySynchronizer();

            if (Game1.player.isInBed.Value) {
                Game1.player.currentEyes = 1;
                Game1.player.blinkTimer = -4000;
                Game1.player.CanMove = false;
            }

            if (Game1.activeClickableMenu == null || Game1.dialogueUp) {
                return;
            }

            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
        }
    }
}
