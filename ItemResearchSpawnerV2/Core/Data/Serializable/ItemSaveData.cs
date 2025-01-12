using Microsoft.Xna.Framework;

namespace ItemResearchSpawnerV2.Models {
    internal class ItemSaveData {

        public int ResearchCount = 0;
        public int ResearchCountGold = 0;
        public int ResearchCountSilver = 0;
        public int ResearchCountIridium = 0;

        public bool Favorite = false;

        public Color ClothesColor = Color.White;
        public int WaterLevel = 0;

        public override bool Equals(object obj) {
            if (obj == null || obj is not ItemSaveData data) {
                return false;
            }
            else {
                return ResearchCount == data.ResearchCount &&
                    ResearchCountGold == data.ResearchCountGold &&
                    ResearchCountSilver == data.ResearchCountSilver &&
                    ResearchCountIridium == data.ResearchCountIridium &&
                    Favorite == data.Favorite &&
                    ClothesColor == data.ClothesColor &&
                    WaterLevel == data.WaterLevel;
            }
        }
    }
}