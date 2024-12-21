using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class DampSoilWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(43, 20, 0));
            DustType = DustID.Dirt;
        }
    }

    public class DampSoilWallSafe : DampSoilWall 
    {
        public override string Texture => "Spooky/Content/Tiles/SpiderCave/DampSoilWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(43, 20, 0));
            DustType = DustID.Dirt;
        }
    }
}