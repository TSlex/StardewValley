using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class CreativeMenu : InventoryMenu {

        public int ItemsPerView => capacity;
        public int ItemsPerRow => capacity / rows;

        public CreativeMenu(InventoryMenu menu) :
            base(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.playerInventory, menu.actualInventory,
                menu.highlightMethod, menu.capacity, menu.rows, menu.horizontalGap, menu.verticalGap, menu.drawSlots) {
        }

        public void RecreateItemSlots() {
            inventory.Clear();

            for (int j = 0; j < capacity; j++) {
                (int top_id, int bottom_id, int left_id, int right_id) = GetNeightSlotsId(j);

                var itemBounds = new Rectangle(
                    38 + xPositionOnScreen + j % ItemsPerRow * 64 + horizontalGap * (j % ItemsPerRow),
                    12 + yPositionOnScreen + j / ItemsPerRow * (64 + verticalGap) + (j / ItemsPerRow - 1) * 4 -
                    ((j <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0), 64, 64);

                if ((j / (ItemsPerRow / 2) + 1) % 2 == 0) {
                    itemBounds.X += 40;
                }

                inventory.Add(new ClickableComponent(itemBounds, j.ToString() ?? "") {
                    myID = j,
                    leftNeighborID = left_id,
                    rightNeighborID = right_id,
                    downNeighborID = bottom_id,
                    upNeighborID = top_id,
                    region = 9000,
                    upNeighborImmutable = true,
                    downNeighborImmutable = true,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
            }
        }

        private (int TOP_ID, int BOTTOM_ID, int LEFT_ID, int RIGHT_ID) GetNeightSlotsId(int ID) {
            int TOP_ID, BOTTOM_ID, LEFT_ID, RIGHT_ID;

            TOP_ID = (ID < ItemsPerRow) ? (12340 + ID) : (ID - ItemsPerRow);
            LEFT_ID = (ID % ItemsPerRow != 0) ? (ID - 1) : 107;
            RIGHT_ID = ((ID + 1) % ItemsPerRow != 0) ? (ID + 1) : 106;

            if (!playerInventory) {
                BOTTOM_ID = (ID >= capacity - ItemsPerRow) ? (-99998) : (ID + ItemsPerRow);
            }
            else {
                BOTTOM_ID = (ID < actualInventory.Count - ItemsPerRow)
                    ? (ID + ItemsPerRow)
                    : ((ID < actualInventory.Count - 3 && actualInventory.Count >= 36)
                        ? (-99998)
                        : ((ID % 12 < 2)
                            ? 102
                            : 101));
            }

            return (TOP_ID, BOTTOM_ID, LEFT_ID, RIGHT_ID);
        }

        // ============================================================================================================


        public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1) {
            for (int i = 0; i < inventory.Count; i++) {
                if (_iconShakeTimer.TryGetValue(i, out var value) && Game1.currentGameTime.TotalGameTime.TotalSeconds >= value) {
                    _iconShakeTimer.Remove(i);
                }
            }

            var color = ((red == -1) ? Color.White : new Color(
                    (int)Utility.Lerp(red, Math.Min(255, red + 150), 0.65f),
                    (int)Utility.Lerp(green, Math.Min(255, green + 150), 0.65f),
                    (int)Utility.Lerp(blue, Math.Min(255, blue + 150), 0.65f)));

            var texture = Game1.mouseCursors;

            for (int j = 0; j < capacity; j++) {

                var slot = inventory[j];
                var location = new Vector2(slot.bounds.X, slot.bounds.Y);

                b.Draw(texture, location - new Vector2(12, 12), new Rectangle(648, 841, 30, 30), color, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);

                if (actualInventory.Count <= j || actualInventory[j] == null) {
                    continue;
                }

                var flag = highlightMethod(actualInventory[j]);

                if (_iconShakeTimer.ContainsKey(j)) {
                    location += 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                }

                actualInventory[j].drawInMenu(b, location, (inventory.Count > j) ? inventory[j].scale : 1f, 1f, 0.865f, StackDrawType.HideButShowQuality, Color.White * (flag ? 1f: 0.25f), flag);

                Utility.drawTextWithColoredShadow(b, "100%", Game1.smallFont, location + new Vector2(4, -32), Color.Gold, Color.Red * (flag ? 1f : 0.25f), 0.9f);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            base.receiveRightClick(x, y, playSound);
        }

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