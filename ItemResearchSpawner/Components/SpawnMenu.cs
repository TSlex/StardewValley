using System;
using System.Collections.Generic;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Constants = StardewModdingAPI.Constants;

namespace ItemResearchSpawner.Components
{
    internal class SpawnMenu : ItemGrabMenu
    {
        private readonly SpawnableItem[] _spawnableItems;
        private readonly IMonitor _monitor;
        private readonly Action<SpriteBatch> _baseDraw;
        private readonly IContentHelper _content;

        // private TextBox _searchBox;
        // private Rectangle _searchBoxBounds;
        // private ClickableComponent _searchBoxArea;
        // private ClickableTextureComponent _searchIcon;
        //
        // private string _searchText;

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
            _spawnableItems = spawnableItems;
            _monitor = monitor;
            _content = content;
            _baseDraw = RenderHelper.GetBaseDraw(this);

            // _searchText = "Pumpkin";

            InitializeComponents();
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

            _researchArea = new ItemResearchArea(_content, _monitor, sideRightAnchor, sideTopAnchor);
            _qualitySelector = new ItemQualitySelectorTab(_content, _monitor, rootLeftAnchor - 8, barTopAnchor);
            _itemSortTab = new ItemSortTab(_content, _monitor, _qualitySelector.Bounds.Right + 20, barTopAnchor);
            _categorySelector = new ItemCategorySelectorTab(_content, _monitor, _spawnableItems,
                _itemSortTab.Bounds.Right + 20, _itemSortTab.Bounds.Y);
            _searchBarTab = new ItemSearchBarTab(_content, _monitor, _categorySelector.Bounds.Right + 20, barTopAnchor);

            // _searchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
            //     Game1.textColor)
            // {
            //     X = rootRightAnchor - 52,
            //     Y = barTopAnchor,
            //     Height = 0,
            //     Width = 200,
            //     Text = _searchText
            // };
            //
            // _searchBoxBounds = new Rectangle(_searchBox.X, _searchBox.Y + 4, _searchBox.Width, 12 * Game1.pixelZoom);
            //
            // _searchBoxArea =
            //     new ClickableComponent(
            //         new Rectangle(_searchBoxBounds.X, _searchBoxBounds.Y, _searchBoxBounds.Width,
            //             _searchBoxBounds.Height), "");
            //
            // var iconRect = new Rectangle(80, 0, 13, 13);
            // const float iconScale = 2.5f;
            //
            // var iconBounds = new Rectangle((int) (_searchBoxBounds.Right - iconRect.Width * iconScale),
            //     (int) (_searchBoxBounds.Center.Y - iconRect.Height / 2f * iconScale),
            //     (int) (iconRect.Width * iconScale), (int) (iconRect.Height * iconScale)
            // );
            //
            // _searchIcon = new ClickableTextureComponent(iconBounds, Game1.mouseCursors, iconRect, iconScale);
        }

        #region InputHandlers

        #endregion

        #region DrawHandlers

        public override void draw(SpriteBatch spriteBatch)
        {
            _baseDraw(spriteBatch);

            _researchArea.Draw(spriteBatch);
            _qualitySelector.Draw(spriteBatch);
            _itemSortTab.Draw(spriteBatch);
            _categorySelector.Draw(spriteBatch);
            _searchBarTab.Draw(spriteBatch);
            
            //TODO: draw held item

            drawMouse(spriteBatch);
        }

        // private void DrawSearchBox(SpriteBatch spriteBatch)
        // {
        //     RenderHelper.DrawMenuBox(_searchBoxBounds.X, _searchBoxBounds.Y - UIConstants.BorderWidth / 2,
        //         _searchBoxBounds.Width - UIConstants.BorderWidth * 3 / 2,
        //         _searchBoxBounds.Height - UIConstants.BorderWidth, out _);
        //
        //     _searchBox.Draw(spriteBatch);
        //     spriteBatch.Draw(_searchIcon.texture, _searchIcon.bounds, _searchIcon.sourceRect, Color.White);
        // }

        #endregion
    }
}