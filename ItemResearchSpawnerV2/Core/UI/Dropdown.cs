using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using ItemResearchSpawnerV2.Core.Utils;
using StardewModdingAPI;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class Dropdown : ClickableComponent {
        // https://github.com/CJBok/SDV-Mods/blob/master/Common/UI/Dropdown.cs

        private readonly SpriteFont Font;
        private readonly Func<string, string> GetLabel;
        private DropdownList List;
        private readonly int TabWidth;
        private bool IsExpandedImpl;
        private bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;


        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;


        public int DefaultDownNeighborId = -99999;
        public bool IsExpanded {
            get => IsExpandedImpl;
            set {
                IsExpandedImpl = value;
                downNeighborID = value
                    ? List.TopComponentId
                    : DefaultDownNeighborId;

                //bounds.Width = (value ? List.MaxLabelWidth : GetTabWidth()) + BorderWidth;
            }
        }
        public string Selected => List.SelectedValue;
        public IEnumerable<ClickableComponent> GetChildComponents() => List.GetChildComponents();

        public Dropdown(Func<int> getXPos, Func<int> getYPos, SpriteFont font, string selectedItem, IList<string> items,
            Func<string, string> getLabel, int tabWidth = 100) : base(Rectangle.Empty, getLabel(selectedItem)) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            Font = font;

            GetLabel = getLabel;

            bounds.X = getXPos();
            bounds.Y = getYPos();

            TabWidth = tabWidth;

            SetOptions(selectedItem, items);
        }

        // -------------------------------------------------------------------------------------

        public void SetOptions(string selectedItem, IList<string> items) {
            List = new DropdownList(selectedItem, items, GetLabel, GetXPos(), GetYPos(), TabWidth - 3 * 4, Font);
            Update();
        }

        public void SetOptions(IList<string> items) {
            SetOptions(List.SelectedValue, items);
        }

        public bool TryLeftClick(int x, int y) {
            return TryLeftClick(x, y, out _, out _);
        }

        public bool TryLeftClick(int x, int y, out bool itemClicked, out bool dropdownToggled) {

            itemClicked = false;
            dropdownToggled = false;

            if (IsExpanded && List.TryClick(x, y, out itemClicked)) {
                if (itemClicked) {
                    IsExpanded = false;
                    dropdownToggled = true;
                }
                return true;
            }

            if (bounds.Contains(x, y) || IsExpanded) {
                IsExpanded = !IsExpanded;
                dropdownToggled = true;
                return true;
            }

            return false;
        }

        public bool TrySelect(string value) {
            if (Selected == value) { 
                return true;
            }

            var succ = List.TrySelect(value);

            Update();

            return succ;
        }

        public void OnScrollWheel(int direction) {
            if (IsExpanded) {
                List.OnScrollWheel(direction);
            }
        }

        public void Draw(SpriteBatch b, float opacity = 1) {

            var newX = GetXPos();
            var newY = GetYPos();

            if (bounds.X != newX || bounds.Y != newY) {
                bounds.X = newX;
                bounds.Y = newY;

                Update();
            }

            // -------------------------------------------------------------------------------------

            DrawHelper.DrawTab(bounds.X, bounds.Y, TabWidth, List.MaxLabelHeight, out Vector2 textPos, drawShadow: IsAndroid);

            //b.DrawString(Font, List.SelectedLabel, textPos, Color.Black * opacity);
            b.DrawString(Font, DrawHelper.TruncateString(List.SelectedLabel, Font, TabWidth), textPos,
                Color.Black * opacity);

            var sourceRect = new Rectangle(439, 453, 5, 5);
            var position = new Vector2(bounds.X + TabWidth - sourceRect.Width, bounds.Y + 20);

            b.Draw(Game1.mouseCursors, position, sourceRect, Color.White, 0, Vector2.Zero, Game1.pixelZoom,
                IsExpanded ? SpriteEffects.FlipVertically : SpriteEffects.None, 1f);

            if (IsExpanded) {
                List.Draw(b, opacity);
            }
        }

        public void Update() {
            bounds.Height = (int)Font.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ").Y - 10 + 4 * 9;
            bounds.Width = TabWidth + 4 * 7;

            List.bounds.X = bounds.X;
            List.bounds.Y = bounds.Bottom;
            List.Update();
        }

        public override bool containsPoint(int x, int y) {
            return base.containsPoint(x, y) || (IsExpanded && List.containsPoint(x, y));
        }

        // -------------------------------------------------------------------------------------
    }

    internal class DropdownList : ClickableComponent {

        // https://github.com/CJBok/SDV-Mods/blob/master/Common/UI/DropdownList.cs

        private DropListOption SelectedOption;

        private readonly List<DropListOption> Options = new();

        private int FirstVisibleIndex;
        private int MaxItems;
        private readonly Func<string, string> GetLabel;
        private readonly SpriteFont Font;
        private ClickableTextureComponent UpArrow;
        private ClickableTextureComponent DownArrow;

        public int MaxLabelHeight;
        public int BaseWidth;

        private int LastVisibleIndex => FirstVisibleIndex + MaxItems - 1;
        private int MaxFirstVisibleIndex => Options.Count - MaxItems;
        private bool CanScrollUp => FirstVisibleIndex > 0;
        private bool CanScrollDown => FirstVisibleIndex < MaxFirstVisibleIndex;

        public IEnumerable<ClickableComponent> GetChildComponents() => Options;
        public string SelectedValue => SelectedOption.Value;
        public string SelectedLabel => SelectedOption.label;
        public int TopComponentId => Options.First(p => p.visible).myID;

        private readonly string DefaultValue;


        public DropdownList(string selectedValue, IList<string> items, Func<string, string> getLabel, int x, int y, int baseWidth, SpriteFont font)
            : base(new Rectangle(), nameof(DropdownList)) {

            BaseWidth = baseWidth;
            GetLabel = getLabel;
            Font = font;

            DefaultValue = selectedValue;

            SetOptions(selectedValue, items);

            bounds.X = x;
            bounds.Y = y;

            Update();
        }

        // -------------------------------------------------------------------------------------

        public void SetOptions(string selectedValue, IList<string> items) {
            Options.Clear();
            Options.AddRange(items.Select(
                (item, index) => new DropListOption(Rectangle.Empty, index, GetLabel(item), item, Font))
            );

            MaxLabelHeight = Options.Max(p => p.LabelHeight);

            var selectedIndex = items.IndexOf(selectedValue);
            SelectedOption = selectedIndex >= 0 ? Options[selectedIndex] : Options.First();
        }

        public void SetOptions(IList<string> items) {
            SetOptions(SelectedValue, items);
        }

        public void OnScrollWheel(int direction) {
            Scroll(direction > 0 ? -1 : 1);
        }

        public bool TryClick(int x, int y, out bool itemClicked) {
            var option = Options.FirstOrDefault(p => p.visible && p.containsPoint(x, y));

            if (option != null) {
                SelectedOption = option;
                itemClicked = true;

                return true;
            }

            itemClicked = false;

            if (UpArrow.containsPoint(x, y)) {
                Scroll(-1);
                return true;
            }
            if (DownArrow.containsPoint(x, y)) {
                Scroll(1);
                return true;
            }

            return false;
        }

        public bool TrySelect(string value) {
            var entry = Options.FirstOrDefault(p =>
                (p.Value == null && value == null) || p.Value?.Equals(value) == true
            );

            if (entry == null) {
                return false;
            }

            if (entry.label == DefaultValue) {
                FirstVisibleIndex = 0;
            }

            SelectedOption = entry;
            Game1.playSound("smallSelect");

            return true;
        }

        public override bool containsPoint(int x, int y) {
            return base.containsPoint(x, y) || UpArrow.containsPoint(x, y) || DownArrow.containsPoint(x, y);
        }

        public void Draw(SpriteBatch b, float opacity = 1) {
            foreach (DropListOption option in Options) {
                if (!option.visible) {
                    continue;
                }

                if (option.containsPoint(Game1.getMouseX(), Game1.getMouseY())) {
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(161, 340, 4, 4), Color.White * opacity);
                }
                else if (option.Index == SelectedOption.Index) {
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(258, 258, 4, 4), Color.White * opacity);
                }
                else {
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(269, 258, 4, 4), Color.White * opacity);
                }

                Vector2 position = new(option.bounds.X + 10, option.bounds.Y + Game1.tileSize / 16);

                //b.DrawString(Font, option.label, position, Color.Black * opacity);
                b.DrawString(Font, DrawHelper.TruncateString(option.label, Font, option.bounds.Width), position,
                    Color.Black * opacity);
            }

            if (CanScrollUp) {
                UpArrow.draw(b, Color.White * opacity, 1);
            }
            if (CanScrollDown) {
                DownArrow.draw(b, Color.White * opacity, 1);
            }
        }

        public void Update() {
            var x = bounds.X + 4 * 3;
            var y = bounds.Y + 4 * -3;

            var itemWidth = Math.Max(Options.Max(p => p.LabelWidth) - 80, BaseWidth) + 20;
            var itemHeight = MaxLabelHeight;

            MaxItems = Math.Min(10, Options.Count);

            FirstVisibleIndex = GetFirstValidItem(FirstVisibleIndex, MaxFirstVisibleIndex);

            bounds.Width = itemWidth;
            bounds.Height = itemHeight * MaxItems;

            var itemY = y;

            foreach (DropListOption option in Options) {
                option.visible = option.Index >= FirstVisibleIndex && option.Index <= LastVisibleIndex;

                if (option.visible) {
                    option.bounds = new Rectangle(x, itemY, itemWidth, itemHeight);
                    itemY += itemHeight;
                }
            }

            var upSource = new Rectangle(76, 72, 40, 44);
            var downSource = new Rectangle(12, 76, 40, 44);

            UpArrow = new ClickableTextureComponent("up-arrow",
                new Rectangle(x - upSource.Width - 4 * 3, y + 4 * 8, upSource.Width, upSource.Height), "", "", Game1.mouseCursors, upSource, 1);
            DownArrow = new ClickableTextureComponent("down-arrow",
                new Rectangle(x - downSource.Width - 4 * 3, y + bounds.Height - downSource.Height - 4 * 8, downSource.Width, downSource.Height), "", "", Game1.mouseCursors, downSource, 1);
        }

        // -------------------------------------------------------------------------------------


        private void Scroll(int amount) {
            var firstItem = GetFirstValidItem(FirstVisibleIndex + amount, MaxFirstVisibleIndex);

            if (firstItem == FirstVisibleIndex) {
                return;
            }

            FirstVisibleIndex = firstItem;

            Update();

            Game1.playSound("boulderCrack");
        }

        private static int GetFirstValidItem(int value, int maxIndex) {
            return Math.Max(Math.Min(value, maxIndex), 0);
        }
    }

    internal class DropListOption : ClickableComponent {
        public int Index;
        public string Value;
        public int LabelWidth;
        public int LabelHeight;

        public DropListOption(Rectangle bounds, int index, string label, string value, SpriteFont font)
            : base(bounds, index.ToString(), label) {
            Index = index;
            Value = value;

            LabelWidth = DrawHelper.GetLabelWidth(font) - 10;
            LabelHeight = (int)font.MeasureString(label).Y;
        }
    }
}
