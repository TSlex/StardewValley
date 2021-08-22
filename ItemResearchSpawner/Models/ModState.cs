using ItemResearchSpawner.Models.Enums;

namespace ItemResearchSpawner.Models
{
    public class ModState
    {
        public ModMode ActiveMode { get; set; }

        public ItemQuality Quality { get; set; } = ItemQuality.Normal;

        public ItemSortOption SortOption { get; set; } = ItemSortOption.Category;

        public string SearchText { get; set; } = "";

        public string Category { get; set; } = I18n.Category_All();

        public override bool Equals(object obj)
        {
            if (obj is ModState CModState)
            {
                return ActiveMode == CModState.ActiveMode &&
                       Quality == CModState.Quality &&
                       SortOption == CModState.SortOption &&
                       SearchText.Equals(CModState.SearchText) &&
                       Category.Equals(CModState.Category);
            }

            return false;
        }
    }
}