using System;
using System.Collections.Generic;
using System.Reflection;
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

        private ClickableComponent _researchArea;
        private ClickableComponent _researchButton;

        private static bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        public SpawnMenu(IMonitor monitor) : base(
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

            _baseDraw = GetBaseDraw();

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

            _researchArea =
                new ClickableComponent(
                    new Rectangle(sideRightAnchor, sideTopAnchor, Game1.tileSize + 60, Game1.tileSize + 50), "");
        }

        #region InputHandlers

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (trashCan.containsPoint(x, y) && heldItem != null)
            {
                Utility.trashItem(heldItem);
                heldItem = null;
            }
            else
            {
                base.receiveLeftClick(x, y, playSound);
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
            base.receiveRightClick(x, y, playSound);
        }

        #endregion

        #region DrawHandlers

        public override void draw(SpriteBatch spriteBatch)
        {
            _baseDraw(spriteBatch);
            
            // draw research area
            RenderHelper.DrawMenuBox(_researchArea.bounds.X, _researchArea.bounds.Y,
                _researchArea.bounds.Width, _researchArea.bounds.Height, out var areaInnerAnchors);

            var researchItemCellX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - Game1.tileSize / 2f;
            RenderHelper.DrawItemBox((int) researchItemCellX, (int) areaInnerAnchors.Y + 10, Game1.tileSize, Game1.tileSize,
                out _);

            const string researchProgressString = "( 0 / 20 )";
            var progressFont = Game1.dialogueFont;
            var progressPositionX = areaInnerAnchors.X + _researchArea.bounds.Width / 2f - progressFont.MeasureString(researchProgressString).X / 2f;
            spriteBatch.DrawString(progressFont, researchProgressString, new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);
            
            //TODO: draw held item
            
            drawMouse(spriteBatch);
        }

        #endregion

        private Action<SpriteBatch> GetBaseDraw()
        {
            var method =
                typeof(ItemGrabMenu).GetMethod("draw", BindingFlags.Instance | BindingFlags.Public, null,
                    new[] {typeof(SpriteBatch)}, null) ??
                throw new InvalidOperationException(
                    $"Can't find {nameof(ItemGrabMenu)}.{nameof(ItemGrabMenu.draw)} method.");

            var pointer = method.MethodHandle.GetFunctionPointer();

            return (Action<SpriteBatch>) Activator.CreateInstance(typeof(Action<SpriteBatch>), this, pointer);
        }
    }
}