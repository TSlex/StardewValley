using Microsoft.Xna.Framework;

namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum MenuSize {
        Normal,
        Larger,
        XLarge,
    }

    internal static class MenuSizeExtensions {
        public static int GetNumberRows(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 4,
                MenuSize.Larger => 6,
                MenuSize.XLarge => 8,
                _ => 4,
            };
        }

        public static int GetNumberCols(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 4,
                MenuSize.Larger => 5,
                MenuSize.XLarge => 6,
                _ => 4,
            };
        }

        public static int GetVerticalGap(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 8,
                MenuSize.Larger => 4,
                MenuSize.XLarge => 4,
                _ => 8,
            };
        }

        public static int GetRootX(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 38,
                MenuSize.Larger => 38,
                MenuSize.XLarge => 52,
                _ => 38,
            };
        }

        public static int GetRootY(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 12,
                MenuSize.Larger => 2,
                MenuSize.XLarge => 2,
                _ => 2,
            };
        }

        public static float GetItemScale(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 1f,
                MenuSize.Larger => 0.75f,
                MenuSize.XLarge => 0.5f,
                _ => 1f,
            };
        }

        public static float GetPageOffset(this MenuSize current) {
            return current switch {
                MenuSize.Normal => 84,
                MenuSize.Larger => 67,
                MenuSize.XLarge => 56.5f,
                _ => 84,
            };
        }

        public static Vector2 GetFavoriteOffcet(this MenuSize current) {
            return current switch {
                MenuSize.Normal => new Vector2(4 * 12, 4 * -1),
                MenuSize.Larger => new Vector2(4 * 9f, 4f * -1f),
                MenuSize.XLarge => new Vector2(4 * 6f, 4 * -1f),
                _ => new Vector2(4 * 12, 4 * -1),
            };
        }

        public static Vector2 GetItemImageOffcet(this MenuSize current) {
            return current switch {
                MenuSize.Normal => new Vector2(4 * 0, 4 * 0),
                MenuSize.Larger => new Vector2(4f * -2f, 4f * -2f),
                MenuSize.XLarge => new Vector2(4 * -4, 4 * -4),
                _ => new Vector2(4 * 0, 4 * 0),
            };
        }

        public static string GetString(this MenuSize current) {
            return GetTranstationFunc(current)();
        }

        public static Func<string> GetTranstationFunc(this MenuSize current) {
            return current switch {
                MenuSize.Normal => I18n.MenuSize_Normal,
                MenuSize.Larger => I18n.MenuSize_Large,
                MenuSize.XLarge => I18n.MenuSize_Xlarge,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
