using CalamityMod.Tiles.Furniture.BossTrophies;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.Trophies
{
    public class ThanatosTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Thanatos Trophy");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<ThanatosTrophyTile>();
        }
    }
}
