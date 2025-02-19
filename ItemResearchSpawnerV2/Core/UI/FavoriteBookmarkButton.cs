using ItemResearchSpawnerV2.Core.Data.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class FavoriteBookmarkButton : BookmarkButtonBase {
        public FavoriteBookmarkButton(Func<int> getXPos, Func<int> getYPos) : 
            base(getXPos, getYPos, UIConstants.FavoriteButton.Width, UIConstants.FavoriteButton.Height - 2 * 4) {
            //InactiveXOffcet -= 4 * 2;
        }

        public override void HandleLeftClick(int x, int y) {
            ModManager.Instance.FavoriteDisplay = ModManager.Instance.FavoriteDisplay.GetNext();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }

        public override void HandleRightClick(int x, int y) {
            ModManager.Instance.FavoriteDisplay = ModManager.Instance.FavoriteDisplay.GetPrevious();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }

        public override void Draw(SpriteBatch b) {
            Inactive = ModManager.Instance.FavoriteDisplay == FavoriteDisplayMode.All;

            base.Draw(b);

            var baseRect = new Rectangle(
                UIConstants.FavoriteButton.X, 
                UIConstants.FavoriteButton.Y, 
                UIConstants.FavoriteButton.Width + XOffcet, 
                UIConstants.FavoriteButton.Height);

            b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X - XOffcet, Component.bounds.Y + 4 * -1),
                baseRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

            if (ModManager.Instance.FavoriteDisplay == FavoriteDisplayMode.FavoriteOnly) {
                b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 6 - XOffcet, Component.bounds.Y + 4 * 3),
                    UIConstants.FavoriteButtonIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }
        }
    }
}
