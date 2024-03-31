using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class DisplayButton : ButtonBase {
        public DisplayButton(Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {
        }

        public override void Draw(SpriteBatch b) {
            base.Draw(b);
            DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
                Component.bounds.Width - UIConstants.BorderWidth,
                Component.bounds.Height - UIConstants.BorderWidth, out var buttonInnerLocation);

            b.Draw(Game1.mouseCursors, new Rectangle((int)buttonInnerLocation.X, (int)buttonInnerLocation.Y, 
                (int)(32 * Scale), (int)(32 * Scale)),
                new Rectangle(208, 321, 14, 15), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
        }
    }
}
