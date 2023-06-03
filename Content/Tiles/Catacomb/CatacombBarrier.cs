using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBarrier : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(Color.Yellow);
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
			MinPick = Flags.downedBigBone ? 0 : 999999;

            Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey1)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
            }
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
            }

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				float time = Main.GameUpdateCount * 0.01f;

				float intensity = 0.7f;
				intensity *= (float)MathF.Sin(-j / 8f + time + i);
				intensity *= (float)MathF.Sin(-i / 8f + time + j);
				intensity += 0.7f;

				Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Yellow * intensity);
			}

			return false;
		}
    }

	public class CatacombBarrier2 : CatacombBarrier
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

		public override void SetStaticDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(Color.Red);
			HitSound = SoundID.Dig;
			DustType = -1;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            MinPick = Flags.downedBigBone ? 0 : 999999;

            Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey2)
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
            }
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
            }

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				float time = Main.GameUpdateCount * 0.01f;

				float intensity = 0.7f;
				intensity *= (float)MathF.Sin(-j / 8f + time + i);
				intensity *= (float)MathF.Sin(-i / 8f + time + j);
				intensity += 0.7f;

				Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.Red * intensity);
			}

			return false;
		}
	}

	public class CatacombBarrier3 : CatacombBarrier
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBarrier";

		public override void SetStaticDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(Color.Orange);
			HitSound = SoundID.Dig;
			DustType = -1;
		}

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            MinPick = Flags.downedBigBone ? 0 : 999999;

            Tile tile = Framing.GetTileSafely(i, j);

			if (Flags.CatacombKey3)
			{
				for (int k = 0; k < Main.maxNPCs; k++)
				{
					if (Main.npc[k].type == ModContent.NPCType<BigFlowerPot>()) 
					{
						if (Main.npc[k].ai[0] > 0)
						{
							tile.Get<TileWallWireStateData>().IsActuated = false;
						}
					}
				}
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

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				float time = Main.GameUpdateCount * 0.01f;

				float intensity = 0.7f;
				intensity *= (float)MathF.Sin(-j / 8f + time + i);
				intensity *= (float)MathF.Sin(-i / 8f + time + j);
				intensity += 0.7f;

				Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
				spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.OrangeRed * intensity);
			}

			return false;
		}
	}
}
