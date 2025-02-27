using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using TimeSkipper.Core.Data.Enums;
using TimeSkipper.Core.UI;

namespace TimeSkipper.Core {
    internal class ModManager {

        public static ModManager Instance;

        public readonly IModHelper Helper;
        public readonly IMonitor Monitor;
        public readonly IManifest Manifest;

        public bool SkippingActive = false;
        public bool SkippingTriggered = false;
        public int DaysToSkip = 1;
        public SleepSchedule SleepSchedule = SleepSchedule.calendar_mode;

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

        public void ResetSkippingState() {
            SkippingActive = false;
            SkippingTriggered = false;
            DaysToSkip = 1;

            if (Config.DisableSavingWhileSkipping) {
                Game1.saveOnNewDay = true;
            }
        }

        public void OnOpenMenu() {
            //Monitor.Log($"{Game1.player.Name} tried to open time skipper menu.", LogLevel.Debug);
            Game1.activeClickableMenu = new TimeSkipperMenu();
        }

        public void SkipOneDay() {
            //Monitor.Log($"{Game1.player.Name} tried to skip one day.", LogLevel.Debug);
            DaysToSkip = 1;
            SleepSchedule = SleepSchedule.calendar_mode;
            StartSkipping();
        }

        public void StartSkipping() {
            SkippingActive = true;

            if (SleepSchedule != SleepSchedule.calendar_mode) {
                DaysToSkip = Config.SleepSheduleMaxDays;
            }

            SkipDay();
        }

        // --------------------------------------------------------------------------------------------------

        public void OnRendered() {
            if (!SkippingActive) {
                return;
            }

            var scale = 1.5f;
            var skippingText = $"{I18n.Info_Skipping()} {DaysToSkip} {(DaysToSkip > 1 ? I18n.Menu_Days() : I18n.Menu_Day())}";
            skippingText = SleepSchedule != SleepSchedule.calendar_mode ? SleepSchedule.GetSkippingNote() : skippingText;

            var skippingTextWidth = Game1.smallFont.MeasureString(skippingText) * scale;

            Utility.drawTextWithColoredShadow(Game1.spriteBatch,
                skippingText, Game1.smallFont,
                new Vector2(Game1.viewport.Width / 2, Game1.viewport.Height / 2) + new Vector2(-skippingTextWidth.X / 2, -skippingTextWidth.Y),
                Color.LightGreen, Color.Black, scale);

            if (!SkippingActive || DaysToSkip <= 1) {
                return;
            }

            var abortText = $"{string.Format(I18n.Info_Abort(), Config.ShowMenuButton)}";
            var abortTextWidth = Game1.smallFont.MeasureString(abortText);

            Utility.drawTextWithColoredShadow(Game1.spriteBatch,
                abortText, Game1.smallFont,
                new Vector2(Game1.viewport.Width / 2, Game1.viewport.Height / 2) + new Vector2(-abortTextWidth.X / 2, +4 * 2),
                Color.LightPink, Color.Cyan, 1f, verticalShadowOffset: 0, horizontalShadowOffset: 0);
        }

        // --------------------------------------------------------------------------------------------------

        public void OnDayStarted() {
            SkippingTriggered = false;

            if (SkippingActive) {
                DaysToSkip--;

                if (!GetShouldContinueSkipping()) {
                    ResetSkippingState();
                    return;
                }
                else {
                    SkipDay();
                }
            }
        }

        public bool GetShouldContinueSkipping() {

            var todayBadWeather = Game1.IsRainingHere() || Game1.IsGreenRainingHere() || Game1.IsLightningHere() || Game1.IsSnowingHere();
            var playerLuck = Game1.player.hasSpecialCharm ? Game1.player.DailyLuck - 0.025f : Game1.player.DailyLuck;
            var buildingsUnderConstruct = Game1.IsThereABuildingUnderConstruction(Game1.builder_robin) || Game1.IsThereABuildingUnderConstruction(Game1.builder_wizard);
            var isEventToday = Utility.isFestivalDay() || Utility.IsPassiveFestivalDay() || Utility.getDaysOfBooksellerThisSeason().Contains(Game1.dayOfMonth);

            if (SleepSchedule == SleepSchedule.rainy_mode) {
                if (todayBadWeather) {
                    return false;
                }
            }
            else if (SleepSchedule == SleepSchedule.sunny_mode) {
                if (!todayBadWeather) {
                    return false;
                }
            }

            else if (SleepSchedule == SleepSchedule.lucky_mode) {
                if (playerLuck >= 0.07f) {
                    return false;
                }
            }

            else if (SleepSchedule == SleepSchedule.unlucky_mode) {
                if (playerLuck <= -0.07f) {
                    return false;
                }
            }

            else if (SleepSchedule == SleepSchedule.event_mode) {
                if (isEventToday) {
                    return false;
                }
            }

            else if (SleepSchedule == SleepSchedule.building_completed_mode) {
                if (!buildingsUnderConstruct) {
                    return false;
                }
            }

            return DaysToSkip > 0;
        }

        public void SkipDay() {
            if (SkippingTriggered) {
                return;
            }

            SkippingTriggered = true;

            LastTileX = (int) Game1.player.Tile.X;
            LastTileY = (int) Game1.player.Tile.Y;
            LastFacingDirection = Game1.player.getFacingDirection();
            LastLocationName = Game1.currentLocation.ToString().Split('.')[Game1.currentLocation.ToString().Split('.').Length - 1];

            PrepareNextDay();

            if (Config.DisableFading) {
                ScreenFade.FadeScreenToBlack(1.1f);
            }
            else {
                ScreenFade.FadeScreenToBlack();
            }
        }

        public void PrepareNextDay() {

            if (Config.DisableSavingWhileSkipping) {
                if (DaysToSkip > 1) {
                    Game1.saveOnNewDay = false;
                }
                else {
                    Game1.saveOnNewDay = true;
                }
            }

            Game1.currentMinigame = null;
            Game1.newDay = true;
            Game1.newDaySync = new NewDaySynchronizer();

            if (Game1.activeClickableMenu == null || Game1.dialogueUp) {
                return;
            }

            Game1.activeClickableMenu.emergencyShutDown();
            Game1.exitActiveMenu();
        }
    }
}
