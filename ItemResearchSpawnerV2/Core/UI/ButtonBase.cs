using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Components.UI {
    internal abstract class ButtonBase {

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        public readonly ClickableComponent Component;
        public Rectangle Bounds => Component.bounds;
        protected static IModContentHelper Content => ModManager.Instance.Helper.ModContent;

        public bool HoveredOver { get; protected set; } = false;
        protected float Scale => HoveredOver ? 1.1f : 1f;

        public ButtonBase(Func<int> getXPos, Func<int> getYPos) {
            GetXPos = getXPos;
            GetYPos = getYPos;

            Component = new ClickableComponent(new Rectangle(getXPos(), getYPos(), 36 + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");
        }

        public virtual void HandleLeftClick(int x, int y) { }

        public virtual void HandleRightClick(int x, int y) { }

        public virtual void HandleHover(int x, int y) { 
            HoveredOver = Bounds.Contains(x, y);
        }

        public virtual void Draw(SpriteBatch b) {
            var baseWidth = 36 + UIConstants.BorderWidth;
            var baseHeight = 36 + UIConstants.BorderWidth;

            var scaledWidth = (int)(baseWidth * Scale);
            var scaledHeight = (int)((baseHeight) * Scale);

            var offX = (baseWidth - scaledWidth) / 2;
            var offY = (baseHeight - scaledHeight) / 2;

            Component.bounds.X = GetXPos() + offX;
            Component.bounds.Y = GetYPos() + offY;

            Component.bounds.Width = scaledWidth;
            Component.bounds.Height = scaledHeight;
        }
    }
}