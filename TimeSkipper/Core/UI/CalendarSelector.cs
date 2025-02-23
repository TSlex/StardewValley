using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
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

        public List<ClickableComponent> CalendarCells = new List<ClickableComponent>();

        public CalendarSelector(Func<int> getXPos, Func<int> getYPos) : base(new Rectangle(), "Calendar") {
            GetXPos = getXPos;
            GetYPos = getYPos;
        }

        public void RecreateCalendarCells(int x1, int y1) {
            CalendarCells.Clear();

            for (int j = 0; j < Capacity; j++) {

                var itemBounds = new Rectangle(
                    x1 + CalendarGapSize + j % ItemsPerRow * (CalendarCellSize + CalendarGapSize),
                    y1 + CalendarGapSize + j / ItemsPerRow * (CalendarCellSize + CalendarGapSize),
                    CalendarCellSize,
                    CalendarCellSize);

                CalendarCells.Add(new ClickableComponent(itemBounds, j.ToString() ?? ""));
            }
        }

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

            for (int j = 0; j < Capacity; j++) {

                var slot = CalendarCells[j];
                var location = new Vector2(slot.bounds.X, slot.bounds.Y);

                var day = j + 1;
                var dayName = day.ToString();
                var dayNameSize = Game1.smallFont.MeasureString(dayName);

                var c = day < Game1.dayOfMonth ? Color.LightGray * 1f : Color.White;
                c = day == Game1.dayOfMonth ? Color.Gold * 0.5f : c;

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
