using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockYellowGreenWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(81, 85, 7));
            DustType = 288;
        }
    }
}