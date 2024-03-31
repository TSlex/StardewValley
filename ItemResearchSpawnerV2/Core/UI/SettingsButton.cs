using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class SettingsButton : ButtonBase {
        public SettingsButton(Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {
        }

        public override void Draw(SpriteBatch b) {
            base.Draw(b);
            DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
                Component.bounds.Width - UIConstants.BorderWidth,
                Component.bounds.Height - UIConstants.BorderWidth, out var buttonInnerLocation);

            b.Draw(Game1.mouseCursors, new Vector2(buttonInnerLocation.X, buttonInnerLocation.Y),
                new Rectangle(370, 377, 8, 8), Color.White, 0f, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1f);
        }
    }
}
