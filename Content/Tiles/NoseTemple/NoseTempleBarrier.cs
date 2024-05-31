using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.NoseCult;

namespace Spooky.Content.Tiles.Catacomb
{
	public class NoseTempleBarrier : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

        private static Asset<Texture2D> TileTexture;

        public override void SetStaticDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(Color.Lime);
			MinPick = int.MaxValue;
			HitSound = SoundID.Dig;
			DustType = -1;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            TileTexture ??= ModContent.Request<Texture2D>(Texture);

            Tile tile = Framing.GetTileSafely(i, j);

			if (!NoseCultAmbushWorld.AmbushActive)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
            }
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
            }

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(time + j);
			intensity += 2f;

			intensity = MathHelper.Clamp(intensity, 0.1f, 1f);

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Lime * intensity);
			}
			else
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Lime * 0.05f);
			}

			return false;
		}
    }

	public class NoseTempleBarrier2 : NoseTempleBarrier
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

        private static Asset<Texture2D> TileTexture;

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            TileTexture ??= ModContent.Request<Texture2D>(Texture);

            Tile tile = Framing.GetTileSafely(i, j);

			bool IsSmallWorld = Main.maxTilesX < 6400;

			bool AllIdolsDowned = IsSmallWorld ? Flags.downedMocoIdol1 && Flags.downedMocoIdol3 && Flags.downedMocoIdol4 : Flags.downedMocoIdol1 && Flags.downedMocoIdol2 && Flags.downedMocoIdol3 && Flags.downedMocoIdol4 && Flags.downedMocoIdol5;

			if (AllIdolsDowned)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
            }
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
            }

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(time + j);
			intensity += 0.7f;

			intensity = MathHelper.Clamp(intensity, 0.5f, 1f);

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Lime * intensity);
			}
			else
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Lime * 0.05f);
			}

			return false;
		}
	}
}
