using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class RootWoodWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(133, 101, 56));
            DustType = DustID.Web;
        }
    }
}