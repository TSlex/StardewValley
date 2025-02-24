using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Extensions;
using Microsoft.Xna.Framework;
using System.Text;
using StardewValley.Menus;
using TimeSkipper.Core.UI;

namespace TimeSkipper.Core.Utils {
    internal static class DrawHelper {
        #region drawDialogueBox
        public static void drawDialogueBox(int x, int y, int width, int height, bool speaker, bool drawOnlyBox, string message = null, bool objectDialogueWithPortrait = false, bool ignoreTitleSafe = true, int r = -1, int g = -1, int b = -1) {
            if (!drawOnlyBox) {
                return;
            }

            Rectangle titleSafeArea = Game1.graphics.GraphicsDevice.Viewport.GetTitleSafeArea();
            int height2 = titleSafeArea.Height;
            int width2 = titleSafeArea.Width;
            int num = 0;
            int num2 = 0;
            if (!ignoreTitleSafe) {
                num2 = y <= titleSafeArea.Y ? titleSafeArea.Y - y : 0;
            }

            int num3 = 0;
            width = Math.Min(titleSafeArea.Width, width);
            if (!Game1.isQuestion && Game1.currentSpeaker == null && Game1.currentObjectDialogue.Count > 0 && !drawOnlyBox) {
                width = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).X + 128;
                height = (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y + 64;
                x = width2 / 2 - width / 2;
                num3 = height > 256 ? -(height - 256) : 0;
            }

            Rectangle value = new Rectangle(0, 0, 64, 64);
            int num4 = -1;
            if (Game1.questionChoices.Count >= 3) {
                num4 = Game1.questionChoices.Count - 3;
            }

            if (!drawOnlyBox && Game1.currentObjectDialogue.Count > 0) {
                if (Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y >= height - 128) {
                    num4 -= (int) ((height - 128 - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y) / 64f) - 1;
                }
                else {
                    height += (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
                    num3 -= (int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2;
                    if ((int) Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Peek()).Y / 2 > 64) {
                        num4 = 0;
                    }
                }
            }

            if (Game1.currentSpeaker != null && Game1.isQuestion && Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex)
                .Contains(Environment.NewLine)) {
                num4++;
            }

            value.Width = 64;
            value.Height = 64;
            value.X = 64;
            value.Y = 128;
            Color color = r == -1 ? Color.White : new Color(r, g, b);
            Texture2D texture = r == -1 ? Game1.menuTexture : Game1.uncoloredMenuTexture;
            Game1.spriteBatch.Draw(texture, new Rectangle(28 + x + num, 28 + y - 64 * num4 + num2 + num3, width - 64, height - 64 + num4 * 64), value, r == -1 ? color : new Color((int) Utility.Lerp(r, Math.Min(255, r + 150), 0.65f), (int) Utility.Lerp(g, Math.Min(255, g + 150), 0.65f), (int) Utility.Lerp(b, Math.Min(255, b + 150), 0.65f)));
            value.Y = 0;
            value.X = 0;
            Game1.spriteBatch.Draw(texture, new Vector2(x + num, y - 64 * num4 + num2 + num3), value, color);
            value.X = 192;
            Game1.spriteBatch.Draw(texture, new Vector2(x + width + num - 64, y - 64 * num4 + num2 + num3), value, color);
            value.Y = 192;
            Game1.spriteBatch.Draw(texture, new Vector2(x + width + num - 64, y + height + num2 - 64 + num3), value, color);
            value.X = 0;
            Game1.spriteBatch.Draw(texture, new Vector2(x + num, y + height + num2 - 64 + num3), value, color);
            value.X = 128;
            value.Y = 0;
            Game1.spriteBatch.Draw(texture, new Rectangle(64 + x + num, y - 64 * num4 + num2 + num3, width - 128, 64), value, color);
            value.Y = 192;
            Game1.spriteBatch.Draw(texture, new Rectangle(64 + x + num, y + height + num2 - 64 + num3, width - 128, 64), value, color);
            value.Y = 128;
            value.X = 0;
            Game1.spriteBatch.Draw(texture, new Rectangle(x + num, y - 64 * num4 + num2 + 64 + num3, 64, height - 128 + num4 * 64), value, color);
            value.X = 192;
            Game1.spriteBatch.Draw(texture, new Rectangle(x + width + num - 64, y - 64 * num4 + num2 + 64 + num3, 64, height - 128 + num4 * 64), value, color);
            if (objectDialogueWithPortrait && Game1.objectDialoguePortraitPerson != null || speaker && Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 && Game1.currentSpeaker.CurrentDialogue.Peek().showPortrait) {
                NPC nPC = objectDialogueWithPortrait ? Game1.objectDialoguePortraitPerson : Game1.currentSpeaker;
                Rectangle value2;
                switch (!objectDialogueWithPortrait ? nPC.CurrentDialogue.Peek().CurrentEmotion : Game1.objectDialoguePortraitPerson.Name == Game1.player.spouse ? "$l" : "$neutral") {
                    case "$h":
                        value2 = new Rectangle(64, 0, 64, 64);
                        break;
                    case "$s":
                        value2 = new Rectangle(0, 64, 64, 64);
                        break;
                    case "$u":
                        value2 = new Rectangle(64, 64, 64, 64);
                        break;
                    case "$l":
                        value2 = new Rectangle(0, 128, 64, 64);
                        break;
                    case "$a":
                        value2 = new Rectangle(64, 128, 64, 64);
                        break;
                    case "$k":
                    case "$neutral":
                        value2 = new Rectangle(0, 0, 64, 64);
                        break;
                    default:
                        value2 = Game1.getSourceRectForStandardTileSheet(nPC.Portrait, Convert.ToInt32(nPC.CurrentDialogue.Peek().CurrentEmotion.Substring(1)));
                        break;
                }

                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
                if (nPC.Portrait != null) {
                    Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(num + x + 768, height2 - 320 - 64 * num4 - 256 + num2 + 16 - 60 + num3), new Rectangle(333, 305, 80, 87), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.98f);
                    Game1.spriteBatch.Draw(nPC.Portrait, new Vector2(num + x + 768 + 32, height2 - 320 - 64 * num4 - 256 + num2 + 16 - 60 + num3), value2, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.99f);
                }

                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin();
                if (Game1.isQuestion) {
                    Game1.spriteBatch.DrawString(Game1.dialogueFont, nPC.displayName, new Vector2(928f - Game1.dialogueFont.MeasureString(nPC.displayName).X / 2f + num + x, height2 - 320 - 64 * num4 - Game1.dialogueFont.MeasureString(nPC.displayName).Y + num2 + 21f + num3) + new Vector2(2f, 2f), new Color(150, 150, 150));
                }
                Game1.spriteBatch.DrawString(Game1.dialogueFont, nPC.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : nPC.displayName, new Vector2(num + x + 896 + 32 - Game1.dialogueFont.MeasureString(nPC.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : nPC.displayName).X / 2f, height2 - 320 - 64 * num4 - Game1.dialogueFont.MeasureString(nPC.Name.Equals("Lewis") ? Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.3756") : nPC.displayName).Y + num2 + 21f + 8f + num3), Game1.textColor);
            }
            if (drawOnlyBox) {
                return;
            }

            string text = "";
            if (Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0) {
                if (Game1.currentSpeaker.CurrentDialogue.Peek() == null || Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Length < Game1.currentDialogueCharacterIndex - 1) {
                    Game1.dialogueUp = false;
                    Game1.currentDialogueCharacterIndex = 0;
                    Game1.playSound("dialogueCharacterClose");
                    Game1.player.forceCanMove();
                    return;
                }

                text = Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue().Substring(0, Game1.currentDialogueCharacterIndex);
            }
            else if (message != null) {
                text = message;
            }
            else if (Game1.currentObjectDialogue.Count > 0) {
                text = Game1.currentObjectDialogue.Peek().Length <= 1 ? "" : Game1.currentObjectDialogue.Peek().Substring(0, Game1.currentDialogueCharacterIndex);
            }

            Vector2 vector = Game1.dialogueFont.MeasureString(text).X > width2 - 256 - num ? new Vector2(128 + num, height2 - 64 * num4 - 256 - 16 + num2 + num3) : Game1.currentSpeaker != null && Game1.currentSpeaker.CurrentDialogue.Count > 0 ? new Vector2(width2 / 2 - Game1.dialogueFont.MeasureString(Game1.currentSpeaker.CurrentDialogue.Peek().getCurrentDialogue()).X / 2f + num, height2 - 64 * num4 - 256 - 16 + num2 + num3) : message != null ? new Vector2(width2 / 2 - Game1.dialogueFont.MeasureString(text).X / 2f + num, y + 96 + 4) : !Game1.isQuestion ? new Vector2(width2 / 2 - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + num, y + 4 + num3) : new Vector2(width2 / 2 - Game1.dialogueFont.MeasureString(Game1.currentObjectDialogue.Count == 0 ? "" : Game1.currentObjectDialogue.Peek()).X / 2f + num, height2 - 64 * num4 - 256 - (16 + (Game1.questionChoices.Count - 2) * 64) + num2 + num3);
            if (!drawOnlyBox) {
                Game1.spriteBatch.DrawString(Game1.dialogueFont, text, vector + new Vector2(3f, 0f), Game1.textShadowColor);
                Game1.spriteBatch.DrawString(Game1.dialogueFont, text, vector + new Vector2(3f, 3f), Game1.textShadowColor);
                Game1.spriteBatch.DrawString(Game1.dialogueFont, text, vector + new Vector2(0f, 3f), Game1.textShadowColor);
                Game1.spriteBatch.DrawString(Game1.dialogueFont, text, vector, Game1.textColor);
            }

            if (Game1.dialogueFont.MeasureString(text).Y <= 64f) {
                num2 += 64;
            }

            if (Game1.isQuestion && !Game1.dialogueTyping) {
                for (int i = 0; i < Game1.questionChoices.Count; i++) {
                    if (Game1.currentQuestionChoice == i) {
                        vector.X = 80 + num + x;
                        vector.Y = height2 - (5 + num4 + 1) * 64 + (text.Trim().Length > 0 ? Game1.dialogueFont.MeasureString(text).Y : 0f) + 128f + 48 * i - (16 + (Game1.questionChoices.Count - 2) * 64) + num2 + num3;
                        Game1.spriteBatch.End();
                        Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp);
                        Game1.spriteBatch.Draw(Game1.objectSpriteSheet, vector + new Vector2((float) Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) * 3f, 0f), GameLocation.getSourceRectForObject(26), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        Game1.spriteBatch.End();
                        Game1.spriteBatch.Begin();
                        vector.X = 160 + num + x;
                        vector.Y = height2 - (5 + num4 + 1) * 64 + (text.Trim().Length > 1 ? Game1.dialogueFont.MeasureString(text).Y : 0f) + 128f - (Game1.questionChoices.Count - 2) * 64 + 48 * i + num2 + num3;
                        Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[i].responseText, vector, Game1.textColor);
                    }
                    else {
                        vector.X = 128 + num + x;
                        vector.Y = height2 - (5 + num4 + 1) * 64 + (text.Trim().Length > 1 ? Game1.dialogueFont.MeasureString(text).Y : 0f) + 128f - (Game1.questionChoices.Count - 2) * 64 + 48 * i + num2 + num3;
                        Game1.spriteBatch.DrawString(Game1.dialogueFont, Game1.questionChoices[i].responseText, vector, Game1.unselectedOptionColor);
                    }
                }
            }

            if (!drawOnlyBox && !Game1.dialogueTyping && message == null) {
                Game1.spriteBatch.Draw(Game1.mouseCursors, new Vector2(x + num + width - 96, y + height + num2 + num3 - 96 - Game1.dialogueButtonScale), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, !Game1.dialogueButtonShrinking && Game1.dialogueButtonScale < 8f ? 3 : 2), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9999999f);
            }
        }

