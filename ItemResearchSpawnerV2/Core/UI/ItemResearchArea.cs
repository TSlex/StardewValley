using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using ItemResearchSpawnerV2.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class ItemResearchArea {
        private readonly ClickableComponent ResearchArea;
        public readonly ResearchButton ResearchButton;
        //private readonly Texture2D ResearchTexture;
        //private readonly Texture2D SellTexture;
        //private readonly Texture2D CombinedTexture;
        //private readonly Texture2D ResearchItemLightTexture;

        //private readonly Texture2D BookAnimations;
        //private readonly int BookTextureSize = 20;

        private int BookAnimOpenLoopId = 0;
        private int BookSpriteID = 0;
        private int TimeCounter = 0;
        private int AnimWait = 60;
        private bool BookOpenAnimComplete = false;
        private bool BookOpenAnimActive => !BookOpenAnimComplete || BookTurnLeftPending || BookTurnLeftPendingC || BookTurnRightPending || BookTurnRightPendingC;

        public bool BookTurnLeftRequested = false;
        private bool BookTurnLeftPending = false;
        private bool BookTurnLeftPendingC = false;

        public bool BookTurnRightRequested = false;
        private bool BookTurnRightPending = false;
        private bool BookTurnRightPendingC = false;

        private double ResearchProcessTime = 0;
        private bool ResearchStarted = false;

        private float SpinAnimationCounter = 0f;
        private float AppearAnimationCounter = 0f;
        private float PingPongAnimationCounter = 0f;
        private int PingPongAnimationPointer = 1;

        public ProgressionItem ResearchItem;
        //private Item LastItem;

        //private string ItemProgression;

        private readonly Func<int> GetXPos;
        private readonly Func<int> GetYPos;

        private readonly int Width;

        private static IModContentHelper Content => ModManager.Instance.Helper.ModContent;
        public Rectangle Bounds => ResearchArea.bounds;
        public Rectangle ButtonBounds => ResearchButton.Bounds;

        private Rectangle GetButtonPosition => new Rectangle(
                    DrawHelper.GetChildCenterPosition(GetXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, 4 * 17),
                    ResearchArea.bounds.Height + 4 * 10 + 2 + GetYPos(), 4 * 13, 4 * 14);

        // ===================================================================================================

        public ItemResearchArea(Func<int> getXPos, Func<int> getYPos, int width) {

            GetXPos = getXPos;
            GetYPos = getYPos;
            Width = width;

            //ResearchTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "search-button"));
            //SellTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "sell-button.png"));
            //CombinedTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "combined-button.png"));

            ResearchArea = new ClickableComponent(new Rectangle(getXPos(), getYPos(), width, Game1.tileSize + 50), "");

            ResearchButton = new ResearchButton(this, () => GetButtonPosition.X, () => GetButtonPosition.Y);

            //BookAnimations = ModManager.Instance.Helper.GameContent.Load<Texture2D>("LooseSprites\\Book_Animation");
            //BookAnimations = ModManager.Instance.Helper.ModContent.Load<Texture2D>(Path.Combine("assets", "images", "Book_Animation"));
            //ResearchItemLightTexture = Content.Load<Texture2D>(Path.Combine("assets", "images", "pinpointLight"));

            //ResearchButton = new ClickableTextureComponent(
            //    new Rectangle(
            //        DrawHelper.GetChildCenterPosition(getXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
            //        ResearchArea.bounds.Height + 38 + getYPos(), ResearchTexture.Width, ResearchTexture.Height),
            //    ResearchTexture, new Rectangle(0, 0, ResearchTexture.Width, ResearchTexture.Height), 1f);

            //ProgressionManager.OnStackChanged += OnStackChanged;
        }

        // ===================================================================================================

        public void SetItem(Item item, out Item returnItem) {
            if (item != null) {
                PingPongAnimationCounter = 0;
                PingPongAnimationPointer = 1;
            }

            returnItem = ResearchItem?.GameItem ?? null;
            ResearchItem = item != null ? ModManager.ProgressionManagerInstance.GetProgressionItem(item) : null;

            if (returnItem != null && ResearchItem?.GameItem != null && CommonHelper.GetItemUniqueKey(returnItem) == CommonHelper.GetItemUniqueKey(ResearchItem.GameItem)) {
                var resIStack = ResearchItem.Stack;
                var retIStack = returnItem.Stack;
                var maxStack = returnItem.maximumStackSize();

                var moveAmount = resIStack + retIStack;
                moveAmount = moveAmount > maxStack ? maxStack : moveAmount;
                moveAmount -= resIStack;

                if (moveAmount > 0) {
                    ResearchItem.GameItem.Stack += moveAmount;

                    if (retIStack == moveAmount) {
                        returnItem = null;
                    }
                    else {
                        returnItem.Stack -= moveAmount;
                    }
                }
            }
        }

        public Item ReturnItem() {
            SetItem(null, out var returnItem);
            return returnItem;
        }

        // ---------------------------------------------------------------------------------------------------

        public void Update(GameTime time) {

            if (ResearchStarted && time.TotalGameTime.TotalSeconds >= ResearchProcessTime) {
                OnResearchCompleted();
            }
            else if (ResearchStarted && !ModManager.Instance.Helper.Input.IsDown(SButton.MouseLeft)) {
                OnResearchInterrupted();
            }


            // ------------------------------------------------------------------

            var spinSpeed = ResearchStarted ? -4f : 1f;
            SpinAnimationCounter += time.ElapsedGameTime.Milliseconds / 1000f * spinSpeed;
            SpinAnimationCounter = SpinAnimationCounter >= 6.28319f ? 0 : SpinAnimationCounter;
            SpinAnimationCounter = SpinAnimationCounter < 0 ? 6.28318f : SpinAnimationCounter;

            var appearSpeed = (!BookOpenAnimComplete || BookOpenAnimActive) ? -100f : 1f * 3;

            AppearAnimationCounter += time.ElapsedGameTime.Milliseconds / 1000f * appearSpeed;
            AppearAnimationCounter = AppearAnimationCounter > 1 ? 1 : AppearAnimationCounter;
            AppearAnimationCounter = AppearAnimationCounter < 0 ? 0 : AppearAnimationCounter;

            PingPongAnimationCounter += time.ElapsedGameTime.Milliseconds / 1000f * PingPongAnimationPointer * 1f;
            PingPongAnimationCounter = PingPongAnimationCounter > 1 ? 1 : PingPongAnimationCounter;
            PingPongAnimationCounter = PingPongAnimationCounter < 0 ? 0 : PingPongAnimationCounter;
            PingPongAnimationPointer = PingPongAnimationCounter >= 1 ? -1 : (PingPongAnimationCounter <= 0 ? 1 : PingPongAnimationPointer);
            //ModManager.Instance.Monitor.Log($"{PingPongAnimationCounter}");

            // ------------------------------------------------------------------

            TimeCounter += time.ElapsedGameTime.Milliseconds;

            if (TimeCounter < AnimWait) {
                return;
            }

            if (!BookOpenAnimComplete) {

                if (BookSpriteID < 4) {
                    AnimWait = 40;
                }
                else if (BookSpriteID > 4 && BookAnimOpenLoopId == 0) {
                    AnimWait = 25;
                }
                else if (BookSpriteID > 10 && BookAnimOpenLoopId == 0) {
                    AnimWait = 20;
                }
                else {
                    AnimWait = 15;
                }
                BookSpriteID++;

                if (BookSpriteID > 10) {
                    BookAnimOpenLoopId++;

                    if (BookAnimOpenLoopId > 1) {
                        BookSpriteID--;
                        BookOpenAnimComplete = true;
                    }

                    BookSpriteID = 4;
                }

                if (BookSpriteID % 3 == 0) {
                    if (ModManager.Instance.Config.GetEnableSounds()) {
                        Game1.playSound("newRecipe");
                    }
                }
            }
            else if (ResearchItem != null) {
                BookSpriteID = 4;
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
                        BookSpriteID = 10;
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
                        BookSpriteID = 4;
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

                    if (BookSpriteID > 10) {
                        if (!BookTurnRightPendingC) {
                            BookSpriteID--;
                            BookTurnRightPending = false;
                        }
                        else {
                            BookTurnRightPendingC = false;
                            BookSpriteID = 4;
                        }
                    }
                }
                else if (BookTurnLeftPending) {
                    AnimWait = 10;
                    BookSpriteID--;

                    if (BookSpriteID < 4) {
                        if (!BookTurnLeftPendingC) {
                            BookSpriteID++;
                            BookTurnLeftPending = false;
                        }
                        else {
                            BookTurnLeftPendingC = false;
                            BookSpriteID = 10;
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

            if (ModManager.Instance.ModMode == ModMode.Research || ModManager.Instance.ModMode == ModMode.ResearchPlus) {
                ResearchArea.bounds.Y -= 48;
                ResearchButton.BaseYOff = -48;
            }

            //var buttonNewLocation = new Rectangle(
            //        DrawHelper.GetChildCenterPosition(GetXPos(), ResearchArea.bounds.Width + 2 * UIConstants.BorderWidth, ResearchTexture.Width),
            //        ResearchArea.bounds.Height + 38 + GetYPos(), ResearchTexture.Width, ResearchTexture.Height);

            //ResearchButton.bounds.X = buttonNewLocation.X;
            //ResearchButton.bounds.Y = buttonNewLocation.Y;

            // ------------------------------------------------------------------------------------------------------

            var areaInnerAnchors = new Vector2(ResearchArea.bounds.X + UIConstants.BorderWidth, ResearchArea.bounds.Y + UIConstants.BorderWidth);

            //DrawHelper.DrawMenuBox(ResearchArea.bounds.X, ResearchArea.bounds.Y,
            //    ResearchArea.bounds.Width, ResearchArea.bounds.Height, out var areaInnerAnchors);

            //b.Draw(BookAnimations,
            //    new Vector2(ResearchArea.bounds.X + 4 * 6 - 1, ResearchArea.bounds.Y - 4 * 6),
            //    new Rectangle(BookTextureSize * BookSpriteID, 0, BookTextureSize, BookTextureSize), Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 1f);

            var bookAnimBase = UIConstants.BookAnimBase;
            var bookAnimFrame = UIConstants.BookAnimFrame;

            b.Draw(ModManager.UITextureInstance,
                new Vector2(ResearchArea.bounds.X + 4 * (2), ResearchArea.bounds.Y - 4 * 9),
                new Rectangle(bookAnimBase.X + 192 * BookSpriteID, bookAnimBase.Y, bookAnimBase.Width, bookAnimBase.Height),
                Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Vector2(ResearchArea.bounds.X + 4 * (2), ResearchArea.bounds.Y - 4 * 5),
                new Rectangle(bookAnimFrame.X + 192 * BookSpriteID, bookAnimFrame.Y, bookAnimFrame.Width, bookAnimFrame.Height),
                ModManager.Instance.ModMode.GetColor(), 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

            //b.Draw(ModManager.UITextureInstance,
            //    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
            //    UIConstants.RNSPentagramEffect,
            //    ModManager.Instance.ModMode.GetColor() * 0.6f * AppearAnimationCounter, 
            //    SpinAnimationCounter, new Vector2(4 * 14 + 1.5f, 4 * 14 + 1.5f), 1f, SpriteEffects.None, 1f);

            //b.Draw(ModManager.UITextureInstance,
            //    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
            //    UIConstants.RNSPentagramEffect,
            //    ModManager.Instance.ModMode.GetColor() * (0.2f * PingPongAnimationCounter + 0.3f) * AppearAnimationCounter,
            //    SpinAnimationCounter, new Vector2(4 * 14 + 2f, 4 * 14 + 2f), 1f, SpriteEffects.None, 1f);

            if (ResearchItem != null && !ResearchItem.Forbidden) {
                b.Draw(ModManager.UITextureInstance,
                    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                    UIConstants.RNSPentagramEffect2,
                    ModManager.Instance.ModMode.GetColor() * (0.5f * PingPongAnimationCounter + 0.3f) * AppearAnimationCounter,
                    SpinAnimationCounter, new Vector2(4 * 16 + 2f, 4 * 16 + 2f), 1.2f, SpriteEffects.None, 1f);
            }
            else if (ResearchItem != null && ResearchItem.Forbidden) {
                b.Draw(ModManager.UITextureInstance,
                    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                    UIConstants.RNSPentagramEffect2,
                    Color.Black * (1f * PingPongAnimationCounter) * AppearAnimationCounter * 0.8f,
                    SpinAnimationCounter, new Vector2(4 * 16 + 2f, 4 * 16 + 2f), 1.2f, SpriteEffects.None, 1f);
                b.Draw(ModManager.UITextureInstance,
                    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                    UIConstants.RNSPentagramEffect2,
                    Color.Red * (1f - (1f * PingPongAnimationCounter)) * AppearAnimationCounter * 0.8f,
                    SpinAnimationCounter, new Vector2(4 * 16 + 2f, 4 * 16 + 2f), 1.2f, SpriteEffects.None, 1f);
            }
            else {
                b.Draw(ModManager.UITextureInstance,
                    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                    UIConstants.RNSPentagramEffect,
                    ModManager.Instance.ModMode.GetColor() * (0.2f * PingPongAnimationCounter + 0.3f) * AppearAnimationCounter,
                    SpinAnimationCounter, new Vector2(4 * 14 + 2f, 4 * 14 + 2f), 1f, SpriteEffects.None, 1f);
            }


            b.Draw(ModManager.UITextureInstance,
                new Vector2(ResearchArea.bounds.X + 4 * 12, ResearchArea.bounds.Y - 4 * 4),
                UIConstants.RNSOutlineEffect,
                ModManager.Instance.ModMode.GetColor() * 0.9f * AppearAnimationCounter,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);



            var researchItemCellX = areaInnerAnchors.X + ResearchArea.bounds.Width / 2f - Game1.tileSize / 2f;

            //DrawHelper.DrawItemBox((int)researchItemCellX, (int)areaInnerAnchors.Y + 10, Game1.tileSize,
            //    Game1.tileSize,
            //    out _);

            //b.DrawString(progressFont, researchProgressString,
            //    new Vector2(progressPositionX, areaInnerAnchors.Y + Game1.tileSize + 10), Color.Black);

            if (ResearchItem != null) {
                var researchProgressString = GetItemProgression();
                var progressFont = Game1.smallFont;
                var progressPositionX = researchItemCellX + 42 -
                                        progressFont.MeasureString(researchProgressString).X / 2f;

                if (ResearchItem.CannotResearch) {
                    Utility.drawTextWithColoredShadow(b, researchProgressString, progressFont,
                        new Vector2(progressPositionX, ResearchArea.bounds.Y + 124),
                        Color.Red, Color.Black * (false ? 1f : 0.25f), 0.9f);
                }
                else {
                    Utility.drawTextWithColoredShadow(b, researchProgressString, progressFont,
                        new Vector2(progressPositionX, ResearchArea.bounds.Y + 124),
                        Color.Cyan, Color.Red * (false ? 1f : 0.25f), 0.9f);
                }


            }

            //var buttonTexture = ModManager.Instance.ModMode switch {
            //    ModMode.BuySell => SellTexture,
            //    ModMode.Combined => CombinedTexture,
            //    _ => ResearchTexture
            //};

            //spriteBatch.Draw(buttonTexture, ResearchButton.bounds, ResearchButton.sourceRect, Color.White);

            ResearchButton.Draw(b, shake: ResearchStarted);



            if (ResearchStarted && ResearchItem != null) {
                float deltatime = ((float) (ResearchProcessTime - Game1.currentGameTime.TotalGameTime.TotalSeconds)) / 2f;

                for (int i = 0; i < 5; i++) {
                    //b.Draw(ResearchItemLightTexture,
                    //    new Rectangle((int) researchItemCellX, (int) (areaInnerAnchors.Y - 10), 64, 64),
                    //    ResearchItemLightTexture.Bounds, Color.White * (1f - deltatime));
                }

                //b.Draw(ModManager.UITextureInstance,
                //    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                //    UIConstants.RNSSplashEffect,
                //    ModManager.Instance.ModMode.GetColor() * (1f - EasingFunctions.InOutQuad(MathF.Sqrt(deltatime)) + 0.2f), 
                //    MathF.PI / 2,
                //    new Vector2(UIConstants.RNSSplashEffect.Width / 2, UIConstants.RNSSplashEffect.Height / 2),
                //    1f, SpriteEffects.None, 1f);

                b.Draw(ModManager.UITextureInstance,
                    new Vector2(ResearchArea.bounds.X + 4 * 26 + 2, ResearchArea.bounds.Y + 4 * 10 + 2),
                    UIConstants.RNSSplashEffect,
                    Color.White * (1f - EasingFunctions.InOutQuad(MathF.Sqrt(deltatime))), MathF.PI / 2,
                    new Vector2(UIConstants.RNSSplashEffect.Width / 2, UIConstants.RNSSplashEffect.Height / 2),
                    1f, SpriteEffects.None, 1f);

                // 1f * (1f - deltatime) / 2 + 0.5f
                // * (1.2f - EasingFunctions.OutQuad(MathF.Sqrt(deltatime))) / 2 + 0.5f

                var shakeOff = 4f * new Vector2(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2)) * (EasingFunctions.InOutQuad(MathF.Sqrt(deltatime)));

                ResearchItem?.GameItem?.drawInMenu(b, new Vector2(researchItemCellX + shakeOff.X, areaInnerAnchors.Y - 10 + shakeOff.Y), 1f,
                    1f, 0.9f, StackDrawType.Draw, Color.White * (EasingFunctions.InOutQuad(MathF.Sqrt(deltatime))), drawShadow: true);

                //var itemData = ItemRegistry.GetDataOrErrorItem(ResearchItem.QualifiedItemId);
                //var location = new Vector2(researchItemCellX, areaInnerAnchors.Y - 10);
                //var scale = 1f;

                //b.Draw(itemData.GetTexture(), location, new Rectangle?(itemData.GetSourceRect()),
                //    Color.White, 0.0f, Vector2.Zero, 4f * scale, SpriteEffects.None, 0.9f);

                //// this drawn quality icons and stack
                //ResearchItem.DrawMenuIcons(b, location, 1f, 1f, 0.9f, StackDrawType.HideButShowQuality, Color.White);
            }
            else {
                ResearchItem?.GameItem?.drawInMenu(b, new Vector2(researchItemCellX, areaInnerAnchors.Y - 10), 1f);
            }

        }

        public void HandleResearch() {
            if (ResearchItem != null) {

                if (ResearchItem.CannotResearch) {
                    OnResearchImpossible();
                    return;
                }

                ResearchProcessTime = Game1.currentGameTime.TotalGameTime.TotalSeconds + ModManager.Instance.Config.GetResearchTimeSeconds();
                ResearchStarted = true;
                //if (ModManager.Instance.ModMode == ModMode.Combined) {
                //    ModManager.Instance.SellItem(_researchItem);
                //}

                //if (ModManager.Instance.ModMode == ModMode.BuySell) {
                //    ModManager.Instance.SellItem(_researchItem);
                //}

                //ProgressionManager.Instance.ResearchItem(_researchItem);
            }
        }

        public void OnResearchInterrupted() {
            ResearchStarted = false;

            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("fireball");
            }
        }

        public void OnResearchImpossible() {
            ResearchStarted = false;
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("grunt");
            }
        }

        public void OnResearchCompleted() {
            var itemAlreadyResearched = ResearchItem.ResearchCompleted;

            ModManager.ProgressionManagerInstance.ResearchItem(ResearchItem, out var leftAmount);

            ResearchStarted = false;

            var pI = ModManager.ProgressionManagerInstance.GetProgressionItem(ResearchItem.Item);

            var itemUnlocked = !itemAlreadyResearched && pI.ResearchCompleted;

            if (itemUnlocked) {
                ModManager.Instance.FavoriteDisplay = FavoriteDisplayMode.All;
                ModManager.Instance.ItemQuality = ItemQuality.Normal;
                ModManager.Instance.ProgressionDisplay = ProgressionDisplayMode.ResearchedOnly;
                ModManager.Instance.UpdateMenu(clearFiltering: true);
                //ModManager.Instance.SearchText = "";
                //ModManager.Instance.SelectedCategory = I18n.Category_All();
                //ModManager.Instance.SortOption = I18n.Sort_ByCategoryAsc();

                ModManager.Instance.RecentlyUnlockedItem = pI;

                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("stardrop");
                    //Game1.playSound("getNewSpecialItem");
                    //Game1.playSound("newRecord");
                    //Game1.playSound("secret1");
                }
            }
            else {
                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("reward");
                }
            }

            //BookTurnLeftRequested = true;
            ModManager.Instance.UpdateMenu(rebuild: true);

            if (itemAlreadyResearched && ModManager.Instance.ModMode.HasPriceBehaviour() && leftAmount > 0) {
                var itemToSell = ResearchItem.GameItem;
                itemToSell.Stack = leftAmount;

                ModManager.Instance.SellItem(itemToSell);
                leftAmount = 0;

                if (ModManager.Instance.Config.GetEnableSounds()) {
                    Game1.playSound("purchase");
                }
            }

            if (leftAmount > 0) {
                ResearchItem.Stack = leftAmount;
                CommonHelper.TryReturnItemToInventory(ResearchItem.GameItem);
            }

            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("newRecipe");
            }

            ResearchItem = null;
        }

        private string GetItemProgression() {

            if (ResearchItem == null) {
                return "";
            }

            var left = ResearchItem.ResearchLeftAmount;

            if (ResearchItem.CannotResearch) {
                return I18n.Ui_ResearchImpossible();
            }

            if (left > 1) {
                return string.Format(I18n.Ui_ResearchMoreLeft(), left);
            }
            else if (left == 1) {
                return I18n.Ui_ResearchOneLeft();
            }
            else {
                return I18n.Ui_ResearchCompleted();
            }
        }
    }
}