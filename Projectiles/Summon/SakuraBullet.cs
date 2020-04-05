﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SakuraBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sakura Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 150;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
			Player player = Main.player[projectile.owner];
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;

            float num472 = projectile.Center.X;
            float num473 = projectile.Center.Y;
            float num474 = 400f;
            bool flag17 = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if ((npc.CanBeChasedBy(projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float num476 = npc.position.X + (float)(npc.width / 2);
                    float num477 = npc.position.Y + (float)(npc.height / 2);
                    float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
                    if (num478 < num474)
                    {
                        num474 = num478;
                        num472 = num476;
                        num473 = num477;
                        flag17 = true;
                    }
                }
            }
			else
			{
				for (int num475 = 0; num475 < Main.maxNPCs; num475++)
				{
					NPC npc = Main.npc[num475];
					if ((npc.CanBeChasedBy(projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
					{
						float num476 = npc.position.X + (float)(npc.width / 2);
						float num477 = npc.position.Y + (float)(npc.height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < num474)
						{
							num474 = num478;
							num472 = num476;
							num473 = num477;
							flag17 = true;
						}
					}
				}
			}

            if (flag17)
            {
                float num483 = 11f;
                Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num484 = num472 - vector35.X;
                float num485 = num473 - vector35.Y;
                float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
                num486 = num483 / num486;
                num484 *= num486;
                num485 *= num486;
                projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
            }

            int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 0.6f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += projectile.velocity * 0.1f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (target.type == NPCID.DukeFishron)
                damage = (int)(damage * 2.0);
            else if (target.type == NPCID.CultistBoss)
                damage = (int)(damage * 0.75);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 25);
            int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 1f);
            Main.dust[num622].velocity *= 0.5f;
            if (Main.rand.NextBool(2))
            {
                Main.dust[num622].scale = 0.5f;
                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 1.4f);
            Main.dust[num624].noGravity = true;
            num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 0.8f);
        }
    }
}
