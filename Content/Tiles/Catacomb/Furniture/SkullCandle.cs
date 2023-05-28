using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class SkullCandle1 : ModTile
    {
		public override void SetStaticDefaults() 
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(130, 123, 95));
            DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0) 
			{
				r = 0.25f;
				g = 0.4f;
				b = 0f;
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

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Furniture/SkullCandle1Flame").Value;
				spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 15f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, 
				new Rectangle(frameX, frameY, width, height), new Color(125, 255, 0, 0), 0f, default, 1f, effects, 0f);
			}
		}
	}

    public class SkullCandle2 : SkullCandle1
    {
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

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Furniture/SkullCandle2Flame").Value;
				spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 15f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, 
				new Rectangle(frameX, frameY, width, height), new Color(125, 255, 0, 0), 0f, default, 1f, effects, 0f);
			}
		}
    }

    public class SkullCandle3 : SkullCandle1
    {
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

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Furniture/SkullCandle3Flame").Value;
				spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 15f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, 
				new Rectangle(frameX, frameY, width, height), new Color(125, 255, 0, 0), 0f, default, 1f, effects, 0f);
			}
		}
    }
}