using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class QualityBookmarkButton : BookmarkButtonBase {
        public QualityBookmarkButton(Func<int> getXPos, Func<int> getYPos) : 
            base(getXPos, getYPos, UIConstants.QualityButton.Width - 1 * 4, UIConstants.QualityButton.Height - 2 * 4) {
        }

        public override void HandleLeftClick(int x, int y) {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetNext();
            //Game1.playSound("axchop");
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("money");
            }
        }

        public override void HandleRightClick(int x, int y) {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetPrevious();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("money");
            }
        }

        public override void Draw(SpriteBatch b) {
            Inactive = ModManager.Instance.ItemQuality == ItemQuality.Normal;

            base.Draw(b);

            var baseRect = new Rectangle(
                UIConstants.QualityButton.X, 
                UIConstants.QualityButton.Y, 
                UIConstants.QualityButton.Width + XOffcet, 
                UIConstants.QualityButton.Height);

            b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X - 4 * 1 - XOffcet, Component.bounds.Y + 4 * -1),
                baseRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

            if (ModManager.Instance.ItemQuality != ItemQuality.Normal) {
                var iconRect = GetCurrentQualityIcon();

                b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 6 - XOffcet, Component.bounds.Y + 4 * 2),
                    iconRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }
        }

        private Rectangle GetCurrentQualityIcon() {
            switch (ModManager.Instance.ItemQuality) {
                case ItemQuality.Silver:
                    return UIConstants.QualitySilverIcon;

                case ItemQuality.Gold:
                    return UIConstants.QualityGoldIcon;

                case ItemQuality.Iridium:
                    return UIConstants.QualityIridiumIcon;

                default:
                    return new Rectangle();
            }
        }
    }
}
