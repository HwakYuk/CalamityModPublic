﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class Spikecrag : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikecrag");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 42;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = Projectile.damage;
                Projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != Projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)(Projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    Projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                Projectile.damage = damage2;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }

            Projectile.velocity.Y += 0.5f;
            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.StickToTiles(false, false);

            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
                return;
            }

            float maxDistance = 1000f;
            bool homeIn = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(Main.npc[i].width / 2) + (Main.npc[i].height / 2);

                    if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance) && Collision.CanHit(Projectile.Center, Projectile.width, Projectile.height, Main.npc[i].Center, Main.npc[i].width, Main.npc[i].height))
                    {
                        homeIn = true;
                        break;
                    }
                }
            }

            if (Projectile.owner == Main.myPlayer && homeIn)
            {
                Projectile.ai[1] += 1f;
                if ((Projectile.ai[1] % 10f) == 0f)
                {
                    int amount = Main.rand.Next(5, 8);
                    for (int i = 0; i < amount; i++)
                    {
                        float velocityX = Main.rand.NextFloat(-10f, 10f);
                        float velocityY = Main.rand.NextFloat(-15f, -8f);
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.oldPosition.X + (Projectile.width / 2), Projectile.oldPosition.Y + (Projectile.height / 2), velocityX, velocityY, ModContent.ProjectileType<SpikecragSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? CanDamage() => false;
    }
}
