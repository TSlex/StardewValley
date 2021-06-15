using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    /**
        MIT License

        Copyright (c) 2018 CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
     **/
    internal class ItemQualitySelectorTab
    {
        private readonly Texture2D _emptyQualityTexture;

        private readonly ClickableComponent _qualityButton;

        private ItemQuality _quality;

        public delegate void QualityChange(ItemQuality newQuality);

        public event QualityChange OnQualityChange;

        public ItemQualitySelectorTab(IContentHelper content, IMonitor monitor, int x, int y, ItemQuality initQuality)
        {
            _emptyQualityTexture = content.Load<Texture2D>("assets/empty-quality-icon.png");
            _quality = initQuality;

            _qualityButton =
                new ClickableComponent(
                    new Rectangle(x, y, 36 + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");
        }

        public Rectangle Bounds => _qualityButton.bounds;

        public void HandleLeftClick()
        {
            _quality = _quality.GetNext();
            OnQualityChange?.Invoke(_quality);
        }
        
        public void HandleRightClick()
        {
            _quality = _quality.GetPrevious();
            OnQualityChange?.Invoke(_quality);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            GetCurrentQualityIcon(out var texture, out var sourceRect, out var color);

            RenderHelpers.DrawMenuBox(_qualityButton.bounds.X, _qualityButton.bounds.Y,
                _qualityButton.bounds.Width - UIConstants.BorderWidth,
                _qualityButton.bounds.Height - UIConstants.BorderWidth, out var qualityIconPos);

            spriteBatch.Draw(texture, new Vector2(qualityIconPos.X, qualityIconPos.Y), sourceRect, color, 0,
                Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);
        }

        private void GetCurrentQualityIcon(out Texture2D texture, out Rectangle sourceRect, out Color color)
        {
            texture = Game1.mouseCursors;
            color = Color.White;

            switch (_quality)
            {
                case ItemQuality.Normal:
                    texture = _emptyQualityTexture;
                    sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                    color *= 0.65f;
                    break;

                case ItemQuality.Silver:
                    sourceRect = CursorSprites.SilverStarQuality;
                    break;

                case ItemQuality.Gold:
                    sourceRect = CursorSprites.GoldStarQuality;
                    break;

                default:
                    sourceRect = CursorSprites.IridiumStarQuality;
                    break;
            }
        }
    }
}