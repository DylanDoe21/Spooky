using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class BirchCandle : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
			AddMapEntry(new Color(155, 153, 153), Lang.GetItemName(ItemID.Candle));
            RegisterItemDrop(ModContent.ItemType<BirchCandleItem>());
            DustType = DustID.Web;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Candles };
        }

        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= 18)
            {
                Main.tile[i, j].TileFrameX -= 18;
            }
            else
            {
                Main.tile[i, j].TileFrameX += 18;
            }
        }

        public override bool RightClick(int i, int j)
        {
            Main.player[Main.myPlayer].PickTile(i, j, 100);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<BirchCandleItem>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                float divide = 400f;

                r = 250f / divide;
                g = 197f / divide;
                b = 55f / divide;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
		{
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

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

			for (int numFlames = 0; numFlames < 5; numFlames++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -5, 6) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -5, 6) * 0.15f;
				spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, 
				new Rectangle(frameX, frameY, width, height), Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}
    }
}