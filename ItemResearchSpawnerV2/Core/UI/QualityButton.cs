using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Components.UI {
    internal class QualityButton : ButtonBase {
        private readonly Texture2D _emptyQualityTexture;

        public QualityButton(Func<int> getXPos, Func<int> getYPos) : base(getXPos, getYPos) {
            _emptyQualityTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "empty-quality-icon.png"));
        }

        public override void HandleLeftClick(int x, int y) {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetNext();
            //Game1.playSound("axchop");
            Game1.playSound("money");
        }

        public override void HandleRightClick(int x, int y) {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetPrevious();
            Game1.playSound("money");
        }

        public override void Draw(SpriteBatch b) {
            base.Draw(b);
            GetCurrentQualityIcon(out var texture, out var sourceRect, out var color);

            DrawHelper.DrawMenuBox(Component.bounds.X, Component.bounds.Y,
                Component.bounds.Width - UIConstants.BorderWidth * 2,
                Component.bounds.Height - UIConstants.BorderWidth * 2, out var buttonInnerLocation);

            b.Draw(texture, new Vector2(buttonInnerLocation.X, buttonInnerLocation.Y), sourceRect, color, 0,
                Vector2.Zero, Game1.pixelZoom * Scale, SpriteEffects.None, 1f);
        }

        private void GetCurrentQualityIcon(out Texture2D texture, out Rectangle sourceRect, out Color color) {
            texture = Game1.mouseCursors;
            color = Color.White;

            switch (ModManager.Instance.ItemQuality) {
                case ItemQuality.Normal:
                    texture = _emptyQualityTexture;
                    sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                    color *= 0.65f;
                    break;

                case ItemQuality.Silver:
                    sourceRect = new Rectangle(338, 400, 8, 8);
                    break;

                case ItemQuality.Gold:
                    sourceRect = new Rectangle(346, 400, 8, 8);
                    break;

                default:
                    sourceRect = new Rectangle(346, 392, 8, 8);
                    break;
            }
        }
    }
}