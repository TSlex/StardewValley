using System.IO;
using ItemResearchSpawnerV2.Core;
using ItemResearchSpawnerV2.Core.UI;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Components.Utils {
    internal class ButtonBase {
        private readonly Texture2D _emptyQualityTexture;

        private readonly ClickableComponent component;

        private static IModContentHelper Content => ModManager.Instance.helper.ModContent;

        public ButtonBase(int x, int y) {
            _emptyQualityTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "empty-quality-icon.png"));

            component = new ClickableComponent(new Rectangle(x, y, 36 + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");
        }

        public Rectangle Bounds => component.bounds;

        public void HandleLeftClick() {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetNext();
        }

        public void HandleRightClick() {
            ModManager.Instance.ItemQuality = ModManager.Instance.ItemQuality.GetPrevious();
        }

        public void Draw(SpriteBatch spriteBatch) {
            GetCurrentQualityIcon(out var texture, out var sourceRect, out var color);

            DrawHelper.DrawMenuBox(component.bounds.X, component.bounds.Y,
                component.bounds.Width - UIConstants.BorderWidth,
                component.bounds.Height - UIConstants.BorderWidth, out var qualityIconPos);

            spriteBatch.Draw(texture, new Vector2(qualityIconPos.X, qualityIconPos.Y), sourceRect, color, 0,
                Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
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
