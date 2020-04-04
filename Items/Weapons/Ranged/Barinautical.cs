using CalamityMod.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Barinautical : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barinautical");
            Tooltip.SetDefault("Shoots a string of electric bolt arrows that explode");
        }

        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.width = 30;
            item.height = 42;
            item.useTime = 3;
            item.reuseDelay = 15;
            item.useAnimation = 9;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 15f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BoltArrow>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
