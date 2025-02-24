using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using TimeSkipper.Core.Utils;

namespace TimeSkipper.Core.UI {
    internal class SleepButton : ButtonBase {

        protected override float Scale => HoveredOver ? 1.05f : 1f;

        public SleepButton(Func<int> getXPos, Func<int> getYPos, int baseWidth = 68, int baseHeight = 68) : 
            base(getXPos, getYPos, baseWidth, baseHeight) {
        }

        public void Draw(SpriteBatch b, float opacity = 1f) {
            base.Draw(b);

            var c = HoveredOver ? Color.Gold : Color.White;

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.SleepButton,
                new Rectangle(Component.bounds.X, Component.bounds.Y, Component.bounds.Width, Component.bounds.Height), cornerSize: 4 * 3, color: c);

            var buttonText = I18n.Menu_SleepButton();
            var buttonTextWidth = Game1.smallFont.MeasureString(buttonText);
            var buttonTextLocation = new Vector2(Component.bounds.X + Component.bounds.Width / 2, Component.bounds.Y + Component.bounds.Height / 2);

            Utility.drawTextWithShadow(b,
                buttonText,
                Game1.smallFont,
                buttonTextLocation + new Vector2(-buttonTextWidth.X / 2, -buttonTextWidth.Y / 2),
                Color.Black);
        }

        public override void Draw(SpriteBatch b) {
            Draw(b);
        }
    }
}
