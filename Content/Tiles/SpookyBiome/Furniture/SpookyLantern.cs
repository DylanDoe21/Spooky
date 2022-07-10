using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
    public class SpookyLantern : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
			name.SetDefault("Lantern");
			AddMapEntry(new Color(65, 52, 32), name);
            DustType = DustID.WoodFurniture;
        }
		
		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
            {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<SpookyLanternItem>());
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX == 0) 
			{
				r = 0.7f;
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
			int height = 32;
			short frameX = tile.TileFrameX;
			short frameY = tile.TileFrameY;

			TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.

			// We can support different flames for different styles here: int style = Main.tile[j, i].frameY / 54;
			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				Texture2D flameTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Furniture/SpookyLanternFlame").Value;
				spriteBatch.Draw(flameTexture, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, new Rectangle(frameX, frameY, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, effects, 0f);
			}
		}
    }
}