using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Jungle.Tree
{
    public class Broccoli : ModTile
    {
        //reminder:
        //X frame 0 = normal segment
        //X frame 18 = branch segment
        //X frame 36 = top segment
        //X frame 54 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> BranchTexture;
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
            AddMapEntry(new Color(55, 98, 58), name);
            DustType = DustID.Grass;
			HitSound = SoundID.Grass;
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

			/*
			//make sure the block is valid for the tree to place on
			if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
			{
				WorldGen.PlaceTile(i, j, ModContent.TileType<Broccoli>(), true);
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
			*/

			//preform a loop where the tree will grow and check to make sure no tiles are above it
			//if there are tiles blocking the way, dont allow the tree to grow
			for (int numSegments = 0; numSegments < height; numSegments++)
			{
				Tile above = Main.tile[i, j - numSegments - 1];

				if (above.HasTile)
				{
					return false;
				}
			}

			for (int numSegments = 0; numSegments < height; numSegments++)
			{
				WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<Broccoli>(), true);
				Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
				Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

				if (WorldGen.genRand.NextBool(4))
				{
					Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
				}

				if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
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
            if (belowFrame < 54)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 54;
            }

            if (tile.TileFrameX == 36)
            {
                /*
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16),
                    new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore3").Type);
                }
                */
            }
        }

        public static void DrawTreePiece(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
			Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

			Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Jungle/Tree/BroccoliTop");
			BranchTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Jungle/Tree/BroccoliBranches");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
			Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//draw tree branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
				//reminder: offset negative numbers are right and down, while positive is left and up

				//left branch
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 36)
                {
                    Vector2 offset = new Vector2((BranchTexture.Width() / 2), -(BranchTexture.Height() / 4) + 1);
                    DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 0, 18, 16), TileGlobal.TileOffset, offset);
                }
                //right branch
                if (Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 36)
                {
					Vector2 offset = new Vector2(-(BranchTexture.Width() / 2) - 14, -(BranchTexture.Height() / 4) + 1);
					DrawTreePiece(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 18, 18, 16), TileGlobal.TileOffset, offset);
                }
            }

            //draw tree tops
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
            {
                Vector2 offset = new Vector2((TopTexture.Width() / 4) - 3, TopTexture.Height() - 8);

                //draw tree tops
                DrawTreePiece(i - 1, j - 1, TopTexture.Value, new Rectangle(0, 0, 52, 36), TileGlobal.TileOffset, offset);
            }

            //draw the actual tree
            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}