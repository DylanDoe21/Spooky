using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.OldHunter;

namespace Spooky.Content.Tiles.Catacomb
{
	public class OldHunterBarrier : ModTile
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
			AddMapEntry(Color.YellowGreen);
			MinPick = int.MaxValue;
			HitSound = SoundID.Dig;
			DustType = -1;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
            TileTexture ??= ModContent.Request<Texture2D>(Texture);

            Tile tile = Framing.GetTileSafely(i, j);

			if (NPC.AnyNPCs(ModContent.NPCType<OldHunterBoss>()))
			{
				tile.Get<TileWallWireStateData>().IsActuated = false;
			}
			else
			{
				tile.Get<TileWallWireStateData>().IsActuated = true;
			}

			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(-j / 8f + time + i);
			intensity *= (float)MathF.Sin(-i / 8f + time + j);
			intensity += 0.7f;

			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (!tile.Get<TileWallWireStateData>().IsActuated)
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.YellowGreen * intensity);
			}
			else
			{
				spriteBatch.Draw(TileTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.YellowGreen * 0.1f);
			}

			return false;
		}
	}
}