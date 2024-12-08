using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Content.NPCs.Boss.Daffodil;
using System;
using ReLogic.Content;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasLightRope : ModTile
	{
		private static Asset<Texture2D> GlowTexture1;
		private static Asset<Texture2D> GlowTexture2;
		private static Asset<Texture2D> GlowTexture3;

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
			GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/Furniture/ChristmasLightRopeGlow1");
			GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/Furniture/ChristmasLightRopeGlow2");
			GlowTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/Furniture/ChristmasLightRopeGlow3");

			Tile tile = Framing.GetTileSafely(i, j);

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			intensity += 0.7f;

			float intensity2 = 0.7f;
			intensity2 *= (float)MathF.Sin(-i / 8f + time + j);
			intensity2 += 0.7f;

			float intensity3 = 0.7f;
			intensity3 *= (float)MathF.Cos(-i / 8f + time + j);
			intensity3 += 0.7f;

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(GlowTexture1.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero,
			new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * intensity);

			spriteBatch.Draw(GlowTexture2.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero,
			new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * intensity2);

			spriteBatch.Draw(GlowTexture3.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero,
			new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * intensity3);
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];

			//this is so goofy its actually over
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

			bool IsRed = tile.TileFrameY == 0 && PurpleFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 1 && PurpleFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && PurpleFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && PurpleFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && PurpleFrameXRow5.Contains(tile.TileFrameX);

			bool IsBlue = tile.TileFrameY == 0 && GreenFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 && GreenFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && GreenFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && GreenFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && GreenFrameXRow5.Contains(tile.TileFrameX);

			bool IsYellow = tile.TileFrameY == 0 && OrangeFrameXRow1.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 1 && OrangeFrameXRow2.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 2 && OrangeFrameXRow3.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 3 && OrangeFrameXRow4.Contains(tile.TileFrameX) ||
			tile.TileFrameY == 18 * 4 && OrangeFrameXRow5.Contains(tile.TileFrameX);

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			//intensity *= (float)MathF.Sin(-i / 8f + time + j);
			intensity += 0.7f;

			float intensity2 = 0.7f;
			intensity2 *= (float)MathF.Sin(-i / 8f + time + j);
			intensity2 += 0.7f;

			float intensity3 = 0.7f;
			intensity3 *= (float)MathF.Cos(-i / 8f + time + j);
			intensity3 += 0.7f;

			if (IsRed)
			{
				r = 229f / 500f * intensity;
				g = 54f / 500f * intensity;
				b = 54f / 500f * intensity;
			}

			if (IsBlue)
			{
				r = 22f / 500f * intensity2;
				g = 128f / 500f * intensity2;
				b = 248f / 500f * intensity2;
			}

			if (IsYellow)
			{
				r = 222f / 500f * intensity3;
				g = 135f / 500f * intensity3;
				b = 37f / 500f * intensity3;
			}
		}
	}
}
