using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
	[LegacyName("TallEyeStalk1")]
	[LegacyName("TallEyeStalk2")]
	[LegacyName("TallEyeStalk3")]
	[LegacyName("Tentacle2")]
	[LegacyName("StalkPurple2")]
	[LegacyName("StalkRed2")]
	public class EyeStalkThinTall : ModTile
	{
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
           	AddMapEntry(new Color(145, 24, 12));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit13;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Ambient/EyeStalkThinTallGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}