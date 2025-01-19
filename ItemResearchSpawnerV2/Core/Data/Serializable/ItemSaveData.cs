using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace ItemResearchSpawnerV2.Models {
    internal class ItemSaveData {

        public int ResearchCount = 0;
        public int ResearchCountGold = 0;
        public int ResearchCountSilver = 0;
        public int ResearchCountIridium = 0;

        public bool Favorite = false;

        public Color ClothesColor = Color.White;
        public int WaterLevel = 0;

        public Dictionary<string, string> Meta = new();

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
                    Meta.Count == data.Meta.Count && !Meta.Except(data.Meta).Any() &&
                    ClothesColor == data.ClothesColor &&
                    WaterLevel == data.WaterLevel;
            }
        }

        public T TryGetMetaPropery<T>(string Key, T fallback) { 
            if (Meta.ContainsKey(Key)) {
                try {
                    T property = JsonConvert.DeserializeObject<T>(Meta[Key]);
                    return property;
                }
                catch {
                    Meta.Remove(Key);
                    return fallback;
                }
            }
            else {
                return fallback;
            }
        }
    }
}