using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleWallRed : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(82, 19, 23));
            DustType = DustID.Stone;
        }
        
        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }
    }

    public class NoseTempleWallRedSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleWallRed";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(82, 19, 23));
            DustType = DustID.Stone;
        }
    }
}