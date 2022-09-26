﻿using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls.DraedonStructures
{
    public class LaboratoryPlatePillar : ModWall
    {

        public override void SetStaticDefaults()
        {
            DustType = 109;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.LaboratoryPlatePillar>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(29, 28, 30));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
