using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class ScleraBlock : ModTile
	{
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(189, 179, 168));
            DustType = DustID.Blood;
            HitSound = SoundID.Dig;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
            GlowTexture ??= ModContent.Request<Texture2D>(Texture);

            Tile tile = Framing.GetTileSafely(i, j);

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
			new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White.MultiplyRGBA(WorldGen.paintColor(Main.tile[i, j].TileColor)) * 0.15f);
		}
	}
}
