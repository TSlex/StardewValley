using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSkipper.Core.UI {
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

            var effects = Type == ArrowButtonType.Up || Type == ArrowButtonType.Down ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(Component.bounds.X, Component.bounds.Y, Component.bounds.Width, Component.bounds.Height),
                SourceRect, Color.White * opacity, 0f, Vector2.Zero, effects, 10f);
        }

        public override void Draw(SpriteBatch b) {
            Draw(b);
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
