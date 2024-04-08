using ItemResearchSpawnerV2.Core.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class ItemResearchArea {
        private readonly ClickableComponent ResearchArea;
        public readonly ResearchButton ResearchButton;
        private readonly Texture2D ResearchTexture;
        private readonly Texture2D SellTexture;
        private readonly Texture2D CombinedTexture;

        private readonly Texture2D BookAnimations;
        private readonly int BookTextureSize = 20;
        private int BookSpriteID = 0;
        private int TimeCounter = 0;
        private int AnimWait = 150;
        private bool BookOpenAnimComplete = false;

        public bool BookTurnLeftRequested = false;
        private bool BookTurnLeftPending = false;
        private bool BookTurnLeftPendingC = false;

        public bool BookTurnRightRequested = false;
        private bool BookTurnRightPending = false;
        private bool BookTurnRightPendingC = false;

        private int ResearchProcessCounter = 0;

        public Item ResearchItem;
        //private Item LastItem;

        //private string ItemProgression;

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        private readonly int Width;

        private static IModContentHelper Content => ModManager.Instance.Helper.ModContent;
        public Rectangle Bounds => ResearchArea.bounds;
        public Rectangle ButtonBounds => ResearchButton.Bounds;

        private Rectangle GetButtonPosition => new Rectangle(
                    DrawHelper.GetChildCenterPosition(GetXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
                    ResearchArea.bounds.Height + 4 * 10 + GetYPos(), ResearchTexture.Width, ResearchTexture.Height);

        // ===================================================================================================

        public ItemResearchArea(Func<int> getXPos, Func<int> getYPos, int width) {

            GetXPos = getXPos;
            GetYPos = getYPos;
            Width = width;

            ResearchTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "search-button"));
            SellTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "sell-button.png"));
            CombinedTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "combined-button.png"));

            ResearchArea = new ClickableComponent(new Rectangle(getXPos(), getYPos(), width, Game1.tileSize + 50), "");

            ResearchButton = new ResearchButton(this, () => GetButtonPosition.X + 3, () => GetButtonPosition.Y);

            //BookAnimations = ModManager.Instance.Helper.GameContent.Load<Texture2D>("LooseSprites\\Book_Animation");
            BookAnimations = ModManager.Instance.Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "images", "Book_Animation"));

            //ResearchButton = new ClickableTextureComponent(
            //    new Rectangle(
            //        DrawHelper.GetChildCenterPosition(getXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
            //        ResearchArea.bounds.Height + 38 + getYPos(), ResearchTexture.Width, ResearchTexture.Height),
            //    ResearchTexture, new Rectangle(0, 0, ResearchTexture.Width, ResearchTexture.Height), 1f);

            //ProgressionManager.OnStackChanged += OnStackChanged;
        }

        // ===================================================================================================

        public void SetItem(Item item, out Item returnItem) {
            returnItem = ResearchItem;
            ResearchItem = item;
        }

        public Item ReturnItem() {
            SetItem(null, out var returnItem);
            return returnItem;
        }

        // ---------------------------------------------------------------------------------------------------

        public void Update(GameTime time) {

            TimeCounter += time.ElapsedGameTime.Milliseconds;

            if (ResearchProcessCounter < 1000) {
                ResearchProcessCounter += time.ElapsedGameTime.Milliseconds;
            }

            if (TimeCounter < AnimWait) {
                return;
            }

            if (!BookOpenAnimComplete) {
                if (BookSpriteID < 1) {
                    AnimWait = 100;
                }
                else if (BookSpriteID > 8) {
                    AnimWait = 30;
                }
                else if (BookSpriteID > 14) {
                    AnimWait = 50;
                }
                else {
                    AnimWait = 15;
                }
                BookSpriteID++;

                if (BookSpriteID > 20) {
                    BookSpriteID--;
                    BookOpenAnimComplete = true;
                }

                if (BookSpriteID % 3 == 0) {
                    Game1.playSound("newRecipe");
                }
            }
            else if (ResearchItem != null) {
                BookSpriteID = 8;
                BookTurnRightPending = false;
                BookTurnLeftPending = false;
                BookTurnLeftRequested = false;
                BookTurnRightRequested = false;
            }
            else {
                if (BookTurnLeftRequested) {
                    BookTurnRightPending = false;
                    BookTurnRightPendingC = false;

                    if (!BookTurnLeftPending) {
                        AnimWait = 10;
                        BookSpriteID = 14;
                        BookTurnLeftPendingC = false;
                    }
                    else {
                        BookTurnLeftPendingC = true;
                    }

                    BookTurnLeftRequested = false;
                    BookTurnLeftPending = true;
                }
                else if (BookTurnRightRequested) {
                    BookTurnLeftPending = false;
                    BookTurnLeftPendingC = false;

                    if (!BookTurnRightPending) {
                        AnimWait = 10;
                        BookSpriteID = 8;
                        BookTurnRightPendingC = false;
                    }
                    else {
                        BookTurnRightPendingC = true;
                    }

                    BookTurnRightRequested = false;
                    BookTurnRightPending = true;
                }
                if (BookTurnRightPending) {
                    AnimWait = 10;
                    BookSpriteID++;

                    if (BookSpriteID > 14) {
                        if (!BookTurnRightPendingC) {
                            BookSpriteID--;
                            BookTurnRightPending = false;
                        }
                        else {
                            BookTurnRightPendingC = false;
                            BookSpriteID = 8;
                        }
                    }
                }
                else if (BookTurnLeftPending) {
                    AnimWait = 10;
                    BookSpriteID--;

                    if (BookSpriteID < 8) {
                        if (!BookTurnLeftPendingC) {
                            BookSpriteID++;
                            BookTurnLeftPending = false;
                        }
                        else {
                            BookTurnLeftPendingC = false;
                            BookSpriteID = 14;
                        }
                    }
                }
            }





            TimeCounter = 0;
            //BookSpriteID++;
            //BookSpriteID = BookSpriteID > 13 ? 8 : BookSpriteID;
        }

        public void Draw(SpriteBatch b) {

            ResearchArea.bounds.X = GetXPos();
            ResearchArea.bounds.Y = GetYPos();

            //var buttonNewLocation = new Rectangle(
            //        DrawHelper.GetChildCenterPosition(GetXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
            //        ResearchArea.bounds.Height + 38 + GetYPos(), ResearchTexture.Width, ResearchTexture.Height);

            //ResearchButton.bounds.X = buttonNewLocation.X;
            //ResearchButton.bounds.Y = buttonNewLocation.Y;

            // ------------------------------------------------------------------------------------------------------

            var areaInnerAnchors = new Vector2(ResearchArea.bounds.X + UIConstants.BorderWidth, ResearchArea.bounds.Y + UIConstants.BorderWidth);

            //DrawHelper.DrawMenuBox(ResearchArea.bounds.X, ResearchArea.bounds.Y,
            //    ResearchArea.bounds.Width, ResearchArea.bounds.Height, out var areaInnerAnchors);

            b.Draw(BookAnimations,
                new Vector2(ResearchArea.bounds.X + 4 * 6 - 1, ResearchArea.bounds.Y - 4 * 6),
                new Rectangle(BookTextureSize * BookSpriteID, 0, BookTextureSize, BookTextureSize), Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 1f);

            var researchItemCellX = areaInnerAnchors.X + ResearchArea.bounds.Width / 2f - Game1.tileSize / 2f;

            //DrawHelper.DrawItemBox((int)researchItemCellX, (int)areaInnerAnchors.Y + 10, Game1.tileSize,
            //    Game1.tileSize,
            //    out _);

            var researchProgressString = GetItemProgression();

            var progressFont = Game1.smallFont;
            var progressPositionX = areaInnerAnchors.X + ResearchArea.bounds.Width / 2f -
                                    progressFont.MeasureString(researchProgressString).X / 2f;

            //b.DrawString(progressFont, researchProgressString,
            //    new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);

            if (ResearchItem != null) {
                Utility.drawTextWithColoredShadow(b, "1 предмет остался!", Game1.smallFont,
                    new Vector2(ResearchArea.bounds.X, ResearchArea.bounds.Y + 124),
                    Color.Cyan, Color.Red * (false ? 1f : 0.25f), 0.9f);

            }

            //var buttonTexture = ModManager.Instance.ModMode switch {
            //    ModMode.BuySell => SellTexture,
            //    ModMode.Combined => CombinedTexture,
            //    _ => ResearchTexture
            //};

            //spriteBatch.Draw(buttonTexture, ResearchButton.bounds, ResearchButton.sourceRect, Color.White);

            ResearchButton.Draw(b);

            ResearchItem?.drawInMenu(b, new Vector2(researchItemCellX, areaInnerAnchors.Y - 10), 1f);
        }

        public void HandleResearch() {
            if (ResearchItem != null) {
                //if (ModManager.Instance.ModMode == ModMode.Combined) {
                //    ModManager.Instance.SellItem(_researchItem);
                //}

                //if (ModManager.Instance.ModMode == ModMode.BuySell) {
                //    ModManager.Instance.SellItem(_researchItem);
                //}

                //ProgressionManager.Instance.ResearchItem(_researchItem);
                Game1.playSound("reward");
            }
        }

        private string GetItemProgression() {
            return "(0 / 0)";

            //if (_researchItem == null)
            //{
            //    return "(0 / 0)";
            //}

            //if (_lastItem == null || !_lastItem.Equals(_researchItem))
            //{
            //    _itemProgression = ProgressionManager.Instance.GetItemProgression(_researchItem, true);
            //    _lastItem = _researchItem;
            //}

            //return _itemProgression;
        }

        //private void OnStackChanged(int newCount) {
        //    LastItem = null;

        //    if (newCount <= 0) {
        //        ResearchItem = null;
        //    }
        //    else if (ResearchItem != null) {
        //        ResearchItem.Stack = newCount % ResearchItem.maximumStackSize();
        //    }
        //}
    }
}