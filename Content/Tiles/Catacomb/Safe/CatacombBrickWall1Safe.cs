using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Catacomb.Safe
{
    public class CatacombBrickWall1Safe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(29, 24, 35));
            DustType = DustID.Stone;
        }
    }
}