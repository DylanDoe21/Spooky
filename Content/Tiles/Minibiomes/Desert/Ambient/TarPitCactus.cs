using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Desert.Ambient
{
	public class TarPitCactus : ModCactus
	{
		public override void SetStaticDefaults() => GrowsOnTileId = new int[2] { ModContent.TileType<DesertSand>(), ModContent.TileType<DesertSandstone>() };

		public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Desert/Ambient/TarPitCactus");

		public override Asset<Texture2D> GetFruitTexture() => null;
	}
}