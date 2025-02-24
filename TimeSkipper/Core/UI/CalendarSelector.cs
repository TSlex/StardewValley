using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using TimeSkipper.Core.Data.Enums;
using TimeSkipper.Core.Utils;

namespace TimeSkipper.Core.UI {
    internal class CalendarSelector : ClickableComponent {

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        public int BaseWidth = 480;
        public int BaseHeight = 310;

        public readonly int Capacity = 28;
        public readonly int ItemsPerRow = 7;
        public readonly int CalendarCellSize = 52;
        public readonly int CalendarGapSize = 4;

        protected int displayerYear = Game1.year;
        protected GameSeason displayerSeason = (GameSeason) Utility.getSeasonNumber(Game1.currentSeason);

        public (int day, int season, int year) SelectedDay { get; protected set; }
        public bool CalendarActive = false;

        public bool IsNotCurrentDate => (int) displayerSeason != Utility.getSeasonNumber(Game1.currentSeason) || displayerYear > Game1.year;

        private int HoveredDay = -1;

        public List<CallendarCell> CalendarCells = new List<CallendarCell>();

        public CalendarSelector(Func<int> getXPos, Func<int> getYPos) : base(new Rectangle(), "Calendar") {
            GetXPos = getXPos;
            GetYPos = getYPos;

            var currentDay = Game1.dayOfMonth - 1;

            if (currentDay + 1 >= 28) {

                var nextSeason = ((GameSeason) Utility.getSeasonNumber(Game1.currentSeason)).GetNext();

                if (nextSeason == GameSeason.spring) {
                    SelectedDay = (0, (int) nextSeason, Game1.year + 1);
                }
                else {
                    SelectedDay = (0, (int) nextSeason, Game1.year);
                }
            }
            else {
                SelectedDay = (currentDay + 1, Utility.getSeasonNumber(Game1.currentSeason), Game1.year);
            }

            CalendarActive = true;
            UpdateSleepDays();
        }

        // --------------------------------------------------------------------------------------------------

        public void RecreateCalendarCells(int x1, int y1) {
            CalendarCells.Clear();

            for (int j = 0; j < Capacity; j++) {

                var itemBounds = new Rectangle(
                    x1 + CalendarGapSize + j % ItemsPerRow * (CalendarCellSize + CalendarGapSize),
                    y1 + CalendarGapSize + j / ItemsPerRow * (CalendarCellSize + CalendarGapSize),
                    CalendarCellSize,
                    CalendarCellSize);

                CalendarCells.Add(new CallendarCell(itemBounds, j.ToString() ?? ""));
            }
        }

        public void SetDisplayedDate(int year, GameSeason season) {
            displayerYear = year;
            displayerSeason = season;
        }

        public void HandleLeftClick(int x, int y) {
            for (int j = 0; j < CalendarCells.Count; j++) {
                if (CalendarCells[j].containsPoint(x, y)) {
                    
                    if (CanSelectDay(j)) {
                        SelectedDay = (j, (int) displayerSeason, displayerYear);
                        CalendarActive = true;
                        UpdateSleepDays();
                    }

                    return;
                }
            }
        }

        public virtual void HandleHover(int x, int y) {
            for (int j = 0; j < CalendarCells.Count; j++) {
                if (CalendarCells[j].containsPoint(x, y)) {
                    HoveredDay = j;
                    return;
                }
            }

            HoveredDay = -1;
        }

        public bool CanSelectDay(int dayIndex) {
            return dayIndex >= Game1.dayOfMonth || IsNotCurrentDate;
        }

        public bool IsSelectedDay(int dayIndex) {
            return CalendarActive && (dayIndex, (int) displayerSeason, displayerYear) == SelectedDay;
        }

        public void UpdateSleepDays() {
            var currentDateDays = Game1.dayOfMonth - 1 + 28 * (Utility.getSeasonNumber(Game1.currentSeason) + 4 * Game1.year);
            var targetDateDays = SelectedDay.day + 28 * (SelectedDay.season + 4 * SelectedDay.year);

            ModManager.Instance.DaysToSkip = targetDateDays - currentDateDays;
        }

        // --------------------------------------------------------------------------------------------------

        public void Draw(SpriteBatch b) {

            // Draw base
            // ----------------------------------------------------

            var calendarRect = DrawHelper.GetRectangleFromAnchor((GetXPos(), GetYPos() + 4 * 10), (1, 0), BaseWidth, BaseHeight);

            bounds.X = calendarRect.X;
            bounds.Y = calendarRect.Y;
            bounds.Width = calendarRect.Width;
            bounds.Height = calendarRect.Height;

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(calendarRect.X + 4 * 3, calendarRect.Y + 4 * 3, calendarRect.Width - 4 * 6, calendarRect.Height - 4 * 6),
                UIConstants.CalendarBackground, Color.White);

            DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.BarFrame,
                    calendarRect, cornerSize: 16, colorize: true);

            // Draw cells and week days
            // ----------------------------------------------------

            var WeekDayOffcet = 4 * 9;
            var cellsTotalWidth = (CalendarCellSize + CalendarGapSize) * ItemsPerRow + CalendarGapSize;
            var cellsTotalHeight = (CalendarCellSize + CalendarGapSize) * (Capacity / ItemsPerRow) + CalendarGapSize + WeekDayOffcet;

