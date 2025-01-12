namespace ItemResearchSpawnerV2.Core.Data.Enums {
    internal enum FavoriteDisplayMode {
        All,
        FavoriteOnly
    }

    internal static class FavoriteDisplayModeExtensions {
        public static FavoriteDisplayMode GetNext(this FavoriteDisplayMode current) {
            return current switch {
                FavoriteDisplayMode.All => FavoriteDisplayMode.FavoriteOnly,
                FavoriteDisplayMode.FavoriteOnly => FavoriteDisplayMode.All,
                _ => throw new NotSupportedException($"Unknown favorite mode: '{current}'")
            };
        }

        public static FavoriteDisplayMode GetPrevious(this FavoriteDisplayMode current) {
            return current switch {
                FavoriteDisplayMode.All => FavoriteDisplayMode.FavoriteOnly,
                FavoriteDisplayMode.FavoriteOnly => FavoriteDisplayMode.All,
                _ => throw new NotSupportedException($"Unknown favorite mode: '{current}'")
            };
        }
    }
}
