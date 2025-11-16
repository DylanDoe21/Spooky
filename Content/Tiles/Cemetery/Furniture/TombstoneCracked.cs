using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
    [LegacyName("TombstoneCracked1")]
    [LegacyName("TombstoneCracked2")]
    public class TombstoneCracked : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(53, 68, 60));
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<CemeteryStoneItem>(), Main.rand.Next(3, 10));
        }
    }
}