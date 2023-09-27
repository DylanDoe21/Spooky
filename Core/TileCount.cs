using Terraria.ID;
using Terraria.ModLoader;
using System;

using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Core
{
	public class TileCount : ModSystem
	{
		public int cemeteryTiles;
		public int spookyTiles;
		public int spookyHellTiles;
		public int glowshroomTiles;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			cemeteryTiles = tileCounts[ModContent.TileType<CemeteryDirt>()] + tileCounts[ModContent.TileType<CemeteryGrass>()] + tileCounts[ModContent.TileType<CemeteryStone>()];
			spookyTiles = tileCounts[ModContent.TileType<SpookyDirt>()] + tileCounts[ModContent.TileType<SpookyDirt2>()] + tileCounts[ModContent.TileType<SpookyGrass>()] + tileCounts[ModContent.TileType<SpookyGrassGreen>()] + tileCounts[ModContent.TileType<SpookyStone>()];
			spookyHellTiles = tileCounts[ModContent.TileType<SpookyMush>()] + tileCounts[ModContent.TileType<SpookyMushGrass>()] + tileCounts[ModContent.TileType<EyeBlock>()];
			glowshroomTiles = tileCounts[ModContent.TileType<MushroomMoss>()];
		}
	}
}