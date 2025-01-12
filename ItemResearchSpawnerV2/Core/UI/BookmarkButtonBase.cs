using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemResearchSpawnerV2.Core.UI {
    internal abstract class BookmarkButtonBase {

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        public ClickableComponent Component;
        public Rectangle Bounds => Component.bounds;
        protected static IModContentHelper Content => ModManager.Instance.Helper.ModContent;

        public bool HoveredOver { get; protected set; } = false;
        public bool Inactive { get; protected set; } = true;

        protected int InactiveXOffcet = 4 * 6;
        protected int HoverXOffcet = 4 * 1;

        protected int XOffcet => -HoverXOffcet + (HoveredOver ? HoverXOffcet : 0) - (Inactive ? InactiveXOffcet : 0);
        //protected int XOffcet => HoverXOffcet + (HoveredOver ? HoverXOffcet : 0) + (Inactive ? InactiveXOffcet : 0);
        //protected int WidthOffcet => HoverXOffcet - XOffcet;

        public BookmarkButtonBase(Func<int> getXPos, Func<int> getYPos,
            int baseWidth = 36 + UIConstants.BorderWidth * 2, int baseHeight = 36 + UIConstants.BorderWidth * 2) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            Component = new ClickableComponent(new Rectangle(getXPos(), getYPos(), baseWidth, baseHeight), "");
        }

        public virtual void HandleLeftClick(int x, int y) { }

        public virtual void HandleRightClick(int x, int y) { }

        public virtual void HandleHover(int x, int y) {
            HoveredOver = Bounds.Contains(x, y);
        }

        public virtual void Draw(SpriteBatch b) {
            Component.bounds.X = GetXPos();
            Component.bounds.Y = GetYPos();
        }
    }
}