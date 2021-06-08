using System;
using System.Collections.Generic;
using System.Linq;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Constants = StardewModdingAPI.Constants;

namespace ItemResearchSpawner.Components
{
    internal class SpawnMenu : ItemGrabMenu
    {
        private readonly IMonitor _monitor;
        private readonly Action<SpriteBatch> _baseDraw;
        private readonly IContentHelper _content;

        // private readonly Texture2D _researchTexture;
        private readonly Texture2D _sortTexture;
        private readonly Texture2D _emptyQualityTexture;

        // private ClickableComponent _researchArea;
        // private ClickableTextureComponent _researchButton;
        private ClickableComponent _qualityButton;
        private ClickableComponent _sortButton;
        private ClickableTextureComponent _sortIcon;
        private string _sortLabelIndent;

        private Dropdown<string> _categoryDropdown;

        private TextBox _searchBox;
        private Rectangle _searchBoxBounds;
        private ClickableComponent _searchBoxArea;
        private ClickableTextureComponent _searchIcon;

        private readonly string[] _availableCategories;

        private string _searchText;
        private ItemQuality _quality;
        private ItemSortOption _sortBy;

        private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        private ItemResearchArea _researchArea;
        private ItemQualitySelectorTab _qualitySelector;
        private ItemSortTab _itemSortTab;
        private ItemCategorySelectorTab _categorySelector;
        private ItemSearchBarTab _searchBarTab;

        public SpawnMenu(SpawnableItem[] spawnableItems, IContentHelper content, IMonitor monitor) : base(
            inventory: new List<Item>(),
            reverseGrab: false,
            showReceivingMenu: true,
            highlightFunction: item => true,
            behaviorOnItemGrab: (item, player) => { },
            behaviorOnItemSelectFunction: (item, player) => { },
            message: null,
            canBeExitedWithKey: true,
            showOrganizeButton: false,
            source: IsAndroid ? source_chest : source_none
        )
        {
            _monitor = monitor;
            _content = content;
            _baseDraw = RenderHelper.GetBaseDraw(this);
            _availableCategories = GetDisplayCategories(spawnableItems).ToArray();
            
            // _researchTexture = content.Load<Texture2D>("assets/search-button.png");
            _sortTexture = content.Load<Texture2D>("assets/sort-icon.png");
            _sortLabelIndent = GetSpaceIndent(Game1.smallFont, _sortTexture.Width) + " ";
            _emptyQualityTexture = content.Load<Texture2D>("assets/empty-quality-icon.png");

            _quality = ItemQuality.Iridium;
            _searchText = "Pumpkin";
            _sortBy = ItemSortOption.Category;

            InitializeComponents();
        }

        private IEnumerable<string> GetDisplayCategories(SpawnableItem[] items)
        {
            var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in items)
            {
                if (item.Category.ToLower().Equals("all") || item.Category.ToLower().Equals("misc"))
                {
                    continue;
                }

                categories.Add(item.Category);
            }

            yield return "all";

            foreach (var category in categories.OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                yield return category;
            }

            yield return "misc";
        }

        private string GetSpaceIndent(SpriteFont font, int width)
        {
            if (width <= 0)
                return "";

            var indent = " ";

            while (font.MeasureString(indent).X < width)
                indent += " ";

            return indent;
        }

        private void InitializeComponents()
        {
            var rootLeftAnchor = xPositionOnScreen;
            var rootTopAnchor = yPositionOnScreen;
            var rootRightAnchor = rootLeftAnchor + width;
            var rootBottomAnchor = rootTopAnchor + height;

            var sideTopAnchor = rootTopAnchor;
            var sideRightAnchor = rootRightAnchor;

            var barTopAnchor = rootTopAnchor - Game1.tileSize * 2;

            _researchArea = new ItemResearchArea(_content, sideRightAnchor, sideTopAnchor);

            // _researchArea =
            //     new ClickableComponent(
            //         new Rectangle(sideRightAnchor, sideTopAnchor, Game1.tileSize + 60, Game1.tileSize + 50), "");
            //
            // _researchButton = new ClickableTextureComponent(
            //     new Rectangle(
            //         (int) (sideRightAnchor + (_researchArea.bounds.Width + 32) / 2f - _researchTexture.Width / 2f),
            //         _researchArea.bounds.Height + 48 + sideTopAnchor, _researchTexture.Width,
            //         _researchTexture.Height), _researchTexture,
            //     new Rectangle(0, 0, _researchTexture.Width, _researchTexture.Height), 1f);

            _qualityButton =
                new ClickableComponent(
                    new Rectangle(rootLeftAnchor - 8, barTopAnchor, 36 + UIConstants.BorderWidth,
                        36 + UIConstants.BorderWidth - 2), "");

            _sortButton =
                new ClickableComponent(
                    new Rectangle(_qualityButton.bounds.Right + 20, barTopAnchor,
                        GetMaxSortLabelWidth(Game1.smallFont) + UIConstants.BorderWidth, Game1.tileSize),
                    GetSortLabel(_sortBy));

            _sortIcon = new ClickableTextureComponent(
                new Rectangle(_sortButton.bounds.X + UIConstants.BorderWidth,
                    barTopAnchor + UIConstants.BorderWidth, _sortTexture.Width, Game1.tileSize), _sortTexture,
                new Rectangle(0, 0, _sortTexture.Width, _sortTexture.Height), 1f);

            _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor)
            {
                X = rootRightAnchor - 52,
                Y = barTopAnchor,
                Height = 0,
                Width = 200,
                Text = _searchText
            };

