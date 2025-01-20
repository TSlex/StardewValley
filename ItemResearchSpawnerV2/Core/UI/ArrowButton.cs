using ItemResearchSpawnerV2.Components.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ItemResearchSpawnerV2.Core.UI {
    internal enum ArrowButtonType {
        Left,
        Right,
        Up,
        Down,
    }

    internal class ArrowButton : ButtonBase {
        public ArrowButtonType Type;
        private Rectangle SourceRect;

        public ArrowButton(Func<int> getXPos, Func<int> getYPos, ArrowButtonType type) : base(getXPos, getYPos) {
            Type = type;
            SourceRect = GetSourceRect();

            BaseWidth = SourceRect.Width - 4 * 1;
            BaseHeight = SourceRect.Height - 4 * 1;
        }

        public void Draw(SpriteBatch b, float opacity = 1f) {
            base.Draw(b);

            var effects = Type == ArrowButtonType.Up || Type == ArrowButtonType.Down ? SpriteEffects.FlipHorizontally: SpriteEffects.None;

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(Component.bounds.X, Component.bounds.Y, Component.bounds.Width, Component.bounds.Height),
                SourceRect, Color.White * opacity, 0f, Vector2.Zero, effects, 10f);
        }

        public override void Draw(SpriteBatch b) {
            Draw(b);

            //DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
            //    Component.bounds.Width - UIConstants.BorderWidth * 2,
            //    Component.bounds.Height - UIConstants.BorderWidth * 2, out var buttonInnerLocation);

            //b.Draw(Game1.mouseCursors, new Vector2(buttonInnerLocation.X, buttonInnerLocation.Y),
            //    new Rectangle(370, 377, 8, 8), Color.White, 0f, Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1f);
        }

        private Rectangle GetSourceRect() {
            return Type switch {
                ArrowButtonType.Left => UIConstants.LeftArrow,
                ArrowButtonType.Right => UIConstants.RightArrow,
                ArrowButtonType.Up => UIConstants.UpArrow,
                ArrowButtonType.Down => UIConstants.DownArrow,
                _ => throw new NotImplementedException()
            };
        }
    }
}
