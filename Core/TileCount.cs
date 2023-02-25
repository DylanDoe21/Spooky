using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Core
{
	public class TileCount : ModSystem
	{
		public int cemeteryTiles;
		public int raveyardTiles;
		public int spookyTiles;
		public int spookyHellTiles;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			cemeteryTiles = tileCounts[ModContent.TileType<CemeteryDirt>()] + tileCounts[ModContent.TileType<CemeteryGrass>()] + tileCounts[ModContent.TileType<CemeteryStone>()];
			raveyardTiles = tileCounts[TileID.DiscoBall];
			spookyTiles = tileCounts[ModContent.TileType<SpookyDirt>()] + tileCounts[ModContent.TileType<SpookyDirt2>()] + tileCounts[ModContent.TileType<SpookyGrass>()] + tileCounts[ModContent.TileType<SpookyGrassGreen>()] + tileCounts[ModContent.TileType<SpookyStone>()];
			spookyHellTiles = tileCounts[ModContent.TileType<SpookyMush>()] + tileCounts[ModContent.TileType<SpookyMushGrass>()] + tileCounts[ModContent.TileType<EyeBlock>()];
		}
	}
}