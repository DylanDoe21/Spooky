using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Content.NPCs.Boss.Daffodil;
using System;
using ReLogic.Content;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
	public class ChristmasLightRope : ModTile
	{
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileRope[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = true;
			AddMapEntry(new Color(9, 116, 9));
            DustType = DustID.Grass;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasLightRopeGlow");

			Tile tile = Framing.GetTileSafely(i, j);

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			intensity *= (float)MathF.Sin(-i / 8f + time + j);
			intensity += 0.7f;

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero,
			new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * intensity);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];

			int[] PurpleFrameXRow1 = new int[] { 18 * 0, 18 * 1, 18 * 4, 18 * 5, 18 * 6, 18 * 10, 18 * 12 };
			int[] PurpleFrameXRow2 = new int[] { 18 * 1, 18 * 8 };
			int[] PurpleFrameXRow3 = new int[] { 18 * 3, 18 * 6, 18 * 9, 18 * 10 };
			int[] PurpleFrameXRow4 = new int[] { 18 * 0, 18 * 3, 18 * 5, 18 * 8, 18 * 11 };
			int[] PurpleFrameXRow5 = new int[] { 18 * 4, 18 * 6 };

			int[] GreenFrameXRow1 = new int[] { 18 * 3, 18 * 8, 18 * 9, 18 * 11 };
			int[] GreenFrameXRow2 = new int[] { 18 * 2, 18 * 7, 18 * 11 };
			int[] GreenFrameXRow3 = new int[] { 18 * 0, 18 * 1, 18 * 4, 18 * 5, 18 * 7, 18 * 12 };
			int[] GreenFrameXRow4 = new int[] { 18 * 2, 18 * 4, 18 * 6, 18 * 9 };
			int[] GreenFrameXRow5 = new int[] { 18 * 1, 18 * 3, 18 * 8 };

			int[] OrangeFrameXRow1 = new int[] { 18 * 2, 18 * 7 };
			int[] OrangeFrameXRow2 = new int[] { 18 * 0, 18 * 3, 18 * 4, 18 * 5, 18 * 6, 18 * 9, 18 * 10, 18 * 12 };
			int[] OrangeFrameXRow3 = new int[] { 18 * 2, 18 * 8, 18 * 11 };
			int[] OrangeFrameXRow4 = new int[] { 18 * 1, 18 * 7, 18 * 10 };
			int[] OrangeFrameXRow5 = new int[] { 18 * 0, 18 * 2, 18 * 5, 18 * 7 };

			bool IsPurple = tile.TileFrameY == 0 && PurpleFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 1 && PurpleFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && PurpleFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && PurpleFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && PurpleFrameXRow5.Contains(tile.TileFrameX);

			bool IsGreen = tile.TileFrameY == 0 && GreenFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 && GreenFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && GreenFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && GreenFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && GreenFrameXRow5.Contains(tile.TileFrameX);

			bool IsOrange = tile.TileFrameY == 0 && OrangeFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 1 && OrangeFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && OrangeFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && OrangeFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && OrangeFrameXRow5.Contains(tile.TileFrameX);

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			intensity *= (float)MathF.Sin(-i / 8f + time + j);
			intensity += 0.7f;

			if (IsPurple)
			{
				r = 178f / 500f * intensity;
				g = 0f;
				b = 255f / 500f * intensity;
			}

			if (IsGreen)
			{
				r = 0f;
				g = 255f / 500f * intensity;
				b = 125f / 500f * intensity;
			}

			if (IsOrange)
			{
				r = 255f / 500f * intensity;
				g = 95f / 500f * intensity;
				b = 0f;
			}
		}
	}
}
