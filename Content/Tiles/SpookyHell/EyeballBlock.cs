using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Terraria.GameContent.Drawing;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeballBlock : ModTile
	{
        public override string Texture => "Spooky/Content/Tiles/SpookyHell/LivingFlesh";

        private Asset<Texture2D> EyeTexture;
		private Asset<Texture2D> EyeGlowTexture;

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(145, 24, 12));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
		}

		//cannot be sloped at all
		public override bool Slope(int i, int j)
		{
			return false;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) 
		{
			Main.instance.TilesRenderer.AddSpecialPoint(i, j, TileDrawing.TileCounterType.CustomSolid);
		}

		public static void DrawEyeball(int i, int j, Texture2D tex, Rectangle? source, Vector2 scaleVec, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
		{
			Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
			Color color = Lighting.GetColor(i, j, WorldGen.paintColor(Main.tile[i, j].TileColor));

			if (Glow)
			{
				Main.spriteBatch.Draw(tex, drawPos, source, Color.White, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
			}
			else
			{
				Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
		{
			EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/EyeBallBlockDraw");
			EyeGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/EyeBallBlockDrawGlow");

			Tile tile = Framing.GetTileSafely(i, j);

			int frame = tile.TileFrameNumber % 3;

			Vector2 offset = new Vector2((EyeTexture.Width() / 2) + 192, ((EyeTexture.Height() / 3) / 2) + 192);

            DrawEyeball(i, j, EyeTexture.Value, new Rectangle(0, 28 * frame, 28, 28), default, TileGlobal.TileOffset, offset);
			DrawEyeball(i, j, EyeGlowTexture.Value, new Rectangle(0, 28 * frame, 28, 28), default, TileGlobal.TileOffset, offset, true);
		}
    }
}
