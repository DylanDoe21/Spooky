using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookyDirtWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(82, 49, 26));
            DustType = DustID.Dirt;
        }
    }

    public class SpookyDirtWallSafe : SpookyDirtWall 
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/SpookyDirtWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(82, 49, 26));
            DustType = DustID.Dirt;
        }
    }
}