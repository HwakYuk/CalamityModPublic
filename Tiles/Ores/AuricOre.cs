﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Tiles.Ores
{
    public class AuricOre : ModTile
    {
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AuricMine", 3);
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 1000;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            DustType = 55;
            ItemDrop = ModContent.ItemType<Items.Placeables.Ores.AuricOre>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Auric Ore");
            AddMapEntry(new Color(255, 200, 0), name);
            MineResist = 10f;
            MinPick = 250;
            HitSound = MineSound;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.20f;
            g = 0.16f;
            b = 0.00f;
        }
    }
}
