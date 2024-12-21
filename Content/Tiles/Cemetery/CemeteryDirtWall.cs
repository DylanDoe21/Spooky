using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Cemetery
{
    public class CemeteryDirtWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(36, 34, 6));
            DustType = DustID.Dirt;
        }
    }

    public class CemeteryDirtWallSafe : CemeteryDirtWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Cemetery/CemeteryDirtWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(36, 34, 6));
            DustType = DustID.Dirt;
        }
    }
}