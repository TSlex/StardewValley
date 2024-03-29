using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class CreativeMenu : InventoryMenu {

        public int ItemsPerView => capacity;
        public int ItemsPerRow => capacity / rows;

        public CreativeMenu(InventoryMenu menu) :
            base(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.playerInventory, menu.actualInventory,
                menu.highlightMethod, menu.capacity, menu.rows, menu.horizontalGap, menu.verticalGap, menu.drawSlots) {
        }

        // ============================================================================================================


        public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1) {
            //base.draw(b, red, green, blue);

            var color = ((red == -1) ? Color.White : new Color(
                    (int)Utility.Lerp(red, Math.Min(255, red + 150), 0.65f),
                    (int)Utility.Lerp(green, Math.Min(255, green + 150), 0.65f),
                    (int)Utility.Lerp(blue, Math.Min(255, blue + 150), 0.65f)));

            //var texture = ((red == -1) ? Game1.mouseCursors2 : Game1.uncoloredMenuTexture);

            var texture = Game1.mouseCursors;

            //if (drawSlots) {
            //    for (int j = 0; j < capacity; j++) {
            //        Vector2 vector = new Vector2(xPositionOnScreen + j % (capacity / rows) * 64 + horizontalGap * (j % (capacity / rows)), yPositionOnScreen + j / (capacity / rows) * (64 + verticalGap) + (j / (capacity / rows) - 1) * 4 - ((j < capacity / rows && playerInventory && verticalGap == 0) ? 12 : 0));
            //        b.Draw(texture, vector, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 10), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            //        //if ((playerInventory || showGrayedOutSlots) && j >= (int)Game1.player.maxItems) {
            //        //    b.Draw(texture, vector, Game1.getSourceRectForStandardTileSheet(Game1.menuTexture, 57), color * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            //        //}

            //        if (!Game1.options.gamepadControls && j < 12 && playerInventory) {
            //            string text = j switch {
            //                11 => "=",
            //                10 => "-",
            //                9 => "0",
            //                _ => (j + 1).ToString() ?? "",
            //            };
            //            Vector2 vector2 = Game1.tinyFont.MeasureString(text);
            //            b.DrawString(Game1.tinyFont, text, vector + new Vector2(32f - vector2.X / 2f, 0f - vector2.Y), (j == Game1.player.CurrentToolIndex) ? Color.Red : Color.DimGray);
            //        }
            //    }
            //}

            for (int l = 0; l < capacity; l++) {

                Vector2 location2 = new Vector2(
                    38 + xPositionOnScreen + l % (capacity / rows) * 64 + horizontalGap * (l % (capacity / rows)),
                    12 + yPositionOnScreen + l / (capacity / rows) * (64 + verticalGap) + (l / (capacity / rows) - 1) * 4 -
                    ((l < capacity / rows && playerInventory && verticalGap == 0) ? 12 : 0));

                //if (l % (capacity / rows / 2) == 0) {
                //    location2 = new Vector2(location2.X + 64, location2.Y);
                //}

                if ((l / (capacity / rows / 2) + 1) % 2 == 0) {
                    location2 = new Vector2(location2.X + 40, location2.Y);
                }

                b.Draw(texture, location2 - new Vector2(12, 12), new Rectangle(648, 841, 30, 30), color, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);


                if (actualInventory.Count > l && actualInventory[l] != null) {
                    bool flag = highlightMethod(actualInventory[l]);
                    //if (_iconShakeTimer.ContainsKey(l)) {
                    //    location2 += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                    //}

                    //var item = actualInventory[l];
                    //var itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                    //var scale = 1f;

                    //b.Draw(itemData.GetTexture(), location2 + new Vector2(32f, 32f) * scale, new Rectangle?(itemData.GetSourceRect()), 
                    //    Color.Black * 0.5f, 0.0f, Vector2.Zero, 4f * scale, SpriteEffects.None, 0.9f);

                    //// this drawn quality icons and stack
                    //item.DrawMenuIcons(b, location2, 1f, 1f, 0.9f, StackDrawType.HideButShowQuality, Color.White);

                    actualInventory[l].drawInMenu(b, location2, 1f, 1f, 0.865f, StackDrawType.HideButShowQuality, Color.White, flag);

                    //Utility.drawTinyDigits(num, sb, 
                    //    location + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(num, 3f * scale_size)) + 3f * scale_size, 
                    //    64f - 18f * scale_size + 1f), 
                    //    3f * scale_size, Math.Min(1f, layer_depth + 1E-06f), color);

                    Utility.drawTextWithColoredShadow(b, "100%", Game1.smallFont, location2 + new Vector2(4, -32), Color.Gold, Color.Red, 0.9f);
                }
            }
        }

        //public new List<Vector2> GetSlotDrawPositions() {
        //    List<Vector2> list = new List<Vector2>();
        //    for (int i = 0; i < capacity; i++) {
        //        list.Add(
        //            new Vector2(
        //                xPositionOnScreen + i % (capacity / rows) * 32 + horizontalGap * (i % (capacity / rows)), 
        //                yPositionOnScreen + i / (capacity / rows) * (32 + verticalGap) + (i / (capacity / rows) - 1) * 4 - ((i < capacity / rows && playerInventory && verticalGap == 0) ? 12 : 0)));
        //    }

        //    return list;
        //}

        #region OtherMethods
        public override void draw(SpriteBatch b) {
            base.draw(b);
        }

        public override void applyMovementKey(int direction) {
            base.applyMovementKey(direction);
        }

        public override bool areGamePadControlsImplemented() {
            return base.areGamePadControlsImplemented();
        }

        public override void automaticSnapBehavior(int direction, int oldRegion, int oldID) {
            base.automaticSnapBehavior(direction, oldRegion, oldID);
        }

        public override void clickAway() {
            base.clickAway();
        }


        public override void drawBackground(SpriteBatch b) {
            base.drawBackground(b);
        }

        public override void emergencyShutDown() {
            base.emergencyShutDown();
        }

        public override bool Equals(object obj) {
            return base.Equals(obj);
        }

        public override void gamePadButtonHeld(Buttons b) {
            base.gamePadButtonHeld(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
            base.gameWindowSizeChanged(oldBounds, newBounds);
        }

        public override ClickableComponent getCurrentlySnappedComponent() {
            return base.getCurrentlySnappedComponent();
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override bool IsActive() {
            return base.IsActive();
        }

        public override bool IsAutomaticSnapValid(int direction, ClickableComponent a, ClickableComponent b) {
            return base.IsAutomaticSnapValid(direction, a, b);
        }

        public override bool isWithinBounds(int x, int y) {
            return base.isWithinBounds(x, y);
        }

        public override void leftClickHeld(int x, int y) {
            base.leftClickHeld(x, y);
        }

        public override bool overrideSnappyMenuCursorMovementBan() {
            return base.overrideSnappyMenuCursorMovementBan();
        }

        public override void performHoverAction(int x, int y) {
            base.performHoverAction(x, y);
        }

        public override void populateClickableComponentList() {
            base.populateClickableComponentList();
        }

        public override bool readyToClose() {
            return base.readyToClose();
        }

        public override void receiveGamePadButton(Buttons b) {
            base.receiveGamePadButton(b);
        }

        public override void receiveKeyPress(Keys key) {
            base.receiveKeyPress(key);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            base.receiveRightClick(x, y, playSound);
        }

        public override void receiveScrollWheelAction(int direction) {
            base.receiveScrollWheelAction(direction);
        }

        public override void releaseLeftClick(int x, int y) {
            base.releaseLeftClick(x, y);
        }

        public override void setCurrentlySnappedComponentTo(int id) {
            base.setCurrentlySnappedComponentTo(id);
        }

        public override void setUpForGamePadMode() {
            base.setUpForGamePadMode();
        }

        public override bool shouldClampGamePadCursor() {
            return base.shouldClampGamePadCursor();
        }

        public override bool shouldDrawCloseButton() {
            return base.shouldDrawCloseButton();
        }

        public override bool showWithoutTransparencyIfOptionIsSet() {
            return base.showWithoutTransparencyIfOptionIsSet();
        }

        public override void snapCursorToCurrentSnappedComponent() {
            base.snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent() {
            base.snapToDefaultClickableComponent();
        }

        public override string ToString() {
            return base.ToString();
        }

        public override void update(GameTime time) {
            base.update(time);
        }

        protected override void actionOnRegionChange(int oldRegion, int newRegion) {
            base.actionOnRegionChange(oldRegion, newRegion);
        }

        protected override void cleanupBeforeExit() {
            base.cleanupBeforeExit();
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID) {
            base.customSnapBehavior(direction, oldRegion, oldID);
        }

        protected override void noSnappedComponentFound(int direction, int oldRegion, int oldID) {
            base.noSnappedComponentFound(direction, oldRegion, oldID);
        }

        protected override bool _ShouldAutoSnapPrioritizeAlignedElements() {
            return base._ShouldAutoSnapPrioritizeAlignedElements();
        }

        #endregion
    }
}
