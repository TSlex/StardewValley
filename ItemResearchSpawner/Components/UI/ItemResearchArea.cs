﻿using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    internal class ItemResearchArea
    {
        private readonly ClickableComponent _researchArea;
        private readonly ClickableTextureComponent _researchButton;
        private readonly Texture2D _researchTexture;

        private Item _researchItem;

        public ItemResearchArea(IContentHelper content, IMonitor monitor, int x, int y)
        {
            _researchTexture = content.Load<Texture2D>("assets/search-button.png");

            _researchArea = new ClickableComponent(new Rectangle(x, y, Game1.tileSize + 60, Game1.tileSize + 50), "");

            _researchButton = new ClickableTextureComponent(
                new Rectangle(
                    RenderHelpers.GetChildCenterPosition(x, _researchArea.bounds.Width + 2 * UIConstants.BorderWidth,
                        _researchTexture.Width),
                    _researchArea.bounds.Height + 48 + y, _researchTexture.Width,
                    _researchTexture.Height), _researchTexture,
                new Rectangle(0, 0, _researchTexture.Width, _researchTexture.Height), 1f);

            ProgressionManager.OnStackChanged += OnStackChanged;
        }

        public Rectangle Bounds => _researchArea.bounds;

        public Rectangle ButtonBounds => _researchButton.bounds;

        public Item ResearchItem => _researchItem;

        public bool TrySetItem(Item item)
        {
            if (_researchItem != null) return false;

            _researchItem = item;

            return true;
        }

        public Item ReturnItem()
        {
            var item = _researchItem;
            _researchItem = null;

            return item;
        }

        public void HandleResearch()
        {
            if (_researchItem != null)
            {
                ProgressionManager.Instance.ResearchItem(_researchItem);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            RenderHelpers.DrawMenuBox(_researchArea.bounds.X, _researchArea.bounds.Y,
                _researchArea.bounds.Width, _researchArea.bounds.Height, out var areaInnerAnchors);

            var researchItemCellX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - Game1.tileSize / 2f;
            RenderHelpers.DrawItemBox((int) researchItemCellX, (int) areaInnerAnchors.Y + 10, Game1.tileSize,
                Game1.tileSize,
                out _);

            var researchProgressString = _researchItem != null
                ? ProgressionManager.Instance.GetItemProgression(_researchItem)
                : "(0 / 0)";

            var progressFont = Game1.dialogueFont;
            var progressPositionX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f -
                                    progressFont.MeasureString(researchProgressString).X / 2f;

            spriteBatch.DrawString(progressFont, researchProgressString,
                new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);

            spriteBatch.Draw(_researchButton.texture, _researchButton.bounds, _researchButton.sourceRect, Color.White);

            _researchItem?.drawInMenu(spriteBatch, new Vector2(researchItemCellX, areaInnerAnchors.Y + 10), 1f);
        }

        private void OnStackChanged(int newCount)
        {
            if (newCount <= 0)
            {
                _researchItem = null;
            }
            else if (_researchItem != null)
            {
                _researchItem.Stack = newCount % _researchItem.maximumStackSize();
            }
        }
    }
}