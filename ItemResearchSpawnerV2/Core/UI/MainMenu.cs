using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Inventories;
using StardewValley.Menus;
using System.Threading;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class MainMenu : ItemGrabMenu {

        protected readonly CreativeMenu CreativeMenu;

        private readonly CashTab CashTab;
        private readonly QualityButton QualityButton;
        private readonly FavoriteButton FavoriteButton;
        private readonly DisplayButton DisplayButton;
        private readonly SettingsButton SettingsButton;
        private readonly Dropdown<string> CategoryDropdown;
        private readonly Dropdown<string> SortDropdown;
        private readonly SearchBar ItemSearchBarTab;
        private readonly ItemResearchArea ItemResearchArea;
        private readonly ClickableTextureComponent LeftArrow;
        private readonly ClickableTextureComponent RightArrow;

        private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        public MainMenu() :
            base(
                inventory: new List<Item>(),
                reverseGrab: false,
                showReceivingMenu: false,
                highlightFunction: _ => true,
                behaviorOnItemGrab: (_, _) => { },
                behaviorOnItemSelectFunction: (_, _) => { },
                message: null,
                snapToBottom: false,
                canBeExitedWithKey: true,
                playRightClickSound: true,
                allowRightClick: true,
                showOrganizeButton: false,
                sourceItem: null,
                whichSpecialButton: -1,
                context: null,
                heldItemExitBehavior: ItemExitBehavior.ReturnToPlayer,
                allowExitWithHeldItem: true,
                source: IsAndroid ? source_chest : source_none) {

            // =========================================================================

            xPositionOnScreen -= 32;
            inventory.xPositionOnScreen -= 32;
            foreach (var item in inventory.inventory) {
                item.bounds.X -= 32;
            }
            

            trashCan.bounds.X += 8;
            okButton.bounds.X += 8;


            CreativeMenu = new(ItemsToGrabMenu) {
                rows = 2
            };
            CreativeMenu.capacity = CreativeMenu.rows * 8;
            CreativeMenu.verticalGap += 40;
            CreativeMenu.horizontalGap += 20;

            // ----------------------------------------------------

            CashTab = new CashTab(() => xPositionOnScreen + width - borderWidth + 10, () => yPositionOnScreen - borderWidth + 16, 180);

            QualityButton = new QualityButton(() => xPositionOnScreen - borderWidth - 40, () => yPositionOnScreen - borderWidth / 2 - 4);
            FavoriteButton = new FavoriteButton(() => xPositionOnScreen - borderWidth - 40, () => yPositionOnScreen - borderWidth / 2 - 4 + 72);
            DisplayButton = new DisplayButton(() => xPositionOnScreen - borderWidth - 40, () => yPositionOnScreen - borderWidth / 2 - 4 + 72 * 2);
            SettingsButton = new SettingsButton(() => xPositionOnScreen - borderWidth - 40, () => yPositionOnScreen - borderWidth / 2 - 4 + 72 * 3);

            CategoryDropdown = new Dropdown<string>(() => xPositionOnScreen - borderWidth - 40, () => yPositionOnScreen - borderWidth / 2 - 4 - 64, 
                Game1.smallFont, CategoryDropdown?.Selected ?? "ALL", Enumerable.Repeat("_CATEGORY_", 20).ToArray(), p => p, maxTabWidth: 200);

            SortDropdown = new Dropdown<string>(() => xPositionOnScreen - borderWidth - 40 + 200 + 72, () => yPositionOnScreen - borderWidth / 2 - 4 - 64, 
                Game1.smallFont, SortDropdown?.Selected ?? "DEFAULT", Enumerable.Repeat("_SORT_OPTION_", 20).ToArray(), p => p, maxTabWidth: 300);

            ItemSearchBarTab = new SearchBar(() => xPositionOnScreen - borderWidth - 40 + 500 + 72 * 2, () => yPositionOnScreen - borderWidth / 2 - 4 - 64, 464);
            ItemResearchArea = new ItemResearchArea(() => xPositionOnScreen + width - borderWidth + 10, () => yPositionOnScreen - borderWidth + 88, 180);

            LeftArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 20, 
                yPositionOnScreen - borderWidth / 2 - 4 + 72 * 3 + 2 * 4, 11, 10), 
                Game1.mouseCursors, new Rectangle(353, 495, 11, 10), 4f);
            RightArrow = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - borderWidth - 120 + 12 * 4 + 8, 
                yPositionOnScreen - borderWidth / 2 - 4 + 72 * 3 + 2 * 4, 11, 10), 
                Game1.mouseCursors, new Rectangle(366, 495, 11, 10), 4f);
        }

        public override void draw(SpriteBatch b) {
            DrawMenu(b);
        }

        // --------------------------------------------------------------------------------------------------

        private void DrawMenu(SpriteBatch b) {

            if (drawBG && !Game1.options.showClearBackgrounds) {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            }

            // ----------------------------------------------------

            DrawInventoryMenu(b);
            DrawCreativeMenu(b);

            CashTab.Draw(b);
            QualityButton.Draw(b);
            FavoriteButton.Draw(b);
            DisplayButton.Draw(b);
            SettingsButton.Draw(b);
            CategoryDropdown.Draw(b);
            SortDropdown.Draw(b);
            ItemSearchBarTab.Draw(b);
            ItemResearchArea.Draw(b);

            // ----------------------------------------------------

            LeftArrow.bounds.X = xPositionOnScreen + 20;
            LeftArrow.bounds.Y = yPositionOnScreen - borderWidth / 2 - 4 + 72 * 3 + 2 * 4;
            RightArrow.bounds.X = xPositionOnScreen + width - borderWidth - 120 + 12 * 4 + 8;
            RightArrow.bounds.Y = LeftArrow.bounds.Y;

            LeftArrow.draw(b);
            RightArrow.draw(b);

            // ----------------------------------------------------

            DrawItems(b);

            // ----------------------------------------------------

            drawMouse(b);
        }

        private void DrawCreativeMenu(SpriteBatch b) {

            // ---- ItemsToGrabMenu drawing (receiving menu) --------------------------

            CreativeMenu.xPositionOnScreen = xPositionOnScreen + 36;
            CreativeMenu.yPositionOnScreen = inventory.yPositionOnScreen - inventory.height - borderWidth - spaceToClearTopBorder + 64;

            DrawCreativeMenuBackground(b);

            CreativeMenu.draw(b);
        }

        private void DrawCreativeMenuBackground(SpriteBatch b) {
            DrawHelper.drawDialogueBox(
            CreativeMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder,
            CreativeMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder + storageSpaceTopBorderOffset,
            CreativeMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2,
            CreativeMenu.height + spaceToClearTopBorder + borderWidth * 2 - storageSpaceTopBorderOffset,
            speaker: false, drawOnlyBox: true, r: 255, b: 255, g: 255);


            //Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();

            //int num = 0;
            //int num2 = 0;
            //int num3 = 0;
            //int num4 = -1;

            //width = Math.Min(titleSafeArea.Width, width);

            //Rectangle value = new Rectangle(0, 0, 64, 64);

            //int r = -1;
            //int g = -1;
            //int b = -1;

            //int x = xPositionOnScreen;
            //int y = yPositionOnScreen;

            //value.X = 64;
            //value.Y = 128;

            //Color color = r == -1 ? Color.White : new Color(-1, -1, -1);

            //var borderColor = r == -1 ? color : new Color((int)Utility.Lerp(r, Math.Min(255, r + 150), 0.65f), (int)Utility.Lerp(g, Math.Min(255, g + 150), 0.65f), (int)Utility.Lerp(b, Math.Min(255, b + 150), 0.65f));

            //Texture2D texture = r == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;

            //Game1.spriteBatch.Draw(texture, new Rectangle(x + borderWidth - 28, y - 64, width - 64, height - 128 * 3), value, borderColor);

            //value.Y = 0;
            //value.X = 0;

            //Game1.spriteBatch.Draw(texture, new Vector2(x + num, y - 64 * num4 + num2 + num3), value, color);

            //value.X = 192;

            //Game1.spriteBatch.Draw(texture, new Vector2(x + width + num - 64, y - 64 * num4 + num2 + num3), value, color);
            //value.Y = 192;

            //Game1.spriteBatch.Draw(texture, new Vector2(x + width + num - 64, y + height + num2 - 64 + num3), value, color);
            //value.X = 0;

            //Game1.spriteBatch.Draw(texture, new Vector2(x + num, y + height + num2 - 64 + num3), value, color);

            //value.X = 128;
            //value.Y = 0;
            //Game1.spriteBatch.Draw(texture, new Rectangle(64 + x + num, y - 64 * num4 + num2 + num3, width - 128, 64), value, color);

            //value.Y = 192;
            //Game1.spriteBatch.Draw(texture, new Rectangle(64 + x + num, y + height + num2 - 64 + num3, width - 128, 64), value, color);

            //value.Y = 128;
            //value.X = 0;
            //Game1.spriteBatch.Draw(texture, new Rectangle(x + num, y - 64 * num4 + num2 + 64 + num3, 64, height - 128 + num4 * 64), value, color);

            //value.X = 192;
            //Game1.spriteBatch.Draw(texture, new Rectangle(x + width + num - 64, y - 64 * num4 + num2 + 64 + num3, 64, height - 128 + num4 * 64), value, color);


            //Game1.spriteBatch.Draw(Game1.uncoloredMenuTexture, 
            //    new Vector2(CreativeMenu.xPositionOnScreen + CreativeMenu.width / 2 - 32, yPositionOnScreen + 64 + 16), 
            //    new Rectangle(128, 384, 64, 64), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            b.Draw(
                    Game1.uncoloredMenuTexture,
                    new Rectangle(CreativeMenu.xPositionOnScreen + CreativeMenu.width / 2 - 24, yPositionOnScreen - 12, 48, 256 + 8),
                    new Rectangle(128, 384, 64, 64), Color.White
                );


            //b.Draw(Game1.mouseCursors, new Vector2(CreativeMenu.xPositionOnScreen - 100, yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            //b.Draw(Game1.mouseCursors, new Vector2(CreativeMenu.xPositionOnScreen - 100, yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            //b.Draw(Game1.mouseCursors, new Vector2(CreativeMenu.xPositionOnScreen - 84, yPositionOnScreen + 64 - 44), new Rectangle(146, 447, 11, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        private void DrawInventoryMenu(SpriteBatch b) {

            // ---- MenuWithInventory drawing --------------------------

            if (trashCan != null) {
                trashCan.draw(b);
                b.Draw(Game1.mouseCursors,
                    new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40),
                    new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
                    Color.White, trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);
            }

            Game1.drawDialogueBox(xPositionOnScreen - borderWidth / 2,
                yPositionOnScreen + borderWidth + spaceToClearTopBorder + 64,
                width, height - (borderWidth + spaceToClearTopBorder + 192),
                speaker: false, drawOnlyBox: true);


            okButton?.draw(b);
            inventory.draw(b, -1, -1, -1);

            Game1.mouseCursorTransparency = 1f;

            // ---- ItemGrabMenu drawing --------------------------

            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 40, yPositionOnScreen + height / 2 + 64 - 44), new Rectangle(4, 372, 8, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
        }

        private void DrawItems(SpriteBatch b) {

            //poof?.draw(b, localPosition: true);

            foreach (TransferredItemSprite transferredItemSprite in _transferredItemSprites) {
                transferredItemSprite.Draw(b);
            }

            if (hoverText != null && (hoveredItem == null || hoveredItem == null || ItemsToGrabMenu == null)) {
                if (hoverAmount > 0) {
                    drawToolTip(b, hoverText, "", null, heldItem: true, -1, 0, null, -1, null, hoverAmount);
                }
                else {
                    drawHoverText(b, hoverText, Game1.smallFont);
                }
            }

            if (hoveredItem != null) {
                drawToolTip(b, hoveredItem.getDescription(), hoveredItem.DisplayName, hoveredItem, heldItem != null);
            }
            else if (hoveredItem != null) {
                drawToolTip(b, CreativeMenu.descriptionText, CreativeMenu.descriptionTitle, hoveredItem, heldItem != null);
            }

            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
        }
    }
}
