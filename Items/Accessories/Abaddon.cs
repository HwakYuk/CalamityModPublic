using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Abaddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abaddon");
            Tooltip.SetDefault("Reduces the damage caused by the Brimstone Flames debuff");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.Calamity().abaddon = true;
        }
    }
}
