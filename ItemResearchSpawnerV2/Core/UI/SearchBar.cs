using System;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

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
        private readonly ClickableTextureComponent SearchIcon;

        private float IconOpacity;
        private bool PersistFocus;

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        private readonly Rectangle IconTexture = new(80, 0, 13, 13);

        public bool Selected => SearchBox.Selected;
        public bool IsSearchBoxSelectionChanging => IconOpacity > 0 && IconOpacity < 1;

        public Rectangle Bounds => SearchBoxArea.bounds;

        public SearchBar(Func<int> getXPos, Func<int> getYPos, int width) {

            GetXPos = getXPos;
            GetYPos = getYPos;

            ModManager.Instance.SearchText = "";
            IconOpacity = 1f;

            SearchBoxArea = new ClickableComponent(
                    new Rectangle(getXPos(), getYPos(), width + UIConstants.BorderWidth, 36 + UIConstants.BorderWidth - 2), "");

            SearchBox = new TextBox(Game1.content.Load<Texture2D>("LooseSprites\\textBox"), null, Game1.smallFont,
                Game1.textColor) {
                X = SearchBoxArea.bounds.X,
                Y = SearchBoxArea.bounds.Y + 2,
                Height = 0,
                Width = SearchBoxArea.bounds.Width,
                Text = "_SAMLE SEARCH TEXT_"
            };

            var iconLocation = new Rectangle((int)(SearchBoxArea.bounds.Right - IconTexture.Width * 2f),
                (int)(SearchBoxArea.bounds.Y + 16),
                (int)(IconTexture.Width * 2f), (int)(IconTexture.Height * 2f)
            );

            SearchIcon = new ClickableTextureComponent(iconLocation, Game1.mouseCursors, IconTexture, 2f);
        }

        public void Focus(bool persist) {
            SearchBox.Selected = true;
            PersistFocus = persist;
        }

        public void Blur() {
            SearchBox.Selected = false;
            PersistFocus = false;
        }



        public void Draw(SpriteBatch b) {

            SearchBoxArea.bounds.X = GetXPos();
            SearchBoxArea.bounds.Y = GetYPos();

            SearchBox.X = SearchBoxArea.bounds.X;
            SearchBox.Y = SearchBoxArea.bounds.Y;

            var iconNewLocation = new Rectangle((int)(SearchBoxArea.bounds.Right - IconTexture.Width * 2f), SearchBoxArea.bounds.Y + 16, 0, 0);

            SearchIcon.bounds.X = iconNewLocation.X;
            SearchIcon.bounds.Y = iconNewLocation.Y;

            // --------------------------------------------------------------------------------------------

            DrawHelper.DrawMenuBox(SearchBoxArea.bounds.X, SearchBoxArea.bounds.Y,
                SearchBoxArea.bounds.Width - UIConstants.BorderWidth,
                SearchBoxArea.bounds.Height - UIConstants.BorderWidth - 8, out _);

            SearchBox.Draw(b);
            b.Draw(SearchIcon.texture, SearchIcon.bounds, SearchIcon.sourceRect, Color.White * IconOpacity);
        }

        public void Update(GameTime time) {
            if (PersistFocus && !SearchBox.Selected) {
                Blur();
            }

            if (ModManager.Instance.SearchText != SearchBox.Text.Trim()) {
                ModManager.Instance.SearchText = SearchBox.Text.Trim();
            }

            var delta = 1.5f / time.ElapsedGameTime.Milliseconds;

            if (!SearchBox.Selected && IconOpacity < 1f) {
                IconOpacity = Math.Min(1f, IconOpacity + delta);
            }
            else if (SearchBox.Selected && IconOpacity > 0f) {
                IconOpacity = Math.Max(0f, IconOpacity - delta);
            }
        }

        public void Clear() {
            SearchBox.Text = "";
        }

        public void SetText(string text) {
            SearchBox.Text = text;
        }
    }
}