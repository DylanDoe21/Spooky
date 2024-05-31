using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleWallPurple : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(46, 0, 82));
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

    public class NoseTempleWallPurpleSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleWallPurple";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(46, 0, 82));
            DustType = DustID.Stone;
        }
    }
}