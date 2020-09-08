using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Perforator
{
	[AutoloadBossHead]
    public class PerforatorHeadMedium : ModNPC
    {
        private bool flies = false;
        private int minLength = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 5 : 10;
        private int maxLength = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 6 : 11;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator");
        }

        public override void SetDefaults()
        {
			npc.GetNPCDamage();
			npc.npcSlots = 5f;
            npc.width = 58;
            npc.height = 68;
            npc.defense = 2;
			npc.LifeMaxNERB(2000, 2200, 700000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = 6;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.alpha = 255;
            npc.buffImmune[ModContent.BuffType<TimeSlow>()] = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.netAlways = true;
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			float speed = 14f;
			float turnSpeed = 0.12f;

			if (expertMode)
			{
				speed += death ? 9f : 9f * (1f - lifeRatio);
				turnSpeed += death ? 0.08f : 0.08f * (1f - lifeRatio);
			}

			if (npc.Calamity().enraged > 0 || (CalamityConfig.Instance.BossRushXerocCurse && BossRushEvent.BossRushActive))
			{
				speed *= 1.25f;
				turnSpeed *= 1.25f;
			}

			if (BossRushEvent.BossRushActive)
			{
				speed *= 1.25f;
				turnSpeed *= 1.25f;
			}

			if (npc.ai[3] > 0f)
            {
                npc.realLife = (int)npc.ai[3];
            }

			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
			{
				npc.TargetClosest(true);
			}

			Player player = Main.player[npc.target];

            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

            if (!TailSpawned)
            {
                int Previous = npc.whoAmI;
                for (int num36 = 0; num36 < maxLength; num36++)
                {
                    int lol;
                    if (num36 >= 0 && num36 < minLength)
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<PerforatorBodyMedium>(), npc.whoAmI);
                    }
                    else
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<PerforatorTailMedium>(), npc.whoAmI);
                    }
                    Main.npc[lol].realLife = npc.whoAmI;
                    Main.npc[lol].ai[2] = (float)npc.whoAmI;
                    Main.npc[lol].ai[1] = (float)Previous;
                    Main.npc[Previous].ai[0] = (float)lol;
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                    Previous = lol;
                }
                TailSpawned = true;
            }
            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num180 < 0)
            {
                num180 = 0;
            }
            if (num181 > Main.maxTilesX)
            {
                num181 = Main.maxTilesX;
            }
            if (num182 < 0)
            {
                num182 = 0;
            }
            if (num183 > Main.maxTilesY)
            {
                num183 = Main.maxTilesY;
            }
            bool flag94 = flies;
            if (!flag94)
            {
                for (int num952 = num180; num952 < num181; num952++)
                {
                    for (int num953 = num182; num953 < num183; num953++)
                    {
                        if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
                        {
                            Vector2 vector105;
                            vector105.X = (float)(num952 * 16);
                            vector105.Y = (float)(num953 * 16);
                            if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
                            {
                                flag94 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag94)
            {
                npc.localAI[1] = 1f;
                Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int num954 = 150;
                bool flag95 = true;
                if (npc.position.Y > player.position.Y)
                {
                    for (int num955 = 0; num955 < 255; num955++)
                    {
                        if (Main.player[num955].active)
                        {
                            Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - num954, (int)Main.player[num955].position.Y - num954, num954 * 2, num954 * 2);
                            if (rectangle12.Intersects(rectangle13))
                            {
                                flag95 = false;
                                break;
                            }
                        }
                    }
                    if (flag95)
                    {
                        flag94 = true;
                    }
                }
            }
            else
            {
                npc.localAI[1] = 0f;
            }
            if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
            {
				npc.TargetClosest(false);
				flag94 = false;
                npc.velocity.Y = npc.velocity.Y + 0.05f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.05f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = player.position.X + (float)(player.width / 2);
            float num192 = player.position.Y + (float)(player.height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (!flag94)
            {
                npc.TargetClosest(true);
                npc.velocity.Y = npc.velocity.Y + (turnSpeed * 0.7f);
                if (npc.velocity.Y > num188)
                {
                    npc.velocity.Y = num188;
                }
                if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.4)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                    }
                }
                else if (npc.velocity.Y == num188)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 0.9f;
                    }
                }
            }
            else
            {
                if (!flies && npc.behindTiles && npc.soundDelay == 0)
                {
                    float num195 = num193 / 40f;
                    if (num195 < 10f)
                    {
                        num195 = 10f;
                    }
                    if (num195 > 20f)
                    {
                        num195 = 20f;
                    }
                    npc.soundDelay = (int)num195;
                    Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 1);
                }
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                bool flag21 = false;
                if (!flag21)
                {
                    if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189;
                            }
                        }
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                        if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                            }
                        }
                        if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 2f;
                            }
                        }
                    }
                    else
                    {
                        if (num196 > num197)
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + num189;
                                }
                                else
                                {
                                    npc.velocity.Y = npc.velocity.Y - num189;
                                }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X = npc.velocity.X + num189;
                                }
                                else
                                {
                                    npc.velocity.X = npc.velocity.X - num189;
                                }
                            }
                        }
                    }
                }
                npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
                if (flag94)
                {
                    if (npc.localAI[0] != 1f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 0f;
                }
                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                {
                    npc.netUpdate = true;
                }
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture2D15 = Main.npcTexture[npc.type];
			Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

			Vector2 vector43 = npc.Center - Main.screenPosition;
			vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
			vector43 += vector11 * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			texture2D15 = ModContent.GetTexture("CalamityMod/NPCs/Perforator/PerforatorHeadMediumGlow");
			Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

			spriteBatch.Draw(texture2D15, vector43, npc.frame, color37, npc.rotation, vector11, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/MediumPerf2"), 1f);
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "The Medium Perforator";
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
			DropHelper.DropItem(npc, ModContent.ItemType<BloodSample>(), 3, 7);
			DropHelper.DropItem(npc, ItemID.CrimtaneBar, 2, 4);
			DropHelper.DropItem(npc, ItemID.Vertebrae, 2, 3);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 240, true);
            player.AddBuff(BuffID.Bleeding, 240, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 180, true);
            }
        }
    }
}
