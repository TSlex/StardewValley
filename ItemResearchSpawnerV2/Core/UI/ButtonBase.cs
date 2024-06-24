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

        public int BaseWidth;
        public int BaseHeight;
        public int BaseXOff;
        public int BaseYOff;

        public ClickableComponent Component;
        public Rectangle Bounds => Component.bounds;
        protected static IModContentHelper Content => ModManager.Instance.Helper.ModContent;

        public bool HoveredOver { get; protected set; } = false;
        protected float Scale => HoveredOver ? 1.1f : 1f;

        public ButtonBase(Func<int> getXPos, Func<int> getYPos, 
            int baseWidth = 36 + UIConstants.BorderWidth * 2, int baseHeight = 36 + UIConstants.BorderWidth * 2) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            BaseWidth = baseWidth;
            BaseHeight = baseHeight;

            Component = new ClickableComponent(new Rectangle(getXPos(), getYPos(), BaseWidth, BaseHeight), "");
        }

        public virtual void HandleLeftClick(int x, int y) { }

        public virtual void HandleRightClick(int x, int y) { }

        public virtual void HandleHover(int x, int y) { 
            HoveredOver = Bounds.Contains(x, y);
        }

        public virtual void Draw(SpriteBatch b) {
            var scaledWidth = (int)(BaseWidth * Scale);
            var scaledHeight = (int)((BaseHeight) * Scale);

            var offX = (BaseWidth - scaledWidth) / 2;
            var offY = (BaseHeight - scaledHeight) / 2;

            Component.bounds.X = GetXPos() + offX + BaseXOff;
            Component.bounds.Y = GetYPos() + offY + BaseYOff;

            Component.bounds.Width = scaledWidth;
            Component.bounds.Height = scaledHeight;
        }
    }
}