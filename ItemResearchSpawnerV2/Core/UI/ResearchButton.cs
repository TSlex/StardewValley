using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class ResearchButton : ButtonBase {

        private readonly Texture2D ResearchTexture;
        private readonly Texture2D SellTexture;
        private readonly Texture2D CombinedTexture;

        private readonly ItemResearchArea researchArea;

        public ResearchButton(ItemResearchArea researchArea, Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {

            ResearchTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "search-button"));
            SellTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "sell-button.png"));
            CombinedTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "combined-button.png"));
            this.researchArea = researchArea;
        }

        public void Draw(SpriteBatch b, bool shake = false) {
            base.Draw(b);

            var buttonTexture = ModManager.Instance.ModMode switch {
                ModMode.BuySell => SellTexture,
                ModMode.Combined => CombinedTexture,
                _ => ResearchTexture
            };

            //DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
            //    Component.bounds.Width - UIConstants.BorderWidth,
            //    Component.bounds.Height - UIConstants.BorderWidth, out var buttonInnerLocation);

            var buttonBounds = new Rectangle(Component.bounds.X, Component.bounds.Y,
                Component.bounds.Width,
                Component.bounds.Height);

            if (shake) {
                var buttonOff = 1f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
                buttonBounds.X += (int)buttonOff.X;
                buttonBounds.Y += (int)buttonOff.Y;
            }

            b.Draw(buttonTexture, buttonBounds, buttonTexture.Bounds, Color.White);
        }

        public override void HandleLeftClick(int x, int y) {
            researchArea.HandleResearch();
        }
    }
}
