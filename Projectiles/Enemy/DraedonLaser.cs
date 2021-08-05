using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class DraedonLaser : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        // The DrawBeam method relies on localAI[0] for its calculations. A different parameter won't work.
        public float TrailLength
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public const int MaxTrailPoints = 50;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Turret Laser");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.hostile = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 4;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                // play a sound frame 1. changed this from space gun sound because that sound was way too annoying
                var sound = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"), projectile.Center);
                if (sound != null)
                    sound.Volume *= 0.35f;

                projectile.localAI[0] = 1f;
            }
            projectile.alpha = (int)(Math.Sin(projectile.timeLeft / 240f * MathHelper.Pi) * 1.6f * 255f);
            if (projectile.alpha > 255)
                projectile.alpha = 255;
            TrailLength += 1.5f;
            if (TrailLength > MaxTrailPoints)
            {
                TrailLength = MaxTrailPoints;
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 190, 255, 0);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(MaxTrailPoints, 1.5f, lightColor);
    }
}
