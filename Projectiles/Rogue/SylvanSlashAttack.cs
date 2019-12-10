using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class SylvanSlashAttack : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sylvan Slash");
            Main.projFrames[projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            projectile.width = 68;
            projectile.height = 68;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            float num = 0f;
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            if (projectile.spriteDirection == -1)
            {
                num = 3.14159274f;
            }
            if (++projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.soundDelay--;
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item15, projectile.Center);
                projectile.soundDelay = 24;
            }
            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float scaleFactor6 = 1f;
                    if (player.inventory[player.selectedItem].shoot == projectile.type)
                    {
                        scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    }
                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                    {
                        vector13 = Vector2.UnitX * (float)player.direction;
                    }
                    vector13 *= scaleFactor6;
                    if (vector13.X != projectile.velocity.X || vector13.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = vector13;
                }
                else
                {
                    projectile.Kill();
                }
            }
            Vector2 vector14 = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(vector14, 3f, 0.2f, 0.2f);
            if (Main.rand.NextBool(3))
            {
                int num30 = Dust.NewDust(vector14 - projectile.Size / 2f, projectile.width, projectile.height, 111, projectile.velocity.X, projectile.velocity.Y, 100, default, 2f);
                Main.dust[num30].noGravity = true;
                Main.dust[num30].position -= projectile.velocity;
            }
            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
            projectile.rotation = projectile.velocity.ToRotation() + num;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2((double)(projectile.velocity.Y * (float)projectile.direction), (double)(projectile.velocity.X * (float)projectile.direction));
        }

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
			if (projectile.owner == Main.myPlayer)
			{
				if ((target.damage > 5 || target.boss) && !target.SpawnedFromStatue)
				{
					if (modPlayer.wearingRogueArmor && modPlayer.rogueStealthMax != 0)
					{
						if (modPlayer.rogueStealth < modPlayer.rogueStealthMax)
							modPlayer.rogueStealth += 0.01f;
					}
				}
				if (Main.rand.NextBool(8))
				{
					if (Main.rand.NextBool(3))
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, Main.rand.Next(-3,-1), ModContent.ProjectileType<SylvanSlash>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					else if (Main.rand.NextBool(2))
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(2,4), 0f, ModContent.ProjectileType<SylvanSlash>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					else
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3,-1), 0f, ModContent.ProjectileType<SylvanSlash>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
				}
			}
		}
    }
}
