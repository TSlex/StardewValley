﻿using ItemResearchSpawnerV2.Core.Data.Enums;

namespace ItemResearchSpawnerV2.Core.Data.Serializable {

    internal class ModManagerState {

        public ModMode ActiveMode => Config.GetDefaultMode();

        public ItemQuality Quality { get; set; } = ItemQuality.Normal;

        public string SearchText { get; set; } = "";

        public ItemSortOption SortOption { get; set; } = ItemSortOption.CategoryASC;

        public string Category { get; set; } = I18n.Category_All();

        public FavoriteDisplayMode FavoriteDisplayMode { get; set; } = FavoriteDisplayMode.All;

        public ProgressionDisplayMode ProgressionDisplayMode { get; set; } = ProgressionDisplayMode.ResearchedOnly;

        public ModConfig Config { get; set; } = new ModConfig();

        public int JMTMoney { get; set; } = 0;

        public override bool Equals(object obj) {
            if (obj is ModManagerState CModState) {

                return ActiveMode == CModState.ActiveMode &&
                       Quality == CModState.Quality &&
                       SortOption == CModState.SortOption &&
                       FavoriteDisplayMode == CModState.FavoriteDisplayMode &&
                       ProgressionDisplayMode == CModState.ProgressionDisplayMode &&
                       SearchText.Equals(CModState.SearchText) &&
                       Category.Equals(CModState.Category);
            }

            return false;
        }
    }
}
