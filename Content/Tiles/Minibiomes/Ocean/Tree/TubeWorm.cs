using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Tree
{
    public class TubeWorm : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> StemTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(207, 207, 187), name);
            DustType = DustID.Slush;
			HitSound = SoundID.Dig;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            resetFrame = false;
            noBreak = true;
            return false;
        }

        public static bool SolidTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        }

        public static bool SolidTopTile(int i, int j) 
        {
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || 
            Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Grow(int i, int j, int minSize, int maxSize)
        {
            int height = WorldGen.genRand.Next(minSize, maxSize);
            for (int k = 1; k < height; k++)
            {
                if (SolidTile(i, j - k - 6))
                {
                    height = k - 2;
                    break;
                }
            }

            if (height < minSize) 
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<TubeWorm>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 1, 1, TileChangeType.None);
				}
			}
            //otherwise dont allow the tree to grow
            else
            {
                return false;
            }

            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<TubeWorm>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                }

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j - numSegments, 1, 1, TileChangeType.None);
				}
			}

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            //kill the tree if there are no tiles below it
            if (!Framing.GetTileSafely(i, j + 1).HasTile)
            {
                WorldGen.KillTile(i, j, false, false, false);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                }
            }
        }

        private void CheckEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
			{
                y--;
			}

            y++;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
                CheckEntireTree(ref x, ref y);
            }

            if (fail)
            {
                return;
            }

            int belowFrame = Framing.GetTileSafely(i, j + 1).TileFrameX;

            //if theres any remaining segments below, turn it into a stub top segment
            if (belowFrame == 0)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 18;
            }
        }

        public static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle source, Vector2 offset)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + offset;
            Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

			float Update = Main.GameUpdateCount * 0.0052f;
			float Rotation = (float)MathF.Sin(i / 20f - Update);

			Main.spriteBatch.Draw(tex, drawPos, source, color, Rotation * 0.35f, new Vector2((tex.Width / 4) / 2, tex.Height), 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			StemTexture ??= ModContent.Request<Texture2D>(Texture);
            TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Tree/TubeWormTop");

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            int frameSize = 16;
            int frameSizeY = 16;

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//draw the actual tree
			spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//draw the tree tops over the tree itself
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
			{
				int frame = tile.TileFrameY / 18;

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(17, 12);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopTexture.Value, new Rectangle(28 * frame, 0, 26, 54), TileGlobal.TileOffset + offset);
			}

			return false;
        }
    }
}