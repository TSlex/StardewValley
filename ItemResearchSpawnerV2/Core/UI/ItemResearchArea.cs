using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class ItemResearchArea {
        private readonly ClickableComponent ResearchArea;
        private readonly ClickableTextureComponent ResearchButton;

        private readonly Texture2D ResearchTexture;
        private readonly Texture2D SellTexture;
        private readonly Texture2D CombinedTexture;

        private Item ResearchItem;
        private Item LastItem;
        private string ItemProgression;

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;
        private readonly int Width;

        private static IModContentHelper Content => ModManager.Instance.helper.ModContent;

        public ItemResearchArea(Func<int> getXPos, Func<int> getYPos, int width) {

            GetXPos = getXPos;
            GetYPos = getYPos;
            Width = width;

            ResearchTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "search-button"));
            SellTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "sell-button.png"));
            CombinedTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "combined-button.png"));

            ResearchArea = new ClickableComponent(new Rectangle(getXPos(), getYPos(), width, Game1.tileSize + 50), "");

            ResearchButton = new ClickableTextureComponent(
                new Rectangle(
                    DrawHelper.GetChildCenterPosition(getXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
                    ResearchArea.bounds.Height + 38 + getYPos(), ResearchTexture.Width, ResearchTexture.Height),
                ResearchTexture, new Rectangle(0, 0, ResearchTexture.Width, ResearchTexture.Height), 1f);

            //ProgressionManager.OnStackChanged += OnStackChanged;
        }
        public void Draw(SpriteBatch spriteBatch) {

            ResearchArea.bounds.X = GetXPos();
            ResearchArea.bounds.Y = GetYPos();

            var buttonNewLocation = new Rectangle(
                    DrawHelper.GetChildCenterPosition(GetXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
                    ResearchArea.bounds.Height + 38 + GetYPos(), ResearchTexture.Width, ResearchTexture.Height);

            ResearchButton.bounds.X = buttonNewLocation.X;
            ResearchButton.bounds.Y = buttonNewLocation.Y;

            // ------------------------------------------------------------------------------------------------------

            DrawHelper.DrawMenuBox(ResearchArea.bounds.X, ResearchArea.bounds.Y,
                ResearchArea.bounds.Width, ResearchArea.bounds.Height, out var areaInnerAnchors);

            var researchItemCellX = areaInnerAnchors.X + ResearchArea.bounds.Width / 2f - Game1.tileSize / 2f;

            DrawHelper.DrawItemBox((int)researchItemCellX, (int)areaInnerAnchors.Y + 10, Game1.tileSize,
                Game1.tileSize,
                out _);

            var researchProgressString = GetItemProgression();

            var progressFont = Game1.smallFont;
            var progressPositionX = areaInnerAnchors.X + ResearchArea.bounds.Width / 2f -
                                    progressFont.MeasureString(researchProgressString).X / 2f;

            spriteBatch.DrawString(progressFont, researchProgressString,
                new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);


            var buttonTexture = ModManager.Instance.ModMode switch {
                ModMode.BuySell => SellTexture,
                ModMode.Combined => CombinedTexture,
                _ => ResearchTexture
            };

            spriteBatch.Draw(buttonTexture, ResearchButton.bounds, ResearchButton.sourceRect, Color.White);
            ResearchItem?.drawInMenu(spriteBatch, new Vector2(researchItemCellX, areaInnerAnchors.Y + 10), 1f);
        }

        //public Rectangle Bounds => _researchArea.bounds;

        //public Rectangle ButtonBounds => _researchButton.bounds;

        //public Item ResearchItem => _researchItem;

        //public bool TrySetItem(Item item)
        //{
        //    if (_researchItem != null) return false;

        //    _researchItem = item;

        //    return true;
        //}

        //public Item ReturnItem()
        //{
        //    var item = _researchItem;
        //    _researchItem = null;
        //    _lastItem = null;

        //    return item;
        //}

        //public void HandleResearch()
        //{
        //    if (_researchItem != null)
        //    {
        //        if (ModManager.Instance.ModMode == ModMode.Combined)
        //        {
        //            ModManager.Instance.SellItem(_researchItem);
        //        }

        //        if (ModManager.Instance.ModMode == ModMode.BuySell)
        //        {
        //            ModManager.Instance.SellItem(_researchItem);
        //        }

        //        ProgressionManager.Instance.ResearchItem(_researchItem);
        //    }
        //}



        //public void PrepareToBeKilled()
        //{
        //    ProgressionManager.OnStackChanged -= OnStackChanged;
        //}

        private string GetItemProgression() {
            return "(0 / 0)";

            //if (_researchItem == null)
            //{
            //    return "(0 / 0)";
            //}

            //if (_lastItem == null || !_lastItem.Equals(_researchItem))
            //{
            //    _itemProgression = ProgressionManager.Instance.GetItemProgression(_researchItem, true);
            //    _lastItem = _researchItem;
            //}

            //return _itemProgression;
        }

        private void OnStackChanged(int newCount) {
            LastItem = null;

            if (newCount <= 0) {
                ResearchItem = null;
            }
            else if (ResearchItem != null) {
                ResearchItem.Stack = newCount % ResearchItem.maximumStackSize();
            }
        }
    }
}