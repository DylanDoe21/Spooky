using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockRedWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(92, 24, 13));
            DustType = 288;
        }
    }
}