        #endregion

        #region V1
        public static string FillString(string value, string fillWith, SpriteFont font, int maxWidth, string overflowSign = "...") {
            var truncatedString = TruncateString(value, font, maxWidth, overflowSign);
            var stringWidth = font.MeasureString(truncatedString).X;

            var charWidth = font.MeasureString(fillWith).X;
            var charCount = (int) (maxWidth - stringWidth) / charWidth;

            var newString = new StringBuilder();

            for (var i = 0; i < charCount; i++) {
                newString.Append(fillWith);
            }

            newString.Append(truncatedString);

            return newString.ToString();
        }

        public static string TruncateString(string value, SpriteFont font, int maxWidth, string overflowSign = "...") {
            var newString = new StringBuilder();
            var width = 0f;

            foreach (var ch in value) {
                var charWidth = font.MeasureString(ch.ToString()).X;

                if (width + charWidth > maxWidth) {
                    newString.Append(overflowSign);
                    break;
                }

                newString.Append(ch);
                width += charWidth;
            }

            return newString.ToString();
        }

        public static int GetLabelWidth(SpriteFont font) {
            return (int) font.MeasureString("THISISLABELWIDTHYEAH").X;
        }

        public static int GetChildCenterPosition(int pos, int parentLenght, int childLenght) {
            return (int) (pos + parentLenght / 2f - childLenght / 2f);
        }

