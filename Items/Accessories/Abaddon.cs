﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Face)]
    public class Abaddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Abaddon");
            Tooltip.SetDefault("Grants immunity to Brimstone Flames and Searing Lava");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().abaddon = true;
            player.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GoldCrown).
                AddIngredient<UnholyCore>(5).
                AddIngredient<EssenceofChaos>(10).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.PlatinumCrown).
                AddIngredient<UnholyCore>(5).
                AddIngredient<EssenceofChaos>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
