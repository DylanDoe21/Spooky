using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookyStoneWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(42, 34, 39));
            DustType = DustID.Stone;
        }
    }

    public class SpookyStoneWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/SpookyStoneWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(42, 34, 39));
            DustType = DustID.Stone;
        }
    }
}