using ItemResearchSpawnerV2.Components.UI;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class RNSButton : ButtonBase {

        public RNSButton(Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {
        }

        public void Draw(SpriteBatch b, bool shake = false) {
            base.Draw(b);

            b.Draw(ModManager.UITextureInstance, Component.bounds, UIConstants.BookBase, Color.White);
            b.Draw(ModManager.UITextureInstance, Component.bounds, UIConstants.BookFrame, ModManager.Instance.ModMode.GetColor());
        }

        public override void HandleLeftClick(int x, int y) {
            if (Component.bounds.Contains(x, y)) {
                ModManager.Instance.Helper.Input.Suppress(SButton.MouseLeft);
                ModManager.Instance.Helper.Input.Suppress(SButton.ControllerA);
                Game1.activeClickableMenu?.exitThisMenuNoSound();
                ModManager.Instance.OpenMenu();

                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("bigSelect");
                }
            }
        }
    }
}
