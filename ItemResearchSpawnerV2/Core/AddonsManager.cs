using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core {
    internal class AddonsManager {
        public static AddonsManager Instance;

        public const string RNSPlus_MOD_ID = "TSlex.ItemResearchSpawnerCPPlus";

        public const string RNS_unlock_stat_ID = $"{RNSPlus_MOD_ID}_RNS_Book_Unlock";

        public const string RNS_book_ID = $"{RNSPlus_MOD_ID}_RNS_Book_Unlock";
        public const string RNS_book_UK = $"{RNS_book_ID}:0";
        public const string RNS_orb_ID = $"{RNSPlus_MOD_ID}_Orb";
        public const string RNS_orb_UK = $"{RNS_orb_ID}:1";

        public const string RNS_book_tag = $"{RNSPlus_MOD_ID}_BookFound";
        public const string RNS_orb_tag = $"{RNSPlus_MOD_ID}_OrbFound";

        public const string RNS_strings_orbDiscoveryDialog = $"{RNSPlus_MOD_ID}_OrbDiscoveryDialog";
        public const string RNS_strings_bookDiscoveryDialog = $"{RNSPlus_MOD_ID}_BookDiscoveryDialog";

        private Item OrbItemMask = null;
        private Item BookItemMask = null;

        public bool RNSPlusInstalled = false;

        public readonly IModHelper Helper;

        public AddonsManager(IModHelper helper) {

            // ---------------------------------------------------------------------

            Instance ??= this;
            if (Instance != this) {
                return;
            }

            // ---------------------------------------------------------------------

            Helper = helper;

            // ---------------------------------------------------------------------

            RNSPlusInstalled = Helper.ModRegistry.IsLoaded(RNSPlus_MOD_ID);
        }

        // ----------------------------------------------------------------------------------------------------------------

        public void Update() {
            if (RNSPlusInstalled && ModEntry.Instance.IsSaveActive) {
                RNSPlusUpdate();
            }
        }

        // ----------------------------------------------------------------------------------------------------------------

        public bool CanOpenBook() {
            return !RNSPlusInstalled || Game1.player.stats.Get(RNS_unlock_stat_ID) == 1;
        }

        public void RNSPlusUpdate() {

            InitItemMasks();

            CheckAndDiscoverIfNeeded(RNS_book_tag, BookItemMask, false);
            CheckAndDiscoverIfNeeded(RNS_orb_tag, OrbItemMask, false);

            //Item orb = ItemRegistry.Create(RNS_orb_ID);
            //int orbInInventoryIndex = Game1.player.getIndexOfInventoryItem(orb);

            //if (orbInInventoryIndex != -1 && Game1.player.Items[orbInInventoryIndex] is StardewValley.Object obj) {
            //    obj.questItem.Set(true);
            //}

            ////Game1.player.mailReceived.Remove(RNS_orb_tag);

            //// orb discovery logic
            //if (!Game1.player.mailReceived.Contains(RNS_orb_tag)) {

            //    if (orbInInventoryIndex != -1) {
            //        Game1.activeClickableMenu?.exitThisMenuNoSound();
            //        Game1.player.mailReceived.Add(RNS_orb_tag);

            //        Game1.player.Halt();
            //        Game1.player.faceDirection(2);
            //        Game1.player.holdUpItemThenMessage(orb, showMessage: false);
            //        Game1.player.jitterStrength = 1f;

            //        Game1.pauseThenDoFunction(7000, new Game1.afterFadeFunction(SetOrbDiscovered));

            //        Game1.changeMusicTrack("none", music_context: StardewValley.GameData.MusicContext.Event);
            //        Game1.playSound("crit");
            //        Game1.screenGlowOnce(new Microsoft.Xna.Framework.Color(30, 0, 150), true, 0.01f, 0.999f);
            //        DelayedAction.playSoundAfterDelay("stardrop", 1500);
            //        Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(
            //                new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
            //                500, Microsoft.Xna.Framework.Color.White, 10, 2000));
            //        Game1.afterDialogues += () => Game1.stopMusicTrack(StardewValley.GameData.MusicContext.Event);
            //    }
            //}
        }

        private void InitItemMasks() {
            OrbItemMask ??= ItemRegistry.Create(RNS_orb_ID);
            BookItemMask ??= ItemRegistry.Create(RNS_book_ID);
        }

        private void CheckAndDiscoverIfNeeded(string foundTag, Item itemMatch, bool preserveAsQuest = false) {
            if (Game1.player.mailReceived.Contains(foundTag)) {
                return;
            }

            int matchedAt = Game1.player.getIndexOfInventoryItem(itemMatch);

            if (preserveAsQuest && matchedAt >= 0 && Game1.player.Items[matchedAt] is StardewValley.Object obj) {
                obj.questItem.Set(true);
            }

            if (matchedAt >= 0) {
                Game1.activeClickableMenu?.exitThisMenuNoSound();
                Game1.player.mailReceived.Add(foundTag);

                Game1.player.Halt();
                Game1.player.faceDirection(2);

                Game1.player.holdUpItemThenMessage(itemMatch, showMessage: false);
                Game1.player.jitterStrength = 1f;

                Game1.pauseThenDoFunction(7000, new Game1.afterFadeFunction(() => FinishItemDiscovery(itemMatch)));

                Game1.changeMusicTrack("none", music_context: StardewValley.GameData.MusicContext.Event);
                Game1.playSound("crit");
                Game1.screenGlowOnce(new Microsoft.Xna.Framework.Color(130, 0, 100), true, 0.01f, 0.999f);
                DelayedAction.playSoundAfterDelay("stardrop", 1500);
                Game1.screenOverlayTempSprites.AddRange(Utility.sparkleWithinArea(
                        new Microsoft.Xna.Framework.Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height),
                        500, Microsoft.Xna.Framework.Color.White, 10, 2000));
                Game1.afterDialogues += () => Game1.stopMusicTrack(StardewValley.GameData.MusicContext.Event);
            }
        }

        private void FinishItemDiscovery(Item itemMatch) {
            Game1.flashAlpha = 1f;

            var recievedItem = ItemRegistry.Create(itemMatch.ItemId);

            Game1.player.completelyStopAnimatingOrDoingAction();
            Game1.MusicDuckTimer = 2000f;
            DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
            Game1.player.faceDirection(2);

            Game1.player.freezePause = 4000;

            Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
            {
                new FarmerSprite.AnimationFrame(57, 0),
                new FarmerSprite.AnimationFrame(57, 2500, secondaryArm: false, flip: false, delegate(Farmer who)
                {
                    Farmer.showHoldingItem(who, recievedItem);
                }),
                new FarmerSprite.AnimationFrame((short) Game1.player.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false, delegate(Farmer who)
                {
                    Game1.drawObjectDialogue(new List<string> { GetItemDiscoveryDialog(itemMatch) });
                    who.completelyStopAnimatingOrDoingAction();
                }, behaviorAtEndOfFrame: true)
            });

            Game1.player.mostRecentlyGrabbedItem = recievedItem;
            Game1.player.canMove = false;

            Game1.player.jitterStrength = 0.0f;
            Game1.screenGlowHold = false;
        }

        private string GetItemDiscoveryDialog(Item itemMatch) {
            return itemMatch.ItemId switch {
                RNS_orb_ID => Game1.content.LoadString($"Strings\\StringsFromCSFiles:{RNS_strings_orbDiscoveryDialog}"),
                RNS_book_ID => Game1.content.LoadString($"Strings\\StringsFromCSFiles:{RNS_strings_bookDiscoveryDialog}"),
                _ => "???",
            };
        }

        //private void SetOrbDiscovered() {
        //    Game1.flashAlpha = 1f;

        //    //Game1.player.holdUpItemThenMessage(ItemRegistry.Create(RNS_orb_ID), showMessage: false);

        //    var recievedOrb = ItemRegistry.Create(RNS_orb_ID);

        //    Game1.player.completelyStopAnimatingOrDoingAction();
        //    Game1.MusicDuckTimer = 2000f;
        //    DelayedAction.playSoundAfterDelay("getNewSpecialItem", 750);
        //    Game1.player.faceDirection(2);

        //    Game1.player.freezePause = 4000;

        //    Game1.player.FarmerSprite.animateOnce(new FarmerSprite.AnimationFrame[3]
        //    {
        //        new FarmerSprite.AnimationFrame(57, 0),
        //        new FarmerSprite.AnimationFrame(57, 2500, secondaryArm: false, flip: false, delegate(Farmer who)
        //        {
        //            Farmer.showHoldingItem(who, recievedOrb);
        //        }),
        //        new FarmerSprite.AnimationFrame((short) Game1.player.FarmerSprite.CurrentFrame, 500, secondaryArm: false, flip: false, delegate(Farmer who)
        //        {
        //            Game1.drawObjectDialogue(new List<string> { Game1.content.LoadString($"Strings\\StringsFromCSFiles:{RNS_strings_orbDiscoveryDialog}") });
        //            who.completelyStopAnimatingOrDoingAction();
        //        }, behaviorAtEndOfFrame: true)
        //    });

        //    Game1.player.mostRecentlyGrabbedItem = recievedOrb;
        //    Game1.player.canMove = false;

        //    Game1.player.jitterStrength = 0.0f;
        //    Game1.screenGlowHold = false;
        //}
    }
}
