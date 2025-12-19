using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.UI {

    internal struct ItemEffects {

    }

    internal class CreativeMenu : InventoryMenu {

        public int ItemsPerView => capacity;
        public int ItemsPerRow => capacity / rows;

        public float[] ItemOpacity;
        public bool AppearAnimComplete = false;
        public float InitialOpacity = 0.1f;

        public const int base_slot_id = 88111000;

        public List<ProgressionItem> ProgressionItems = new List<ProgressionItem>();

        public CreativeMenu(InventoryMenu menu, int rows, int capacity) :
            base(menu.xPositionOnScreen, menu.yPositionOnScreen, menu.playerInventory, menu.actualInventory,
                menu.highlightMethod, menu.capacity, menu.rows, menu.horizontalGap, menu.verticalGap, menu.drawSlots) {

            this.capacity = capacity;
            this.rows = rows;

            ItemOpacity = new float[capacity];
            Array.Fill(ItemOpacity, 1f);

            actualInventory.Clear();
            inventory.Clear();
        }

        public void Update(GameTime time) {
            float availableValue = 0.2f;
            float availablePerItem = 0f;
            float valueToAdd = 0;

            if (!AppearAnimComplete) {
                for (int i = 0; i < capacity; i++) {
                    availablePerItem = availableValue * 0.1f;

                    valueToAdd = ItemOpacity[i] + availablePerItem;
                    valueToAdd = valueToAdd >= 1f ? 1f - ItemOpacity[i] : availablePerItem;

                    ItemOpacity[i] = ItemOpacity[i] + valueToAdd;
                    availableValue -= valueToAdd;
                }

                AppearAnimComplete = !ItemOpacity.Any(x => x < 1f);
            }
        }

        public void SetItems(IEnumerable<ProgressionItem> items) {
            ProgressionItems.Clear();
            ProgressionItems.AddRange(items);

            actualInventory.Clear();

            foreach (var item in items) {
                actualInventory.Add(item.GameItem);
            }

            OnInventoryChange();
        }

        public void OnInventoryChange() {
            AppearAnimComplete = false;
            Array.Fill(ItemOpacity, 1f);

            if (ModManager.Instance.RecentlyUnlockedItemIndex >= 0 && ModManager.Instance.RecentlyUnlockedItemIndex < capacity) {
                ItemOpacity[ModManager.Instance.RecentlyUnlockedItemIndex] = 0f;

                ModManager.Instance.RecentlyUnlockedItemIndex = -1;
                ModManager.Instance.RecentlyUnlockedItem = null;
            }
        }

        public void RecreateItemSlots() {

            yPositionOnScreen -= 10 * 4;

            var slotScale = ModManager.Instance.Config.MenuSize.GetItemScale();
            var pageOffset = ModManager.Instance.Config.MenuSize.GetPageOffset();
            var rootX = ModManager.Instance.Config.MenuSize.GetRootX();
            var rootY = ModManager.Instance.Config.MenuSize.GetRootY();

            int slotRectSize = (int) (64 * slotScale);

            var middleID = capacity / 2;

            if (inventory.Count != 0) {
                for (int j = 0; j < capacity; j++) {
                    var itemBounds = new Rectangle(
                        rootX + xPositionOnScreen + j % ItemsPerRow * slotRectSize + horizontalGap * (j % ItemsPerRow),
                        rootY + yPositionOnScreen + j / ItemsPerRow * (slotRectSize + verticalGap) + (j / ItemsPerRow - 1) * 4 -
                        ((j <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0), slotRectSize, slotRectSize);

                    if (j >= middleID) {
                        itemBounds.X += (int) (ItemsPerRow * pageOffset) + 40;
                        itemBounds.Y = rootY + yPositionOnScreen + (j - middleID) / ItemsPerRow * (slotRectSize + verticalGap) + ((j - middleID) / ItemsPerRow - 1) * 4 -
                        (((j - middleID) <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0);
                    }

                    inventory[j].bounds = itemBounds;
                }

                return;
            }

            for (int j = 0; j < capacity; j++) {
                //(int top_id, int bottom_id, int left_id, int right_id) = GetNeightSlotsId(j);

                //var itemBounds = new Rectangle(
                //    38 + xPositionOnScreen + j % ItemsPerRow * 64 + horizontalGap * (j % ItemsPerRow),
                //    12 + yPositionOnScreen + j / ItemsPerRow * (64 + verticalGap) + (j / ItemsPerRow - 1) * 4 -
                //    ((j <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0), 64, 64);

                //if ((j / (ItemsPerRow / 2) + 1) % 2 == 0) {
                //    itemBounds.X += 40;
                //}

                var itemBounds = new Rectangle(
                    38 + xPositionOnScreen + j % ItemsPerRow * slotRectSize + horizontalGap * (j % ItemsPerRow),
                    rootY + yPositionOnScreen + j / ItemsPerRow * (slotRectSize + verticalGap) + (j / ItemsPerRow - 1) * 4 -
                    ((j <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0), slotRectSize, slotRectSize);

                if (j >= middleID) {
                    itemBounds.X += (int) (ItemsPerRow * pageOffset) + 40;
                    itemBounds.Y = rootY + yPositionOnScreen + (j - middleID) / ItemsPerRow * (slotRectSize + verticalGap) + ((j - middleID) / ItemsPerRow - 1) * 4 -
                    (((j - middleID) <= ItemsPerRow && playerInventory && verticalGap == 0) ? 12 : 0);
                }

                //int row = j / ItemsPerRow;
                //int j_in_row = j % ItemsPerRow;

                //int mR = row % 2;
                //int mC = row / 2;
                //int mJ = j_in_row + mC * (ItemsPerRow) + mR * (ItemsPerRow * 2);

                int myID = base_slot_id + J2mJ(j);

                int relativeID = myID - base_slot_id;

                int topID = (relativeID < ItemsPerRow * 2 ? -1 : relativeID - ItemsPerRow * 2) + base_slot_id;
                int bottomID = (relativeID >= capacity - ItemsPerRow * 2 ? -1 : relativeID + ItemsPerRow * 2) + base_slot_id;

                int leftID = (relativeID % (ItemsPerRow * 2) == 0 ? -1 : relativeID - 1) + base_slot_id;
                int rightID = (relativeID % (ItemsPerRow * 2) == ItemsPerRow * 2 - 1 ? -1 : relativeID + 1) + base_slot_id;

                inventory.Add(new ClickableComponent(itemBounds, j.ToString() ?? "") {
                    myID = myID,
                    leftNeighborID = leftID,
                    rightNeighborID = rightID,
                    downNeighborID = bottomID,
                    upNeighborID = topID,
                    region = 88111,
                    upNeighborImmutable = true,
                    downNeighborImmutable = true,
                    leftNeighborImmutable = true,
                    rightNeighborImmutable = true
                });
            }
        }

        public List<ClickableComponent> GetSlotAsTheyAppear() {
            return inventory.Select((cc, i) => (j: J2mJ(i), ccj: cc)).OrderBy(o => o.j).Select(o => o.ccj).ToList();
        }

        public int J2mJ(int j) {
            int row = j / ItemsPerRow;
            int j_in_row = j % ItemsPerRow;

            int rowsPerPage = capacity / 2 / ItemsPerRow;

            int mR = row % rowsPerPage;
            int mC = row / rowsPerPage;
            int mJ = j_in_row + mC * ItemsPerRow + mR * ItemsPerRow * 2;

            return mJ;
        }

        //private (int TOP_ID, int BOTTOM_ID, int LEFT_ID, int RIGHT_ID) GetNeightSlotsId(int idx) {
        //    int TOP_ID, BOTTOM_ID, LEFT_ID, RIGHT_ID;

        //    TOP_ID = ()

        //    //TOP_ID = (ID < ItemsPerRow) ? (12340 + ID) : (ID - ItemsPerRow);
        //    //LEFT_ID = (ID % ItemsPerRow != 0) ? (ID - 1) : 107;
        //    //RIGHT_ID = ((ID + 1) % ItemsPerRow != 0) ? (ID + 1) : 106;

        //    //if (!playerInventory) {
        //    //    BOTTOM_ID = (ID >= capacity - ItemsPerRow) ? (-99998) : (ID + ItemsPerRow);
        //    //}
        //    //else {
        //    //    BOTTOM_ID = (ID < actualInventory.Count - ItemsPerRow)
        //    //        ? (ID + ItemsPerRow)6tyu 
        //    //        : ((ID < actualInventory.Count - 3 && actualInventory.Count >= 36)
        //    //            ? (-99998)
        //    //            : ((ID % 12 < 2)
        //    //                ? 102
        //    //                : 101));
        //    //}

        //    //return (TOP_ID, BOTTOM_ID, LEFT_ID, RIGHT_ID);
        //}

        // ============================================================================================================


        public override void draw(SpriteBatch b, int red = -1, int green = -1, int blue = -1) {
            for (int i = 0; i < inventory.Count; i++) {
                if (_iconShakeTimer.TryGetValue(i, out var value) && Game1.currentGameTime.TotalGameTime.TotalSeconds >= value) {
                    _iconShakeTimer.Remove(i);
                }
            }

            var color = ((red == -1) ? Color.White : new Color(
                    (int) Utility.Lerp(red, Math.Min(255, red + 150), 0.65f),
                    (int) Utility.Lerp(green, Math.Min(255, green + 150), 0.65f),
                    (int) Utility.Lerp(blue, Math.Min(255, blue + 150), 0.65f)));

            var texture = ModManager.UITextureInstance;

            for (int j = 0; j < capacity; j++) {

                var slot = inventory[j];
                var location = new Vector2(slot.bounds.X, slot.bounds.Y);

                var c = (ProgressionItems.ElementAtOrDefault(j)?.Favorited ?? false) ? Color.Gold * 0.5f : Color.White;

                //b.Draw(texture, location - new Vector2(12, 12), new Rectangle(648, 841, 30, 30), c, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0.5f);
                b.Draw(texture, location - new Vector2(8, 8) * ModManager.Instance.Config.MenuSize.GetItemScale(), UIConstants.ItemCell, c, 0f, Vector2.Zero, 1f * ModManager.Instance.Config.MenuSize.GetItemScale(), SpriteEffects.None, 0.5f);

                if (actualInventory.Count <= j || actualInventory[j] == null) {
                    continue;
                }

                var opacity = ItemOpacity[j];
                var flag = highlightMethod(actualInventory[j]);

                if (_iconShakeTimer.ContainsKey(j)) {
                    location += 2f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) * (1f - opacity);
                    //location += 10f * new Vector2(0, 1) * (1f - opacity) * MathF.Cos(j);
                }

                opacity = MathF.Min(opacity, flag ? 1f : 0.25f);

                float itemScale = ((inventory.Count > j) ? inventory[j].scale : 1f) * ModManager.Instance.Config.MenuSize.GetItemScale();

                Vector2 itemImageLocation = location + ModManager.Instance.Config.MenuSize.GetItemImageOffcet();

                float textScale = ModManager.Instance.Config.MenuSize.GetItemScale();

                if (!(ProgressionItems.ElementAtOrDefault(j)?.ResearchCompleted ?? false)) {
                    actualInventory[j].drawInMenu(b,
                        itemImageLocation, itemScale, 1f, 0.865f,
                        StackDrawType.HideButShowQuality,
                        Color.Black * 0.25f,
                        flag);

                    var progressText = $"{ProgressionItems[j].ResearchPerc}%";

                    Utility.drawTextWithColoredShadow(b,
                        progressText,
                        Game1.smallFont, location + new Vector2(Game1.smallFont.MeasureString(progressText).X * 0.01f + 16f * textScale, (32 / 2 + 4f) * textScale),
                        Color.Gold, Color.Red * 0.5f, 0.9f * textScale);

                    Utility.drawTextWithColoredShadow(b,
                        progressText,
                        Game1.smallFont, location + new Vector2(Game1.smallFont.MeasureString(progressText).X * 0.01f + 17f * textScale, (32 / 2 + 4f) * textScale),
                        Color.Gold, Color.Red * 0.5f, 0.9f * textScale);
                }
                else if (ModManager.Instance.ShouldDisableItemByPrice(actualInventory[j])) {
                    actualInventory[j].drawInMenu(b, itemImageLocation, itemScale, 1f, 0.865f, StackDrawType.Draw, Color.White * 0.25f, flag);
                }
                else {
                    actualInventory[j].drawInMenu(b, itemImageLocation, itemScale, 1f, 0.865f, StackDrawType.Draw, Color.White * opacity, flag);
                }

                Vector2 favoriteIconPosition = ModManager.Instance.Config.MenuSize.GetFavoriteOffcet();

                if (ProgressionItems.ElementAtOrDefault(j)?.Favorited ?? false) {
                    b.Draw(texture, location + favoriteIconPosition, UIConstants.FavoriteItemIcon, Color.White, 0f, Vector2.Zero, 1f * ModManager.Instance.Config.MenuSize.GetItemScale(), SpriteEffects.None, 0.5f);
                }

                //actualInventory[j].drawInMenu(b, location, (inventory.Count > j) ? inventory[j].scale : 1f, 1f, 0.865f, StackDrawType.Draw_OneInclusive, Color.White * opacity, flag);

                //Utility.drawTextWithColoredShadow(b, "100%", Game1.smallFont, location + new Vector2(4, -32), Color.Gold, Color.Red * (flag ? 1f : 0.25f), 0.9f);
            }
        }

        public Item HandleLeftClick(int x, int y, Item toPlace, bool playSound = true) {

            foreach (var (item, i) in inventory.Select((value, i) => (value, i))) {
                if (!item.containsPoint(x, y)) {
                    continue;
                }

                int num = Convert.ToInt32(item.name);

                if (num >= actualInventory.Count || (actualInventory[num] != null && !highlightMethod(actualInventory[num]) && !actualInventory[num].canStackWith(toPlace))) {
                    continue;
                }

                if (actualInventory[num] != null && (ModManager.Instance.Helper.Input.IsDown(SButton.LeftAlt) || ModManager.Instance.Helper.Input.IsDown(SButton.RightShoulder))) {
                    ModManager.ProgressionManagerInstance.FavoriteItem(ProgressionItems[i]);
                    return toPlace;
                }

                if (actualInventory[num] != null && ModManager.Instance.ShouldDisableItemByPrice(actualInventory[num])) {
                    continue;
                }

                if (actualInventory[num] != null) {

                    if (ProgressionItems[i] == null || !ProgressionItems[i].ResearchCompleted || ProgressionItems[i].CannotResearch) {
                        return toPlace;
                    }

                    if (toPlace != null) {
                        if (playSound) {
                            Game1.playSound("stoneStep");
                        }

                        return Utility.addItemToInventory(toPlace, num, actualInventory, onAddItem);
                    }

                    if (playSound) {
                        Game1.playSound(moveItemSound);
                    }

                    return Utility.removeItemFromInventory(num, actualInventory);
                }

                if (toPlace != null) {
                    if (playSound) {
                        Game1.playSound("stoneStep");
                    }

                    return Utility.addItemToInventory(toPlace, num, actualInventory, onAddItem);
                }
            }

            return toPlace;
        }

        public Item HandleRightClick(int x, int y, Item toAddTo, bool playSound = true, bool onlyCheckToolAttachments = false) {
            foreach (var (item, i) in inventory.Select((value, i) => (value, i))) {
                int num = Convert.ToInt32(item.name);
                if (!item.containsPoint(x, y) || num >= actualInventory.Count || (actualInventory[num] != null && !highlightMethod(actualInventory[num])) || num >= actualInventory.Count || actualInventory[num] == null) {
                    continue;
                }

                if (actualInventory[num] != null && ModManager.Instance.ShouldDisableItemByPrice(actualInventory[num])) {
                    continue;
                }

                if (ProgressionItems[i] == null || !ProgressionItems[i].ResearchCompleted || ProgressionItems[i].CannotResearch) {
                    return toAddTo;
                }

                if (actualInventory[num] is Tool tool && (toAddTo == null || toAddTo is SObject) && tool.canThisBeAttached((SObject) toAddTo)) {
                    return tool.attach((SObject) toAddTo);
                }

                if (onlyCheckToolAttachments) {
                    return toAddTo;
                }

                if (toAddTo == null) {
                    if (actualInventory[num].maximumStackSize() != -1) {
                        if (num == Game1.player.CurrentToolIndex && actualInventory[num] != null && actualInventory[num].Stack == 1) {
                            actualInventory[num].actionWhenStopBeingHeld(Game1.player);
                        }

                        Item one = actualInventory[num].getOne();

                        if (actualInventory[num].Stack > 1) {
                            if (ModManager.Instance.Helper.Input.IsDown(SButton.LeftShift) || ModManager.Instance.Helper.Input.IsDown(SButton.LeftTrigger)) {
                                int val = (int) Math.Ceiling((double) actualInventory[num].Stack / 2.0);
                                one.Stack = val;
                                actualInventory[num].Stack -= val;
                            }
                            else if (ModManager.Instance.Helper.Input.IsDown(SButton.LeftControl) || ModManager.Instance.Helper.Input.IsDown(SButton.LeftShoulder)) {
                                int val = Math.Min(10, actualInventory[num].Stack);
                                one.Stack = val;
                                actualInventory[num].Stack -= val;
                            }
                        }
                        else if (actualInventory[num].Stack == 1) {
                            actualInventory[num] = null;
                        }
                        else {
                            actualInventory[num].Stack--;
                        }

                        if (actualInventory[num] != null && actualInventory[num].Stack <= 0) {
                            actualInventory[num] = null;
                        }

                        if (playSound) {
                            Game1.playSound(moveItemSound);
                        }

                        return one;
                    }
                }
                else {
                    if (!actualInventory[num].canStackWith(toAddTo) || toAddTo.Stack >= toAddTo.maximumStackSize()) {
                        continue;
                    }

                    if (ModManager.Instance.Helper.Input.IsDown(SButton.LeftShift) || ModManager.Instance.Helper.Input.IsDown(SButton.LeftTrigger)) {
                        int val = (int) Math.Ceiling((double) actualInventory[num].Stack / 2.0);
                        val = Math.Min(toAddTo.maximumStackSize() - toAddTo.Stack, val);
                        toAddTo.Stack += val;
                        actualInventory[num].Stack -= val;
                    }
                    else if (ModManager.Instance.Helper.Input.IsDown(SButton.LeftControl) || ModManager.Instance.Helper.Input.IsDown(SButton.LeftShoulder)) {
                        int val = Math.Min(10, actualInventory[num].Stack);
                        val = Math.Min(toAddTo.maximumStackSize() - toAddTo.Stack, val);
                        toAddTo.Stack += val;
                        actualInventory[num].Stack -= val;
                    }
                    else {
                        toAddTo.Stack++;
                        actualInventory[num].Stack--;
                    }

                    if (playSound) {
                        Game1.playSound(moveItemSound);
                    }

                    if (actualInventory[num].Stack <= 0) {
                        if (num == Game1.player.CurrentToolIndex) {
                            actualInventory[num].actionWhenStopBeingHeld(Game1.player);
                        }

                        actualInventory[num] = null;
                    }

                    return toAddTo;
                }
            }

            return toAddTo;
        }

        #region OtherMethods

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            base.receiveRightClick(x, y, playSound);
        }

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