using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Desert.Ambient
{
    public class TarPitCactus : ModTile
    {
        //reminder:
        //X frame 0 = segment
        //X frame 16 = left branch segment
        //X frame 36 = right branch segment
        //X frame 54 = top segment

        private static Asset<Texture2D> BranchLeftTexture;
		private static Asset<Texture2D> BranchRightTexture;
		private static Asset<Texture2D> TileTexture;

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
            AddMapEntry(new Color(59, 100, 52), name);
            DustType = DustID.Grass;
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
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Grow(int i, int j, int minSize, int maxSize, bool saplingExists = false)
        {
            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 2, 1, TileChangeType.None);
				}
			}

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

            for (int numSegments = 0; numSegments < height; numSegments++)
            {
				if (Main.tile[i - 1, j - numSegments].HasTile || Main.tile[i, j - numSegments].HasTile || Main.tile[i + 1, j - numSegments].HasTile || Main.tile[i, j - numSegments].LiquidAmount > 0)
				{
					return false;
				}
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<TarPitCactus>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

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
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<TarPitCactus>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

				if (WorldGen.genRand.NextBool())
				{
					if (WorldGen.genRand.NextBool())
					{
						Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
					}
					else
					{
						Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
					}
				}
				else
				{
					Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
				}

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 54;
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
                int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ItemID.Cactus);

                if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
                }

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
            //X frame 0 = normal tree segment
            //X frame 16 = tree top draw segment
            //X frame 36 = stubby top segment

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
            if (belowFrame < 54)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 54;
            }
        }

        public static void DrawBranch(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
			Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

			Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			BranchLeftTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Desert/Ambient/TarPitCactusBranchLeft");
			BranchRightTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Desert/Ambient/TarPitCactusBranchRight");
			TileTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));
			float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

			if (xOff == 1 && (j / 4f) == 0)
			{
				xOff = 0;
			}

			int frameSize = 16;
			int frameSizeY = 16;

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
			{
				int frame = tile.TileFrameY / 18;

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2((BranchLeftTexture.Width() / 2) - 1, (BranchLeftTexture.Height() / 3) - 26);

				//draw tree tops
				DrawBranch(i - 1, j - 1, BranchLeftTexture.Value, new Rectangle(0, 18 * frame, 16, 16), TileGlobal.TileOffset, offset);
			}
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
				int frame = tile.TileFrameY / 18;

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2((BranchRightTexture.Width() / 2) - 33, (BranchRightTexture.Height() / 3) - 26);

				//draw tree tops
				DrawBranch(i - 1, j - 1, BranchRightTexture.Value, new Rectangle(0, 18 * frame, 16, 16), TileGlobal.TileOffset, offset);
			}

			Vector2 treeNormalOffset = new Vector2(0, -6);

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY), col, 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);
            }

			//draw the actual tree
			spriteBatch.Draw(TileTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			return false;
		}
    }
}