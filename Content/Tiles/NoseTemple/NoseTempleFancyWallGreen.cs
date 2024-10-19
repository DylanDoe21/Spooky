using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleFancyWallGreen : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(80, 87, 69));
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

    public class NoseTempleFancyWallGreenSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleFancyWallGreen";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(80, 87, 69));
            DustType = DustID.Stone;
        }
    }
}