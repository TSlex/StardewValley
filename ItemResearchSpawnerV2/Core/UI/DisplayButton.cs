using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Data.Enums;
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
                Component.bounds.Width - UIConstants.BorderWidth * 2,
                Component.bounds.Height - UIConstants.BorderWidth * 2, out var buttonInnerLocation);

            switch (ModManager.Instance.ProgressionDisplay) {
                case Data.Enums.ProgressionDisplayMode.ResearchedOnly:

                    b.Draw(Game1.mouseCursors, new Rectangle((int)buttonInnerLocation.X, (int)buttonInnerLocation.Y,
                        (int)(32 * Scale), (int)(32 * Scale)),
                        new Rectangle(208, 321, 14, 15), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    break;

                case Data.Enums.ProgressionDisplayMode.ResearchStarted:

                    b.Draw(Game1.mouseCursors, new Rectangle((int)buttonInnerLocation.X, (int)buttonInnerLocation.Y,
                        (int)(32 * Scale), (int)(32 * Scale)),
                        new Rectangle(208, 321, 14, 15), Color.Blue, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    break;

                case Data.Enums.ProgressionDisplayMode.Combined:

                    b.Draw(Game1.mouseCursors, new Rectangle((int)buttonInnerLocation.X, (int)buttonInnerLocation.Y,
                        (int)(32 * Scale), (int)(32 * Scale)),
                        new Rectangle(208, 321, 14, 15), Color.Red, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                    break;
            }
        }

        public override void HandleLeftClick(int x, int y) {
            ModManager.Instance.ProgressionDisplay = ModManager.Instance.ProgressionDisplay.GetNext();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }

        public override void HandleRightClick(int x, int y) {
            ModManager.Instance.ProgressionDisplay = ModManager.Instance.ProgressionDisplay.GetPrevious();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }
    }
}
