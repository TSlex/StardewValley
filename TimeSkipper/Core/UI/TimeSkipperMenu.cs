using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using TimeSkipper.Core.Utils;

namespace TimeSkipper.Core.UI {
    internal class TimeSkipperMenu : IClickableMenu {

        public int Width => width;
        public int Height => height;
        public int X1 => xPositionOnScreen;
        public int Y1 => yPositionOnScreen;
        public int X2 => xPositionOnScreen + Width;
        public int Y2 => yPositionOnScreen + Height;
        public int XC => X1 + Width / 2;
        public int YC => Y1 + Height / 2;

        public int BaseWidth = 500;
        public int BaseHeight = 550;

        protected readonly Dropdown ModeDropdown;
        protected readonly CalendarSelector CalendarSelector;
        protected readonly ArrowButton LeftArrow;
        protected readonly ArrowButton RightArrow;
        protected readonly SleepButton SleepButton;

        public TimeSkipperMenu() {

            Game1.playSound("bigSelect");

            UpdateView();

            ModeDropdown = new Dropdown(
                () => XC - 200 - 4 * 10,
                () => YC + 4 * 32,
                Game1.smallFont, "Calendar", new[] { "Calendar" }, p => p, tabWidth: 348);

            CalendarSelector = new CalendarSelector(() => XC, () => Y1);

            LeftArrow = new ArrowButton(
                () => XC + 4 * 60 - UIConstants.RightArrow.Width + 4,
                () => YC + 4 * 14,
                ArrowButtonType.Left);
            RightArrow = new ArrowButton(
                () => XC - 4 * 60,
                () => YC + 4 * 14,
                ArrowButtonType.Right);

            SleepButton = new SleepButton(
                () => XC - 125,
                () => Y2 + 4 * 2,
                250,
                80
                );
        }

        public override void draw(SpriteBatch b) {
            DrawMenu(b);
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds) {
            UpdateView();
        }

        // --------------------------------------------------------------------------------------------------

        private void UpdateView() {
            width = BaseWidth + IClickableMenu.borderWidth * 2;
            height = BaseHeight + IClickableMenu.borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - width / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;
        }

        // --------------------------------------------------------------------------------------------------

        protected void DrawMenu(SpriteBatch b) {

            if (!Game1.options.showClearBackgrounds) {
                b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height), Color.Black * 0.5f);
            }

            // Draw header
            // ----------------------------------------------------

            var headerRect = DrawHelper.GetRectangleFromAnchor((XC, Y1 - 4 * 2), (1, 2), 348, 100);

            b.Draw(ModManager.UITextureInstance,
                DrawHelper.GetRectangleFromAnchor((headerRect.X + 4 * 3, headerRect.Y + 4 * 3), (0, 0),
                headerRect.Width - 4 * 6, headerRect.Height - 4 * 6),
                UIConstants.BaseBackground, Color.White);

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.BarFrame,
                    headerRect, cornerSize: 16, colorize: true);

            var headerTextRect = DrawHelper.GetRectangleFromAnchor(
                (headerRect.X + headerRect.Width / 2, headerRect.Y + headerRect.Height / 2),
                (1, 1), UIConstants.HeaderText.Width, UIConstants.HeaderText.Height);

            b.Draw(ModManager.UITextureInstance,
                new Vector2(headerTextRect.X, headerTextRect.Y),
                UIConstants.HeaderText, Color.White);

            // Draw background
            // ----------------------------------------------------

            b.Draw(ModManager.UITextureInstance,
                DrawHelper.GetRectangleFromAnchor((X1 + 4 * 3, Y1 + 4 * 3), (0, 0),
                Width - 4 * 6, Height - 4 * 6),
                UIConstants.BaseBackground, Color.White);

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.MenuFrame,
                    new Rectangle(X1, Y1, Width, Height), cornerSize: 64, colorize: true);

            // Draw calendar
            // ----------------------------------------------------

            CalendarSelector.Draw(b);

            // Draw arrows and current date
            // ----------------------------------------------------

            LeftArrow.Draw(b);
            RightArrow.Draw(b);

            var currentDateString = $"{Game1.CurrentSeasonDisplayName} --- {I18n.Calendar_Year()} {Game1.year}".ToLower();
            var currentDateWidth = Game1.smallFont.MeasureString(currentDateString);
            var currentDateLocation = new Vector2(XC, LeftArrow.Bounds.Y);

            Utility.drawTextWithShadow(b,
                currentDateString,
                Game1.smallFont, currentDateLocation + new Vector2(-currentDateWidth.X / 2, 0),
                Color.Black);

            //if (ShowLeftButton) {
            //    LeftArrow.Draw(b);
            //}
            //if (ShowRightButton) {
            //    RightArrow.Draw(b);
            //}

            // Draw dropdown and dropdown checker
            // ----------------------------------------------------

            ModeDropdown.Draw(b);

            var DDCheckerRect = DrawHelper.GetRectangleFromAnchor(
                (ModeDropdown.bounds.X + ModeDropdown.bounds.Width + 4 * 3,
                ModeDropdown.bounds.Y + ModeDropdown.bounds.Height / 2),
                (0, 1), 92, 92);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(DDCheckerRect.X + 4 * 3, DDCheckerRect.Y + 4 * 3, DDCheckerRect.Width - 4 * 6, DDCheckerRect.Height - 4 * 6),
                UIConstants.CalendarBackground, Color.White);

            var DDCheckerInnerRect = new Rectangle(
                DDCheckerRect.X + 4 * 4,
                DDCheckerRect.Y + 4 * 4,
                DDCheckerRect.Width - 4 * 8,
                DDCheckerRect.Height - 4 * 8);

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.CalendarCellWithFrame,
                DDCheckerInnerRect,
                cornerSize: 4, colorize: true);

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.BarFrame,
                DDCheckerRect, cornerSize: 16, colorize: true);

            Utility.drawTextWithShadow(b,
                "?",
                Game1.smallFont,
                new Vector2(DDCheckerInnerRect.X + DDCheckerInnerRect.Width / 2,
                DDCheckerInnerRect.Y + DDCheckerInnerRect.Height / 2) + new Vector2(-Game1.smallFont.MeasureString("?").X / 2, -4 * 4),
                Color.Black);

            // Draw duration
            // ----------------------------------------------------

            var sleepDurationString = $"{I18n.Menu_SleepDuration()}: {12} {I18n.Menu_Days()}";
            var sleepDurationWidth = Game1.smallFont.MeasureString(sleepDurationString);
            var sleepDurationLocation = new Vector2(XC, Y2);

            Utility.drawTextWithShadow(b,
                sleepDurationString,
                Game1.smallFont,
                sleepDurationLocation + new Vector2(-sleepDurationWidth.X / 2, - 4 * 20),
                Color.Black);

            // Draw start button
            // ----------------------------------------------------

            SleepButton.Draw(b);

            // Draw rest
            // ----------------------------------------------------

            drawMouse(b);
        }
    }
}