            var cellsRect = DrawHelper.GetRectangleFromAnchor(
                (calendarRect.X + calendarRect.Width / 2, calendarRect.Y + calendarRect.Height / 2),
                (1, 1), cellsTotalWidth, cellsTotalHeight);

            RecreateCalendarCells(cellsRect.X, cellsRect.Y + WeekDayOffcet - 4 * 1);

            for (int d = 0; d < ItemsPerRow; d++) {
                var slot = CalendarCells[d];
                var location = new Vector2(slot.bounds.X, slot.bounds.Y - WeekDayOffcet);

                var weekDayName = GetWeekDayName(d);
                var weekDaySize = Game1.smallFont.MeasureString(weekDayName);

                Utility.drawTextWithShadow(b,
                    weekDayName,
                    Game1.smallFont, location + new Vector2(CalendarCellSize / 2 - weekDaySize.X / 2, 0),
                    Color.Black);
            }

            var isNotCurrentDate = IsNotCurrentDate;

            for (int j = 0; j < Capacity; j++) {

                var slot = CalendarCells[j];
                var location = new Vector2(slot.bounds.X, slot.bounds.Y);

                var day = j + 1;
                var dayName = day.ToString();
                var dayNameSize = Game1.smallFont.MeasureString(dayName);

                var c = day < Game1.dayOfMonth ? Color.LightGray * 1f : Color.White;
                c = day == Game1.dayOfMonth ? Color.Gold * 0.5f : c;
                c = isNotCurrentDate ? Color.White : c;
                c = HoveredDay == j && CanSelectDay(j) ? Color.Blue * 0.3f : c;
                //c = IsSelectedDay(j) ? Color.Red * 0.3f : c;


                // Draw gaps
                // ----------------------------------------------------

                b.Draw(ModManager.UITextureInstance,
                    new Rectangle(slot.bounds.X - 4, slot.bounds.Y - 4, 4, CalendarCellSize + 4),
                    UIConstants.CalendarCellGap, Color.White);

                b.Draw(ModManager.UITextureInstance,
                    new Rectangle(slot.bounds.X - 4, slot.bounds.Y - 4, CalendarCellSize + 4, 4),
                    UIConstants.CalendarCellGap, Color.White);

                if (j % ItemsPerRow == ItemsPerRow - 1) {
                    b.Draw(ModManager.UITextureInstance,
                        new Rectangle(slot.bounds.X + CalendarCellSize, slot.bounds.Y - 4, 4, CalendarCellSize + 4),
                        UIConstants.CalendarCellGap, Color.White);
                }

                if (j / ItemsPerRow == Capacity / ItemsPerRow - 1) {
                    b.Draw(ModManager.UITextureInstance,
                        new Rectangle(slot.bounds.X - 4, slot.bounds.Y + CalendarCellSize, CalendarCellSize + 8, 4),
                        UIConstants.CalendarCellGap, Color.White);
                }

                // Draw cells
                // ----------------------------------------------------

                b.Draw(ModManager.UITextureInstance, new Rectangle((int) location.X, (int) location.Y, CalendarCellSize, CalendarCellSize),
                    UIConstants.CalendarCell, c);

                Utility.drawTextWithShadow(b,
                    dayName,
                    Game1.smallFont, location + new Vector2(CalendarCellSize / 2 - dayNameSize.X / 2, CalendarCellSize / 2 - 4 * 4),
                    Color.Black);
            }

            // Draw selected day frame
            // ----------------------------------------------------

            if (IsSelectedDay(SelectedDay.day)) {
                var slot = CalendarCells[SelectedDay.day];

                var sdFrame = DrawHelper.GetRectangleFromAnchor(
                    (slot.bounds.Center.X, slot.bounds.Center.Y), 
                    (1, 1), CalendarCellSize + 4 * 6, CalendarCellSize + 4 * 4);

                b.Draw(ModManager.UITextureInstance, 
                    new Rectangle(calendarRect.X, slot.bounds.Y + 4, sdFrame.X - calendarRect.X, CalendarCellSize - 4 * 2), 
                    UIConstants.CalendarCellSelectedBackground, Color.White * 0.4f);

                b.Draw(ModManager.UITextureInstance,
                    new Rectangle(sdFrame.Right, slot.bounds.Y + 4, calendarRect.Right - sdFrame.Right, CalendarCellSize - 4 * 2),
                    UIConstants.CalendarCellSelectedBackground, Color.White * 0.4f);

                DrawHelper.DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.CalendarCellSelectedFrame,
                    sdFrame, cornerSize: 16, colorize: true);
            }
        }

        public string GetWeekDayName(int d) {
            return d switch {
                0 => I18n.Calendar_DayMon(),
                1 => I18n.Calendar_DayTue(),
                2 => I18n.Calendar_DayWed(),
                3 => I18n.Calendar_DayThu(),
                4 => I18n.Calendar_DayFri(),
                5 => I18n.Calendar_DaySat(),
                6 => I18n.Calendar_DaySun(),
                _ => "",
            };
        }
    }
}
