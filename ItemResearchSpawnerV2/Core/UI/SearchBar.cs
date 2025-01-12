using System;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using static System.Net.Mime.MediaTypeNames;

namespace ItemResearchSpawnerV2.Core.UI {
    /**
        MIT License

        Copyright (c) 2018 CJBok

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

    internal class SearchBar {

        private readonly TextBox SearchBox;
        private readonly ClickableComponent SearchBoxArea;

        private readonly ClickableTextureComponent SearchBarButton;

        private readonly Rectangle SearchTexture = new(80, 0, 13, 13);
        private readonly Rectangle ClearTexture = new(290, 344, 9, 9);


        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        private float IconOpacity;

        public bool PersistFocus {
            get; private set;
        }

        public bool Selected => SearchBox.Selected;
        public bool IsSearchBoxSelectionChanging => IconOpacity > 0 && IconOpacity < 1;

        public Rectangle Bounds => SearchBoxArea.bounds;
        public string Text => SearchBox.Text.Trim();

        public SearchBar(Func<int> getXPos, Func<int> getYPos, int width) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            IconOpacity = 1f;

            SearchBoxArea = new ClickableComponent(
                    new Rectangle(getXPos(), getYPos(), width + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");

            SearchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor) {
                X = SearchBoxArea.bounds.X,
                Y = SearchBoxArea.bounds.Y + 2,
                Height = 0,
                Width = SearchBoxArea.bounds.Width,
                Text = ""
            };

            var iconLocation = new Rectangle((int)(SearchBoxArea.bounds.Right - SearchTexture.Width * 2f),
                (int)(SearchBoxArea.bounds.Y + 16),
                (int)(SearchTexture.Width * 2f), (int)(SearchTexture.Height * 2f)
            );

            SearchBarButton = new ClickableTextureComponent(iconLocation, Game1.mouseCursors, SearchTexture, 2f);
        }

        public void Focus(bool persist) {
            SearchBox.Selected = true;
            PersistFocus = persist;
        }

        public void Blur() {
            Game1.closeTextEntry();
            SearchBox.Selected = false;
            PersistFocus = false;
        }

        public void Clear() {
            SearchBox.Text = "";
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("cancel");
            }
        }

        public void SetText(string text) {
            SearchBox.Text = text;
        }

        public bool Contains(int x, int y) {
            return Bounds.Contains(x, y);
        }

        public void HandleLeftClick(int x, int y) {
            if (SearchBarButton.bounds.Contains(x, y)) {
                Clear();
            }

            Focus(true);
        }

        public void Draw(SpriteBatch b) {

            SearchBoxArea.bounds.X = GetXPos();
            SearchBoxArea.bounds.Y = GetYPos();

            SearchBox.X = SearchBoxArea.bounds.X;
            SearchBox.Y = SearchBoxArea.bounds.Y + 4 * 1 - 2;

            var iconNewLocation = new Rectangle((int)(SearchBoxArea.bounds.Right - SearchTexture.Width * 2f), SearchBoxArea.bounds.Y + 16, 0, 0);

            SearchBarButton.bounds.X = iconNewLocation.X;
            SearchBarButton.bounds.Y = iconNewLocation.Y;

            // --------------------------------------------------------------------------------------------

            DrawHelper.DrawMenuBox(SearchBoxArea.bounds.X, SearchBoxArea.bounds.Y,
                SearchBoxArea.bounds.Width,
                SearchBoxArea.bounds.Height - 8, out _);

            SearchBox.Draw(b);

            if (SearchBox.Text != "") {
                //b.Draw(Game1.mouseCursors, SearchBarButton.bounds, ClearTexture, Color.White);
                b.Draw(ModManager.UITextureInstance, SearchBarButton.bounds, UIConstants.SearchbarClearIcon, Color.White);
            }
            else {
                //b.Draw(SearchBarButton.texture, SearchBarButton.bounds, SearchBarButton.sourceRect, Color.White * IconOpacity);
                b.Draw(ModManager.UITextureInstance, SearchBarButton.bounds, UIConstants.SearchbarIcon, Color.White * IconOpacity);
            }
        }

        public void Update(GameTime time) {
            if (PersistFocus && !SearchBox.Selected) {
                Blur();
            }

            var delta = 1.5f / time.ElapsedGameTime.Milliseconds;

            if (!SearchBox.Selected && IconOpacity < 1f) {
                IconOpacity = Math.Min(1f, IconOpacity + delta);
            }
            else if (SearchBox.Selected && IconOpacity > 0f) {
                IconOpacity = Math.Max(0f, IconOpacity - delta);
            }
        }
    }
}