            _searchBoxBounds = new Rectangle(_searchBox.X, _searchBox.Y + 4, _searchBox.Width, 12 * Game1.pixelZoom);

            _searchBoxArea =
                new ClickableComponent(
                    new Rectangle(_searchBoxBounds.X, _searchBoxBounds.Y, _searchBoxBounds.Width,
                        _searchBoxBounds.Height), "");

            var iconRect = new Rectangle(80, 0, 13, 13);
            const float iconScale = 2.5f;

            var iconBounds = new Rectangle((int) (_searchBoxBounds.Right - iconRect.Width * iconScale),
                (int) (_searchBoxBounds.Center.Y - iconRect.Height / 2f * iconScale),
                (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            );

            _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);

            _categoryDropdown = new Dropdown<string>(_sortButton.bounds.Right + 20, _sortButton.bounds.Y,
                Game1.smallFont, _categoryDropdown?.Selected ?? "All", _availableCategories, p => p);

            _categoryDropdown.IsExpanded = true;
        }

        private int GetMaxSortLabelWidth(SpriteFont font)
        {
            return
                (
                    from ItemSortOption key in Enum.GetValues(typeof(ItemSortOption))
                    let text = GetSortLabel(key)
                    select (int) font.MeasureString(text).X
                )
                .Max();
        }

        private string GetSortLabel(ItemSortOption sort)
        {
            return _sortLabelIndent + sort switch
            {
                ItemSortOption.Name => "Name",
                ItemSortOption.Category => "Category",
                ItemSortOption.ID => "ID",
                _ => throw new NotSupportedException($"Invalid sort type {sort}.")
            };
        }

        #region InputHandlers

        #endregion

        #region DrawHandlers

        public override void draw(SpriteBatch spriteBatch)
        {
            _baseDraw(spriteBatch);

            // DrawResearchArea(spriteBatch);
            
            _researchArea.Draw(spriteBatch);
            
            DrawQualityButton(spriteBatch);

            DrawSearchBox(spriteBatch);
            DrawSortBox(spriteBatch);
            DrawCategoryDropdown(spriteBatch);

            //TODO: draw held item

            drawMouse(spriteBatch);
        }

        // private void DrawResearchArea(SpriteBatch spriteBatch)
        // {
        //     RenderHelper.DrawMenuBox(_researchArea.bounds.X, _researchArea.bounds.Y,
        //         _researchArea.bounds.Width, _researchArea.bounds.Height, out var areaInnerAnchors);
        //
        //     var researchItemCellX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - Game1.tileSize / 2f;
        //     RenderHelper.DrawItemBox((int) researchItemCellX, (int) areaInnerAnchors.Y + 10, Game1.tileSize,
        //         Game1.tileSize,
        //         out _);
        //
        //     const string researchProgressString = "(0 / 20)";
        //     var progressFont = Game1.dialogueFont;
        //     var progressPositionX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f -
        //                             progressFont.MeasureString(researchProgressString).X / 2f;
        //     spriteBatch.DrawString(progressFont, researchProgressString,
        //         new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);
        //
        //     spriteBatch.Draw(_researchButton.texture, _researchButton.bounds, _researchButton.sourceRect, Color.White);
        // }

        private void DrawQualityButton(SpriteBatch spriteBatch)
        {
            GetCurrentQualityIcon(out var texture, out var sourceRect, out var color);

            RenderHelper.DrawMenuBox(_qualityButton.bounds.X, _qualityButton.bounds.Y,
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

        private void DrawSortBox(SpriteBatch spriteBatch)
        {
            RenderHelper.DrawTextMenuBox(_sortButton.bounds.X, _sortButton.bounds.Y, Game1.smallFont, _sortButton.name);
            _sortIcon.draw(spriteBatch);
        }

        private void DrawCategoryDropdown(SpriteBatch spriteBatch)
        {
            var position = new Vector2(
                x: _categoryDropdown.bounds.X + _categoryDropdown.bounds.Width - 12,
                y: _categoryDropdown.bounds.Y + 8
            );

            var sourceRect = CursorSprites.DropdownButton;
            
            spriteBatch.Draw(Game1.mouseCursors, position, sourceRect, Color.White, 0, Vector2.Zero, Game1.pixelZoom,
                SpriteEffects.None, 1f);

            if (_categoryDropdown.IsExpanded)
            {
                spriteBatch.Draw(Game1.mouseCursors,
                    new Vector2(position.X + 2 * Game1.pixelZoom, position.Y + 3 * Game1.pixelZoom),
                    new Rectangle(sourceRect.X + 2, sourceRect.Y + 3, 5, 6), Color.White, 0, Vector2.Zero,
                    Game1.pixelZoom, SpriteEffects.FlipVertically, 1f);
            }

            _categoryDropdown.Draw(spriteBatch);
        }

        private void DrawSearchBox(SpriteBatch spriteBatch)
        {
            RenderHelper.DrawMenuBox(_searchBoxBounds.X, _searchBoxBounds.Y - UIConstants.BorderWidth / 2,
                _searchBoxBounds.Width - UIConstants.BorderWidth * 3 / 2,
                _searchBoxBounds.Height - UIConstants.BorderWidth, out _);

            _searchBox.Draw(spriteBatch);
            spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect, Color.White);
        }

        #endregion
    }
}