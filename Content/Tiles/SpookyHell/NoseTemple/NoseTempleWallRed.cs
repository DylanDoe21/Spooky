using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyHell.NoseTemple
{
    public class NoseTempleWallRed : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(63, 39, 48));
            DustType = DustID.Stone;
        }
        
        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            //fail = true;
        }
    }

    public class NoseTempleWallRedSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyHell/NoseTemple/NoseTempleWallRed";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(63, 39, 48));
            DustType = DustID.Stone;
        }
    }
}