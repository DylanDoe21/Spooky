using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class PetrifiedWoodWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(56, 48, 34));
            DustType = DustID.WoodFurniture;
			ItemDrop = ModContent.ItemType<PetrifiedWoodWallItem>();
        }
    }
}