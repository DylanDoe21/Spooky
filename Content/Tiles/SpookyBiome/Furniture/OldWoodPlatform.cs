using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class OldWoodPlatform : ModTile
	{
		public override void SetStaticDefaults() 
        {
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileTable[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.Platforms[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
			TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(93, 62, 39));
            ItemDrop = ModContent.ItemType<OldWoodPlatformItem>();
			DustType = DustID.WoodFurniture;
			AdjTiles = new int[] { TileID.Platforms };
		}

		public override void PostSetDefaults() 
		{
			Main.tileNoSunLight[Type] = false;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}
	}
}