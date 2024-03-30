using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using ItemResearchSpawnerV2.Core.Utils;
using StardewModdingAPI;

namespace ItemResearchSpawnerV2.Core.UI {

    /**
        MIT License

        Copyright (c) 2018 Pathoschild, CJBok

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
    **/

    internal class Dropdown<TItem> : ClickableComponent {

        // https://github.com/CJBok/SDV-Mods/blob/master/Common/UI/Dropdown.cs

        private readonly SpriteFont Font;
        private readonly DropdownList<TItem> List;
        private readonly int BorderWidth = 5 * 2 * Game1.pixelZoom;
        private readonly int? MaxTabWidth;
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

                bounds.Width = (value ? List.MaxLabelWidth : GetTabWidth()) + BorderWidth;
            }
        }
        public TItem Selected => List.SelectedValue;
        public IEnumerable<ClickableComponent> GetChildComponents() => List.GetChildComponents();


        public Dropdown(Func<int> getXPos, Func<int> getYPos, SpriteFont font, TItem selectedItem, TItem[] items, Func<TItem, string> getLabel, int? maxTabWidth = null)
            : base(Rectangle.Empty, getLabel(selectedItem)) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            Font = font;
            List = new DropdownList<TItem>(selectedItem, items, getLabel, getXPos(), getYPos(), font);

            bounds.X = getXPos();
            bounds.Y = getYPos();
            MaxTabWidth = maxTabWidth;

            ReinitializeComponents();
        }

        // -------------------------------------------------------------------------------------

        public override bool containsPoint(int x, int y) {
            return
                base.containsPoint(x, y)
                || (IsExpanded && List.containsPoint(x, y));
        }

        public bool TryClick(int x, int y) {
            return TryClick(x, y, out _, out _);
        }

        public bool TryClick(int x, int y, out bool itemClicked, out bool dropdownToggled) {
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

        public bool TrySelect(TItem value) {
            return List.TrySelect(value);
        }

        public void ReceiveScrollWheelAction(int direction) {
            if (IsExpanded) {
                List.ReceiveScrollWheelAction(direction);
            }
        }

        public void Draw(SpriteBatch b, float opacity = 1) {

            bounds.X = GetXPos();
            bounds.Y = GetYPos();
            List.bounds.X = bounds.X;
            List.bounds.Y = bounds.Y;

            // -------------------------------------------------------------------------------------

            int tabWidth = GetTabWidth();

            DrawHelper.DrawTab(bounds.X, bounds.Y, tabWidth, List.MaxLabelHeight, out Vector2 textPos, drawShadow: IsAndroid);

            b.DrawString(Font, List.SelectedLabel, textPos, Color.Black * opacity);

            Vector2 position = new(x: bounds.X + tabWidth + BorderWidth - 3 * Game1.pixelZoom, y: bounds.Y + 2 * Game1.pixelZoom);
            Rectangle sourceRect = new(437, 450, 10, 11);

            b.Draw(Game1.mouseCursors, position, sourceRect, Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 1f);

            if (IsExpanded) {
                b.Draw(Game1.mouseCursors, new Vector2(position.X + 2 * Game1.pixelZoom, position.Y + 3 * Game1.pixelZoom),
                    new Rectangle(sourceRect.X + 2, sourceRect.Y + 3, 5, 6), Color.White, 0, Vector2.Zero, Game1.pixelZoom, SpriteEffects.FlipVertically, 1f);
                
                List.Draw(b, opacity);
            }


        }

        public void ReinitializeComponents() {
            bounds.Height = (int)Font.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ").Y - 10 + BorderWidth;

            bounds.Width = GetTabWidth() + BorderWidth;

            List.bounds.X = bounds.X;
            List.bounds.Y = bounds.Bottom;

            List.ReinitializeComponents();
            ReinitializeControllerFlow();
        }

        public void ReinitializeControllerFlow() {
            List.ReinitializeControllerFlow();
            IsExpanded = IsExpanded;
        }

        // -------------------------------------------------------------------------------------

        private int GetTabWidth() {
            int tabWidth = List.MaxLabelWidth;
            if (tabWidth > MaxTabWidth)
                tabWidth = MaxTabWidth.Value;

            return tabWidth;
        }
    }

    internal class DropdownList<TValue> : ClickableComponent {

        // https://github.com/CJBok/SDV-Mods/blob/master/Common/UI/DropdownList.cs

        private const int DropdownPadding = 5;
        private DropListOption<TValue> SelectedOption;
        private readonly DropListOption<TValue>[] Options;
        private int FirstVisibleIndex;
        private int MaxItems;
        private readonly SpriteFont Font;
        private ClickableTextureComponent UpArrow;
        private ClickableTextureComponent DownArrow;

        public readonly int MaxLabelHeight;
        public int MaxLabelWidth {
            get; private set;
        }

        private int LastVisibleIndex => FirstVisibleIndex + MaxItems - 1;
        private int MaxFirstVisibleIndex => Options.Length - MaxItems;
        private bool CanScrollUp => FirstVisibleIndex > 0;
        private bool CanScrollDown => FirstVisibleIndex < MaxFirstVisibleIndex;

        public IEnumerable<ClickableComponent> GetChildComponents() => Options;
        public TValue SelectedValue => SelectedOption.Value;
        public string SelectedLabel => SelectedOption.label;
        public int TopComponentId => Options.First(p => p.visible).myID;


        public DropdownList(TValue selectedValue, TValue[] items, Func<TValue, string> getLabel, int x, int y, SpriteFont font)
            : base(new Rectangle(), nameof(DropdownList<TValue>)) {

            Options = items
                .Select((item, index) => new DropListOption<TValue>(Rectangle.Empty, index, getLabel(item), item, font))
                .ToArray();

            Font = font;
            MaxLabelHeight = Options.Max(p => p.LabelHeight);

            var selectedIndex = Array.IndexOf(items, selectedValue);

            SelectedOption = selectedIndex >= 0 ? Options[selectedIndex] : Options.First();

            bounds.X = x;
            bounds.Y = y;

            ReinitializeComponents();
        }

        // -------------------------------------------------------------------------------------

        public void ReceiveScrollWheelAction(int direction) {
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

        public bool TrySelect(TValue value) {
            var entry = Options.FirstOrDefault(p =>
                (p.Value == null && value == null) || p.Value?.Equals(value) == true
            );

            if (entry == null) {
                return false;
            }

            SelectedOption = entry;
            return true;
        }

        public override bool containsPoint(int x, int y) {
            return
                base.containsPoint(x, y)
                || UpArrow.containsPoint(x, y)
                || DownArrow.containsPoint(x, y);
        }

        public void Draw(SpriteBatch b, float opacity = 1) {
            foreach (DropListOption<TValue> option in Options) {
                if (!option.visible)
                    continue;

                if (option.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(161, 340, 4, 4), Color.White * opacity);
                else if (option.Index == SelectedOption.Index)
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(258, 258, 4, 4), Color.White * opacity);
                else
                    b.Draw(Game1.mouseCursors, option.bounds, new Rectangle(269, 258, 4, 4), Color.White * opacity);

                Vector2 position = new(option.bounds.X + DropdownList<TValue>.DropdownPadding, option.bounds.Y + Game1.tileSize / 16);
                b.DrawString(Font, option.label, position, Color.Black * opacity);
            }

            if (CanScrollUp)
                UpArrow.draw(b, Color.White * opacity, 1);
            if (CanScrollDown)
                DownArrow.draw(b, Color.White * opacity, 1);
        }

        public void ReinitializeComponents() {
            int x = bounds.X;
            int y = bounds.Y;

            int itemWidth = MaxLabelWidth = Math.Max(Options.Max(p => p.LabelWidth), Game1.tileSize * 2) + DropdownPadding * 2;
            int itemHeight = MaxLabelHeight;

            MaxItems = Math.Min((Game1.uiViewport.Height - y) / itemHeight, Options.Length);
            FirstVisibleIndex = GetValidFirstItem(FirstVisibleIndex, MaxFirstVisibleIndex);

            bounds.Width = itemWidth;
            bounds.Height = itemHeight * MaxItems;

            int itemY = y;

            foreach (DropListOption<TValue> option in Options) {
                option.visible = option.Index >= FirstVisibleIndex && option.Index <= LastVisibleIndex;

                if (option.visible) {
                    option.bounds = new Rectangle(x, itemY, itemWidth, itemHeight);
                    itemY += itemHeight;
                }
            }

            var upSource = new Rectangle(76, 72, 40, 44);
            var downSource = new Rectangle(12, 76, 40, 44);

            UpArrow = new ClickableTextureComponent("up-arrow",
                new Rectangle(x - upSource.Width, y, upSource.Width, upSource.Height), "", "", Game1.mouseCursors, upSource, 1);
            DownArrow = new ClickableTextureComponent("down-arrow",
                new Rectangle(x - downSource.Width, y + bounds.Height - downSource.Height, downSource.Width, downSource.Height), "", "", Game1.mouseCursors, downSource, 1);

            ReinitializeControllerFlow();
        }

        public void ReinitializeControllerFlow() {
            int firstIndex = FirstVisibleIndex;
            int lastIndex = LastVisibleIndex;

            int initialId = 1_100_000;
            foreach (DropListOption<TValue> option in Options) {
                int index = option.Index;
                int id = initialId + index;

                option.myID = id;
                option.upNeighborID = index > firstIndex
                    ? id - 1
                    : -99999;
                option.downNeighborID = index < lastIndex
                    ? id + 1
                    : -1;
            }
        }

        // -------------------------------------------------------------------------------------


        private void Scroll(int amount) {
            int firstItem = GetValidFirstItem(FirstVisibleIndex + amount, MaxFirstVisibleIndex);

            if (firstItem == FirstVisibleIndex) {
                return;
            }

            FirstVisibleIndex = firstItem;
            ReinitializeComponents();
        }

        private int GetValidFirstItem(int value, int maxIndex) {
            return Math.Max(Math.Min(value, maxIndex), 0);
        }
    }

    internal class DropListOption<TValue> : ClickableComponent {
        public int Index;
        public TValue Value;
        public int LabelWidth;
        public int LabelHeight;

        public DropListOption(Rectangle bounds, int index, string label, TValue value, SpriteFont font)
            : base(bounds, index.ToString(), label) {
            Index = index;
            Value = value;

            LabelWidth = DrawHelper.GetLabelWidth(font) - 10;
            LabelHeight = (int)font.MeasureString(label).Y;
        }
    }
}
