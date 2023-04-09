using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class Candle : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(225, 225, 0));
			DustType = DustID.Torch;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Candles };
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) 
		{
			if (Main.gamePaused || !Main.instance.IsActive || Lighting.UpdateEveryFrame && !Main.rand.NextBool(4)) 
			{
				return;
			}

			Tile tile = Main.tile[i, j];

			short frameY = tile.TileFrameY;

			int style = frameY / 54;

			if (frameY / 18 % 3 == 0) 
			{
				if (Main.rand.Next(3) == 0)
				{
					int newDust = Dust.NewDust(new Vector2(i * 16 + 4, j * 16 + 2), 4, 4, DustID.Torch, 0f, 0f, 100, default, 1f);

					Main.dust[newDust].noGravity = true;
					Main.dust[newDust].scale = 1.2f;
					Main.dust[newDust].velocity *= 0.3f;
					Main.dust[newDust].velocity.Y -= 0.001f;
				}
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
		{
			SpriteEffects effects = SpriteEffects.None;

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen) 
			{
				zero = Vector2.Zero;
			}

			Tile tile = Main.tile[i, j];
			int width = 16;
			int offsetY = 0;
			int height = 16;
			short frameX = tile.TileFrameX;
			short frameY = tile.TileFrameY;

			TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.

			// We can support different flames for different styles here: int style = Main.tile[j, i].frameY / 54;
			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Furniture/CandleFlame").Value;
				spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, new Rectangle(frameX, frameY, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, effects, 0f);
			}
		}
	}
}