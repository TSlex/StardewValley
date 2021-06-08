using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    internal class SpawnMenu : ItemGrabMenu
    {
        public SpawnMenu() : base(
            inventory: new List<Item>(),
            reverseGrab: false,
            showReceivingMenu: true,
            highlightFunction: item => true,
            behaviorOnItemGrab: (item, player) => { },
            behaviorOnItemSelectFunction: (item, player) => { },
            message: null,
            canBeExitedWithKey: true,
            showOrganizeButton: false,
            source: Constants.TargetPlatform == GamePlatform.Android ? source_chest : source_none
        )
        {
            movePosition(0, 100);
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

        // public override void draw(SpriteBatch b)
        // {
        //     base.draw(b);
        // }

        #endregion
    }
}