using CalamityMod.Items.Placeables.Walls;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Plates
{
    public class Navyplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navyplate");
            Tooltip.SetDefault("It resonates with otherworldly energy.");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Plates.Navyplate>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = 3;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 1);
            recipe.AddIngredient(ItemID.Obsidian, 3);
            recipe.SetResult(this, 3);
            recipe.AddTile(TileID.Hellforge);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<NavyplateWall>(), 4);
            recipe.SetResult(this);
            recipe.AddTile(TileID.WorkBenches);
            recipe.AddRecipe();
        }
    }
}
