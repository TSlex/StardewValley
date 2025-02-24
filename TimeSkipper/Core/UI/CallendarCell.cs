using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeSkipper.Core.UI {
    internal class CallendarCell : ClickableComponent {
        public bool HoveredOver { get; protected set; } = false;

        public CallendarCell(Rectangle bounds, string name) : base(bounds, name) {
        }

        public virtual void HandleHover(int x, int y) {
            HoveredOver = bounds.Contains(x, y);

            //if (HoveredOver) {
            //    ModManager.Instance.Monitor.Log($"Hovered over {name} --- (X: {x}, Y: {y})");
            //}
        }
    }
}
