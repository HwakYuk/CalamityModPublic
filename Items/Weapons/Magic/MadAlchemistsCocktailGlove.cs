﻿using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class MadAlchemistsCocktailGlove : ModItem
    {
        private int flaskIndex = 0;

        private static int[] flaskIDs;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Glove");
            Tooltip.SetDefault("Fires a variety of high-velocity flasks\n" +
                "Right click to throw a prismatic flask that inflicts many debuffs\n" +
                "Red flasks explode violently, blue flasks contain poison gas,\n" +
                "green flasks summon lunar flares and purple flasks explode into homing shrapnel");

            flaskIDs = new int[]
            {
                ModContent.ProjectileType<MadAlchemistsCocktailRed>(),
                ModContent.ProjectileType<MadAlchemistsCocktailBlue>(),
                ModContent.ProjectileType<MadAlchemistsCocktailGreen>(),
                ModContent.ProjectileType<MadAlchemistsCocktailPurple>(),
                ModContent.ProjectileType<MadAlchemistsCocktailAlt>()
            };
            SacrificeTotal = 1;
        }

        // Rest in peace Mad Cock, you will not be missed.
        // Ozzatron 09FEB2021
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;

            Item.damage = 182;
            Item.DamageType = DamageClass.Magic;
            Item.noUseGraphic = true;
            Item.mana = 12;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;

            Item.UseSound = SoundID.Item106;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MadAlchemistsCocktailRed>();
            Item.shootSpeed = 19f;

            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (player.altFunctionUse == 2)
            {
                type = flaskIDs[4];
                return;
            }

            // Cycle through the flask types in a circle.
            type = flaskIDs[flaskIndex++];
            if (flaskIndex > 3)
                flaskIndex = 0;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ToxicFlask).
                AddIngredient(ItemID.BottledWater, 15).
                AddIngredient(ItemID.Leather, 5).
                AddIngredient<EffulgentFeather>(5).
                AddIngredient<CoreofCalamity>(2).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
