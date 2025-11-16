using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeSink : ModTile
	{
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(114, 13, 39), Language.GetText("MapObject.Sink"));
            DustType = DustID.Blood;
			AdjTiles = new int[] { TileID.Sinks };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/EyeSinkGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}