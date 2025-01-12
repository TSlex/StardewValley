using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class FavoriteButton : ButtonBase {
        public FavoriteButton(Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {
        }

        public override void Draw(SpriteBatch b) {
            base.Draw(b);
            DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
                Component.bounds.Width - UIConstants.BorderWidth * 2,
                Component.bounds.Height - UIConstants.BorderWidth * 2, out var buttonInnerLocation);

            switch (ModManager.Instance.FavoriteDisplay) {
                case Data.Enums.FavoriteDisplayMode.All:
                    b.Draw(Game1.mouseCursors, new Vector2(buttonInnerLocation.X, buttonInnerLocation.Y),
                        new Rectangle(626, 1892, 9, 9), Color.Blue, 0f, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1f);
                    break;
                case Data.Enums.FavoriteDisplayMode.FavoriteOnly:
                    b.Draw(Game1.mouseCursors, new Vector2(buttonInnerLocation.X, buttonInnerLocation.Y),
                        new Rectangle(626, 1892, 9, 9), Color.White, 0f, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1f);
                    break;
            }
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
    }
}
