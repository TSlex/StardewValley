using System;
using StardewValley;
using StardewValley.Enchantments;

namespace ItemResearchSpawnerV2.Core.Utils {
    public static class CommonHelper {
        public static bool EqualsCaseInsensitive(string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetItemUniqueKey(Item item) {
            return $"{item.Name}:" + $"{item.ParentSheetIndex}";
        }

        public static IEnumerable<string> GetClassFullNames(Item item) {
            for (Type type = item.GetType(); type != null; type = type.BaseType) {
                yield return type.FullName;
            }
        }

        public static void TryReturnItemToInventory(Item item) {
            if (item != null) {
                if (Game1.player.isInventoryFull()) {
                    DropItem(item);
                }
                else {
                    Game1.player.addItemByMenuIfNecessary(item);
                }
            }
        }

        public static void DropItem(Item item) {
            Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }

        public static List<BaseEnchantment> GetAllEnchantments() {
            return new List<BaseEnchantment>
            {
                new ArtfulEnchantment(),
                new BugKillerEnchantment(),
                new CrusaderEnchantment(),
                new HaymakerEnchantment(),
                new MagicEnchantment(),
                new VampiricEnchantment(),
                new AxeEnchantment(),
                new HoeEnchantment(),
                new MilkPailEnchantment(),
                new PanEnchantment(),
                new PickaxeEnchantment(),
                new ShearsEnchantment(),
                new WateringCanEnchantment(),
                new ArchaeologistEnchantment(),
                new AutoHookEnchantment(),
                new BottomlessEnchantment(),
                new EfficientToolEnchantment(),
                new GenerousEnchantment(),
                new MasterEnchantment(),
                new PowerfulEnchantment(),
                new PreservingEnchantment(),
                new ReachingToolEnchantment(),
                new ShavingEnchantment(),
                new SwiftToolEnchantment(),
                new FisherEnchantment(),
                new AmethystEnchantment(),
                new AquamarineEnchantment(),
                new DiamondEnchantment(),
                new EmeraldEnchantment(),
                new JadeEnchantment(),
                new RubyEnchantment(),
                new TopazEnchantment(),
                new AttackEnchantment(),
                new DefenseEnchantment(),
                new SlimeSlayerEnchantment(),
                new CritEnchantment(),
                new WeaponSpeedEnchantment(),
                new CritPowerEnchantment(),
                new LightweightEnchantment(),
                new SlimeGathererEnchantment(),
                new GalaxySoulEnchantment(),
            };
        }
    }
}