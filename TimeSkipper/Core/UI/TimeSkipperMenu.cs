using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using System.Collections.Generic;
using TimeSkipper.Core.Data.Enums;
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

        protected int displayerYear = Game1.year;
        protected GameSeason displayerSeason = (GameSeason) Utility.getSeasonNumber(Game1.currentSeason);

        public bool ShowLeftArrow => displayerYear > Game1.year || displayerYear == Game1.year && (int) displayerSeason > Utility.getSeasonNumber(Game1.currentSeason);

        public TimeSkipperMenu() {

            Game1.playSound("bigSelect");

            UpdateView();

            ModeDropdown = new Dropdown(
                () => XC - 200 - 4 * 10,
                () => YC + 4 * 32,
                Game1.smallFont, SleepSchedule.calendar_mode.GetString(),
                Enum.GetValues(typeof(SleepSchedule)).Cast<SleepSchedule>().Select(option => option.GetString()).ToArray(), 
                p => p, tabWidth: 348);

            CalendarSelector = new CalendarSelector(() => XC, () => Y1);

            LeftArrow = new ArrowButton(
                () => XC - 4 * 60,
                () => YC + 4 * 14,
                ArrowButtonType.Left);
            RightArrow = new ArrowButton(
                () => XC + 4 * 60 - UIConstants.RightArrow.Width + 4,
                () => YC + 4 * 14,
                ArrowButtonType.Right);

            SleepButton = new SleepButton(
                () => XC - 125,
                () => Y2 + 4 * 2,
                250,
                80
                );
        }

        // --------------------------------------------------------------------------------------------------

        public void ChangeDisplayedDate(int direction) {
            var newSeasonNumber = (int) displayerSeason + direction;

            if (newSeasonNumber < (int) GameSeason.spring) {
                displayerYear -= 1;
                displayerSeason = GameSeason.winter;
            }
            else if (newSeasonNumber > (int) GameSeason.winter) {
                displayerYear += 1;
                displayerSeason = GameSeason.spring;
            }
            else {
                displayerSeason = (GameSeason) newSeasonNumber;
            }

            CalendarSelector.SetDisplayedDate(displayerYear, displayerSeason);
        }

        public void ReseetDisplayedDate() {
            displayerYear = Game1.year;
            displayerSeason = (GameSeason) Utility.getSeasonNumber(Game1.currentSeason);

            CalendarSelector.SetDisplayedDate(displayerYear, displayerSeason);
        }

        protected void SetModeDropdown(bool expanded) {
            ModeDropdown.IsExpanded = expanded;

            if (!expanded && !Game1.lastCursorMotionWasMouse) {
                setCurrentlySnappedComponentTo(ModeDropdown.myID);
                snapCursorToCurrentSnappedComponent();
            }
        }

        protected void SetSleepSchedule(string category) {
            if (!ModeDropdown.TrySelect(category)) {
                ModManager.Instance.Monitor.Log($"Failed selecting mode '{category}'.", LogLevel.Warn);
                if (category != SleepSchedule.calendar_mode.GetString()) {
                    SetSleepSchedule(SleepSchedule.calendar_mode.GetString());
                }
            }

            //ModManager.Instance.SelectedCategory = category;
        }

        // --------------------------------------------------------------------------------------------------

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

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            if (LeftArrow.HoveredOver && ShowLeftArrow) {
                ChangeDisplayedDate(-1);
            }
            else if (RightArrow.HoveredOver) {
                ChangeDisplayedDate(1);
            }
            else if (CalendarSelector.containsPoint(x, y)) {
                CalendarSelector.HandleLeftClick(x, y);
            }

            else if (ModeDropdown.TryLeftClick(x, y, out bool itemClicked2, out bool dropdownToggled2)) {
                if (dropdownToggled2) {
                    SetModeDropdown(ModeDropdown.IsExpanded);
                }
                if (itemClicked2) {
                    SetSleepSchedule(ModeDropdown.Selected);
                    Game1.playSound("drumkit6");
                }
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true) {
            if (ModeDropdown.IsExpanded || ModeDropdown.containsPoint(x, y)) {
                if (ModeDropdown.Selected != SleepSchedule.calendar_mode.GetString()) {
                    SetSleepSchedule(SleepSchedule.calendar_mode.GetString());
                    Game1.playSound("smallSelect");
                }
                SetModeDropdown(false);
            }
            else if (LeftArrow.HoveredOver && ShowLeftArrow) {
                ReseetDisplayedDate();
            }
            else if (RightArrow.HoveredOver) {
                ReseetDisplayedDate();
            }
        }

        public override void performHoverAction(int x, int y) {
            LeftArrow.HandleHover(x, y);
            RightArrow.HandleHover(x, y);
            SleepButton.HandleHover(x, y);
            CalendarSelector.HandleHover(x, y);
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

            if (ShowLeftArrow) {
                LeftArrow.Draw(b);
            }
            RightArrow.Draw(b);

            var currentDateString = $"{displayerSeason.GetString()} --- {I18n.Calendar_Year()} {displayerYear}".ToLower();
            var currentDateWidth = Game1.smallFont.MeasureString(currentDateString);
            var currentDateLocation = new Vector2(XC, YC + 4 * 14);

            Utility.drawTextWithShadow(b,
                currentDateString,
                Game1.smallFont, currentDateLocation + new Vector2(-currentDateWidth.X / 2, 0),
                Color.Black);

            // Draw duration
            // ----------------------------------------------------

            var daysTextPluralForm = ModManager.Instance.DaysToSkip <= 1 ? I18n.Menu_Day() : I18n.Menu_Days();
            var sleepDurationString = $"{I18n.Menu_SleepDuration()}: {ModManager.Instance.DaysToSkip} {daysTextPluralForm}";
            var sleepDurationWidth = Game1.smallFont.MeasureString(sleepDurationString);
            var sleepDurationLocation = new Vector2(XC, Y2);

            Utility.drawTextWithShadow(b,
                sleepDurationString,
                Game1.smallFont,
                sleepDurationLocation + new Vector2(-sleepDurationWidth.X / 2, -4 * 20),
                Color.Black);

            // Draw start button
            // ----------------------------------------------------

            SleepButton.Draw(b);

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

            // Draw rest
            // ----------------------------------------------------

            drawMouse(b);
        }
    }
}
