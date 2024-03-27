using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Reflection;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class MainMenu : ItemGrabMenu {

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

        }

        public override void draw(SpriteBatch b) {
            DrawMenu(b);
        }

        // --------------------------------------------------------------------------------------------------

        private void DrawMenu(SpriteBatch b) {

            if (drawBG && !Game1.options.showClearBackgrounds) {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            }


            // ---- MenuWithInventory drawing --------------------------

            if (trashCan != null) {
                trashCan.draw(b);
                b.Draw(Game1.mouseCursors,
                    new Vector2(trashCan.bounds.X + 60, trashCan.bounds.Y + 40),
                    new Rectangle(564 + Game1.player.trashCanLevel * 18, 129, 18, 10),
                    Color.White, trashCanLidRotation, new Vector2(16f, 10f), 4f, SpriteEffects.None, 0.86f);


            }

            Game1.drawDialogueBox(xPositionOnScreen - borderWidth / 2, yPositionOnScreen + borderWidth + spaceToClearTopBorder + 64, width, height - (borderWidth + spaceToClearTopBorder + 192), speaker: false, drawOnlyBox: true);


            okButton?.draw(b);
            inventory.draw(b, -1, -1, -1);

            Game1.mouseCursorTransparency = 1f;


            // ---- ItemsToGrabMenu drawing (receiving menu) --------------------------

            ItemsToGrabMenu.xPositionOnScreen = xPositionOnScreen + 36;

            DrawHelper.drawDialogueBox(
                ItemsToGrabMenu.xPositionOnScreen - borderWidth - spaceToClearSideBorder,
                ItemsToGrabMenu.yPositionOnScreen - borderWidth - spaceToClearTopBorder + storageSpaceTopBorderOffset,
                ItemsToGrabMenu.width + borderWidth * 2 + spaceToClearSideBorder * 2,
                ItemsToGrabMenu.height + spaceToClearTopBorder + borderWidth * 2 - storageSpaceTopBorderOffset,
                speaker: false, drawOnlyBox: true, r: 255, b: 255, g: 255);

            b.Draw(Game1.mouseCursors, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 100, yPositionOnScreen + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 100, yPositionOnScreen + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(ItemsToGrabMenu.xPositionOnScreen - 84, yPositionOnScreen + 64 - 44), new Rectangle(146, 447, 11, 10), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            ItemsToGrabMenu.draw(b);


            // ---- ItemGrabMenu drawing --------------------------

            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 + 16), new Rectangle(16, 368, 12, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 64, yPositionOnScreen + height / 2 + 64 - 16), new Rectangle(21, 368, 11, 16), Color.White, 4.712389f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            b.Draw(Game1.mouseCursors, new Vector2(xPositionOnScreen - 40, yPositionOnScreen + height / 2 + 64 - 44), new Rectangle(4, 372, 8, 11), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

            poof?.draw(b, localPosition: true);
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
            else if (hoveredItem != null && ItemsToGrabMenu != null) {
                drawToolTip(b, ItemsToGrabMenu.descriptionText, ItemsToGrabMenu.descriptionTitle, hoveredItem, heldItem != null);
            }

            heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);


            // ----------------------------------------------------


            drawMouse(b);
        }
    }
}
