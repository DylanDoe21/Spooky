using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasBrickWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(70, 5, 5));
            DustType = -1;
        }
    }

    public class ChristmasBrickWallAlt : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(27, 33, 81));
            DustType = -1;
        }
    }
}