using System;
using System.Collections.Generic;
using ItemResearchSpawner.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawner.Components
{
    /**
    This code is copied from Pathoschild.Stardew.Common.UI in https://github.com/Pathoschild/StardewMods,
    available under the MIT License. See that repository for the latest version.
    **/
    internal class Dropdown<TItem> : ClickableComponent
    {
        private readonly SpriteFont _font;

        private readonly DropdownList<TItem> List;

        private bool IsAndroid => Constants.TargetPlatform == GamePlatform.Android;

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                downNeighborID = value
                    ? List.TopComponentId
                    : DefaultDownNeighborId;
            }
        }

        public TItem Selected => List.SelectedValue;

        public int DefaultDownNeighborId { get; set; } = -99999;

        public Dropdown(int x, int y, SpriteFont font, TItem selectedItem, TItem[] items, Func<TItem, string> getLabel)
            : base(Rectangle.Empty, getLabel(selectedItem))
        {
            _font = font;

            List = new DropdownList<TItem>(selectedItem, items, getLabel, x, y, font);

            bounds.X = x;
            bounds.Y = y;

            List.bounds.X = bounds.X;
            List.bounds.Y = bounds.Bottom;

            List.ReinitializeComponents();
            
            bounds.Height = (int) _font.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZ").Y - 10 + UIConstants.BorderWidth;
            bounds.Width = List.MaxLabelWidth + UIConstants.BorderWidth * 2;

            List.ReinitializeControllerFlow();
            IsExpanded = IsExpanded;
        }

        public override bool containsPoint(int x, int y)
        {
            return
                base.containsPoint(x, y)
                || (IsExpanded && List.containsPoint(x, y));
        }

        public bool TryClick(int x, int y)
        {
            return TryClick(x, y, out _, out _);
        }

        public bool TryClick(int x, int y, out bool itemClicked, out bool dropdownToggled)
        {
            itemClicked = false;
            dropdownToggled = false;

            if (IsExpanded && List.TryClick(x, y, out itemClicked))
            {
                if (itemClicked)
                {
                    IsExpanded = false;
                    dropdownToggled = true;
                }

                return true;
            }

            if (bounds.Contains(x, y) || IsExpanded)
            {
                IsExpanded = !IsExpanded;
                dropdownToggled = true;

                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch sprites, float opacity = 1)
        {
            RenderHelpers.DrawMenuBox(bounds.X, bounds.Y, bounds.Width - UIConstants.BorderWidth * 2,
                List.MaxLabelHeight, out var textPos);
            
            sprites.DrawString(_font, List.SelectedLabel, textPos, Color.Black * opacity);

            if (IsExpanded)
            {
                List.Draw(sprites, opacity);
            }
        }
    }
}