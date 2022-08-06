using Terraria.ModLoader;
using System;

using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.Catacomb;

namespace Spooky.Core
{
	public class TileCount : ModSystem
	{
		public int spookyTiles;
		public int spookyHellTiles;

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			spookyTiles = tileCounts[ModContent.TileType<SpookyGrass>()] + tileCounts[ModContent.TileType<SpookyStone>()];
			spookyHellTiles = tileCounts[ModContent.TileType<SpookyMush>()] + tileCounts[ModContent.TileType<EyeBlock>()];
		}
	}
}