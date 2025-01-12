using Microsoft.Xna.Framework;
using SObject = StardewValley.Object;

namespace ItemResearchSpawnerV2.Core.Data {
    internal class MissingItem : SObject {
        private readonly string Key;

        public MissingItem(string missingKey) : 
            base(Vector2.Zero, "MissingItem", false) {

            Key = missingKey;
        }

        public override string getDescription() {
            return I18n.Item_MissingItemDesc();
        }

        public override string DisplayName => Key;

        protected override int getDescriptionWidth() {
            return base.getDescriptionWidth();
        }
    }
}
