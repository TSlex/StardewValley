using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Components.UI {
    internal abstract class ButtonBase {

        private readonly Func<int> getXPos;
        private readonly Func<int> getYPos;

        public readonly ClickableComponent Component;
        public Rectangle Bounds => Component.bounds;
        protected static IModContentHelper Content => ModManager.Instance.helper.ModContent;

        public ButtonBase(Func<int> getXPos, Func<int> getYPos) {
            this.getXPos = getXPos;
            this.getYPos = getYPos;

            Component = new ClickableComponent(new Rectangle(getXPos(), getYPos(), 36 + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");
        }

        public abstract void HandleLeftClick();

        public abstract void HandleRightClick();

        public virtual void Draw(SpriteBatch b) {
            Component.bounds.X = getXPos();
            Component.bounds.Y = getYPos();
        }
    }
}