using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AstrumAureus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class AstralChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Chunk");
            Tooltip.SetDefault("Summons Astrum Aureus\n" +
				"Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Lime;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && player.Calamity().ZoneAstral && !NPC.AnyNPCs(ModContent.NPCType<AstrumAureus>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
			Main.PlaySound(SoundID.Roar, player.position, 0);
			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-250, 251)), (int)(player.position.Y - 500f), ModContent.NPCType<AstrumAureus>(), 1);
				Main.npc[npc].timeLeft *= 20;
				CalamityUtils.BossAwakenMessage(npc);
            }
			else
				NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<AstrumAureus>());

			return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 30);
            recipe.AddIngredient(ItemID.FallenStar, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