        public static void DrawItemBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition) {
            var spriteBatch = Game1.spriteBatch;

            IClickableMenu.drawTextureBox(
                spriteBatch,
                Game1.menuTexture,
                new Rectangle(128, 128, 64, 64),
                x,
                y,
                innerWidth,
                innerHeight,
                Color.White,
                1,
                false);

            innerDrawPosition = new Vector2(x, y);
        }

        public static void DrawTextMenuBox(int x, int y, SpriteFont font, string text, int offsetX = 0, int offsetY = 0, int paddingX = 0, int paddingY = 0) {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            var additionalBounds = new Vector2(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0);

            DrawMenuBox(
                x - paddingX,
                y - paddingY,
                (int) ((int) bounds.X + additionalBounds.X + paddingX * 2),
                (int) ((int) bounds.Y + additionalBounds.Y + paddingY * 2),
                out var textPosition);

            Utility.drawTextWithShadow(spriteBatch, text, font,
                new Vector2(textPosition.X + offsetX + paddingX, textPosition.Y + offsetY + paddingY), Game1.textColor);
        }

        public static void DrawTextMenuBox(int x, int y, int width, SpriteFont font, string text, int offsetX = 0, int offsetY = 0, bool bypassDraw = false) {
            var spriteBatch = Game1.spriteBatch;
            var bounds = font.MeasureString(text);

            var additionalBounds = new Vector2(offsetX > 0 ? offsetX : 0, offsetY > 0 ? offsetY : 0);

            DrawMenuBox(x, y, width, (int) ((int) bounds.Y + additionalBounds.Y),
                out var textPosition, bypassDraw);

            Utility.drawTextWithShadow(spriteBatch, text, font,
                new Vector2(textPosition.X + offsetX, textPosition.Y + offsetY), Game1.textColor);
        }

        public static void DrawMenuBox(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, bool bypassDraw = false) {
            var b = Game1.spriteBatch;

            if (!bypassDraw) {

                DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.BarBase,
                    new Rectangle(x + 8, y + 8, innerWidth, innerHeight), cornerSize: 8, colorize: false);

                DrawTileableTexture(b, ModManager.UITextureInstance, UIConstants.BarFrame,
                    new Rectangle(x, y, innerWidth + 16, innerHeight + 16), cornerSize: 16, colorize: true);

            }

            innerDrawPosition = new Vector2(x + 8, y + 8);
        }

        public static void DrawTileableTexture(SpriteBatch b, Texture2D texture, Rectangle sourceRect, Rectangle destRect, int cornerSize = 8, bool colorize = false, Color? color = null) {

            var sX = sourceRect.X;
            var sY = sourceRect.Y;
            var sW = sourceRect.Width;
            var sH = sourceRect.Height;

            var dX = destRect.X;
            var dY = destRect.Y;
            var dW = destRect.Width > cornerSize * 2 ? destRect.Width : cornerSize * 2 + 1;
            var dH = destRect.Height > cornerSize * 2 ? destRect.Height : cornerSize * 2 + 1;

            //var color = colorize ? ModManager.Instance.ModMode.GetColor() : Color.White;
            color ??= Color.White;

            // scaled base

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + cornerSize, dY + cornerSize, dW - cornerSize * 2, dH - cornerSize * 2),
                new Rectangle(sX + cornerSize, sY + cornerSize, sW - cornerSize * 2, sH - cornerSize * 2),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            // corners

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX, dY, cornerSize, cornerSize),
                new Rectangle(sX, sY, cornerSize, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX, dY + dH - cornerSize, cornerSize, cornerSize),
                new Rectangle(sX, sY + sH - cornerSize, cornerSize, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + dW - cornerSize, dY, cornerSize, cornerSize),
                new Rectangle(sX + sW - cornerSize, sY, cornerSize, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + dW - cornerSize, dY + dH - cornerSize, cornerSize, cornerSize),
                new Rectangle(sX + sW - cornerSize, sY + sH - cornerSize, cornerSize, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            // scaled borders

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + cornerSize, dY, dW - cornerSize * 2, cornerSize),
                new Rectangle(sX + cornerSize, sY, sW - cornerSize * 2, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + cornerSize, dY + dH - cornerSize, dW - cornerSize * 2, cornerSize),
                new Rectangle(sX + cornerSize, sY + sH - cornerSize, sW - cornerSize * 2, cornerSize),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX, dY + cornerSize, cornerSize, dH - cornerSize * 2),
                new Rectangle(sX, sY + cornerSize, cornerSize, sH - cornerSize * 2),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            b.Draw(ModManager.UITextureInstance,
                new Rectangle(dX + dW - cornerSize, dY + cornerSize, cornerSize, dH - cornerSize * 2),
                new Rectangle(sX + sW - cornerSize, sY + cornerSize, cornerSize, sH - cornerSize * 2),
                (Color) color, 0f, Vector2.Zero, SpriteEffects.None, 1f);

        }

        #endregion

        public static void DrawTab(int x, int y, SpriteFont font, string text, int align = 0, float alpha = 1, bool drawShadow = true) {
            SpriteBatch spriteBatch = Game1.spriteBatch;
            Vector2 bounds = font.MeasureString(text);

            DrawTab(x, y, (int) bounds.X, (int) bounds.Y, out Vector2 drawPos, align, alpha, drawShadow: drawShadow);
            Utility.drawTextWithShadow(spriteBatch, text, font, drawPos, Game1.textColor);
        }


        public static void DrawTab(int x, int y, int innerWidth, int innerHeight, out Vector2 innerDrawPosition, int align = 0, float alpha = 1, bool drawShadow = true) {
            SpriteBatch spriteBatch = Game1.spriteBatch;

            // calculate outer coordinates
            int outerWidth = innerWidth + UIConstants.BorderWidth * 2;
            int outerHeight = innerHeight + Game1.tileSize / 3;
            int offsetX = align switch {
                1 => -outerWidth / 2,
                2 => -outerWidth,
                _ => 0
            };

            // draw texture
            IClickableMenu.drawTextureBox(spriteBatch, Game1.uncoloredMenuTexture, new Rectangle(0, 256, 60, 60), x + offsetX, y, outerWidth, outerHeight + Game1.tileSize / 16, Color.White * alpha, drawShadow: drawShadow);
            innerDrawPosition = new Vector2(x + UIConstants.BorderWidth + offsetX, y + UIConstants.BorderWidth);
        }

        public static Rectangle GetRectangleFromAnchor((int X, int Y) anchor, (int X, int Y) anchorType, int width, int height) {

            int x = anchorType.X switch {
                0 => anchor.X,
                1 => anchor.X - width / 2,
                2 => anchor.X - width,
                _ => 0
            };

            int y = anchorType.Y switch {
                0 => anchor.Y,
                1 => anchor.Y - height / 2,
                2 => anchor.Y - height,
                _ => 0
            };

            return new Rectangle(x, y, width, height);
        }
    }
}
