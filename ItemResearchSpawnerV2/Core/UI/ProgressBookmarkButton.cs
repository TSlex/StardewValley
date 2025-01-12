using ItemResearchSpawnerV2.Core.Data.Enums;
using ItemResearchSpawnerV2.Core.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace ItemResearchSpawnerV2.Core.UI {
    internal class ProgressBookmarkButton : BookmarkButtonBase {
        public ProgressBookmarkButton(Func<int> getXPos, Func<int> getYPos) : 
            base(getXPos, getYPos, UIConstants.ProgressButton.Width - 1 * 4, UIConstants.ProgressButton.Height - 2 * 4) {
            //InactiveXOffcet -= 4 * 2;
        }

        public override void HandleLeftClick(int x, int y) {
            ModManager.Instance.ProgressionDisplay = ModManager.Instance.ProgressionDisplay.GetNext();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }

        public override void HandleRightClick(int x, int y) {
            ModManager.Instance.ProgressionDisplay = ModManager.Instance.ProgressionDisplay.GetPrevious();
            if (ModManager.Instance.Config.GetEnableSounds()) {
                Game1.playSound("drumkit6");
            }
        }

        public override void Draw(SpriteBatch b) {
            Inactive = ModManager.Instance.ProgressionDisplay == ProgressionDisplayMode.ResearchedOnly;

            base.Draw(b);

            var baseRect = new Rectangle(
                UIConstants.ProgressButton.X, 
                UIConstants.ProgressButton.Y, 
                UIConstants.ProgressButton.Width + XOffcet, 
                UIConstants.ProgressButton.Height);

            b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X - XOffcet - 4 * 1, Component.bounds.Y + 4 * -1),
                baseRect, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);

            if (!Inactive) {
                //var c = ModManager.Instance.ProgressionDisplay == ProgressionDisplayMode.ResearchStarted ? Color.Gray * 0.7f : Color.White;
                var c = ModManager.Instance.ProgressionDisplay switch {
                    ProgressionDisplayMode.ResearchedOnly => Color.White,
                    ProgressionDisplayMode.ResearchStarted => Color.Gray * 0.7f,
                    ProgressionDisplayMode.Combined => Color.White,
                    ProgressionDisplayMode.NotResearched => Color.Red * 0.7f,
                    _ => Color.White,
                };

                b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 6 - XOffcet, Component.bounds.Y + 4 * 3),
                    UIConstants.ProgressButtonIconBase, c, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
                b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 9 - XOffcet, Component.bounds.Y + 4 * 4),
                    UIConstants.ProgressButtonIconFill, c, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            }

            //if (ModManager.Instance.ProgressionDisplay == ProgressionDisplayMode.ResearchStarted) {
            //    b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 6 - XOffcet, Component.bounds.Y + 4 * 3),
            //        UIConstants.FavoriteButtonIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            //}
            //else if (ModManager.Instance.ProgressionDisplay == ProgressionDisplayMode.Combined) {
            //    b.Draw(ModManager.UITextureInstance, new Vector2(Component.bounds.X + 4 * 6 - XOffcet, Component.bounds.Y + 4 * 3),
            //        UIConstants.FavoriteButtonIcon, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
            //}
        }
    }
}
