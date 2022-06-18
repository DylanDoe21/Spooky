using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookyWoodWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallBlend[Type] = ModContent.WallType<SpookyStoneWall>();
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(56, 36, 22));
            DustType = DustID.WoodFurniture;
			ItemDrop = ModContent.ItemType<SpookyWoodWallItem>();
        }
    }
}