using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBarrier : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			AddMapEntry(new Color(36, 69, 39));
			DustType = DustID.Grass;
			HitSound = SoundID.Grass;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey1)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
			}
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombBarrierGlow").Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
			}
		}
    }

	public class CatacombBarrier2 : CatacombBarrier
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey2)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
			}
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombBarrierGlow2").Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
			}
		}
	}

	public class CatacombBarrier3 : CatacombBarrier
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
			Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey3)
			{
				if (NPC.CountNPCS(ModContent.NPCType<BigBone>()) > 0)
				{
					tile.Get<TileWallWireStateData>().IsActuated = false;
				}
				else
				{
					tile.Get<TileWallWireStateData>().IsActuated = true;
				}
			}
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
			}
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombBarrierGlow3").Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
			}
		}
	}
}
