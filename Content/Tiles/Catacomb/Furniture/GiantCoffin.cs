using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class GiantCoffin1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 4;	
			TileObjectData.newTile.Origin = new Point16(1, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(122, 81, 70));
			DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}
    }

	public class GiantCoffin2 : GiantCoffin1
	{
	}
}