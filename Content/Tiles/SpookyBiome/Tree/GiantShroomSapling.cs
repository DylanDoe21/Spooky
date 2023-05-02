using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spooky.Content.Tiles.SpookyBiome.Tree
{
	public class GiantShroomSapling : ModTile
	{
		public override void SetStaticDefaults() 
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.CommonSapling[Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<SpookyStone>() };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleMultiplier = 3;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(104, 95, 128), name);
            DustType = DustID.Slush;
			AdjTiles = new int[] { TileID.Saplings };
		}

        public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}

		public override void RandomUpdate(int i, int j) 
		{
            if (Main.tile[i, j + 1].TileType != ModContent.TileType<SpookyGrass>() && Main.tile[i, j + 1].TileType != ModContent.TileType<SpookyGrassGreen>() &&
			Main.tile[i, j + 1].TileType != ModContent.TileType<SpookyStone>())
            {
				if (WorldGen.genRand.Next(10) == 0)
				{
					//make the mushroom grow height bigger if its on the surface
					int minHeight = j <= (int)Main.worldSurface ? 8 : 5;
					int maxHeight = j <= (int)Main.worldSurface ? 15 : 8;

					GiantShroom.Grow(i, j + 1, minHeight, maxHeight, true);
				}
            }
		}
	}
}