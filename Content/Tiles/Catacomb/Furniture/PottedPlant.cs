using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class PottedPlant1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(21, 59, 32));
			DustType = DustID.Grass;
			HitSound = SoundID.Dig;
		}
    }

	public class PottedPlant2 : PottedPlant1
	{
	}

	public class PottedPlant3 : PottedPlant1
	{
	}

	public class PottedPlant4 : PottedPlant1
	{
	}

	public class PottedPlant5 : PottedPlant1
	{
	}

	public class PottedPlant6 : PottedPlant1
	{
	}

	public class PottedPlant7 : PottedPlant1
	{
	}

	public class PottedPlant8 : PottedPlant1
	{
	}
}