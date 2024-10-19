using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.NoseTemple
{  
    public class NoseTempleFancyWallGray : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(87, 87, 87));
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

    public class NoseTempleFancyWallGraySafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleFancyWallGray";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(87, 87, 87));
            DustType = DustID.Stone;
        }
    }
}