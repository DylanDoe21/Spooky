﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Tree
{
    internal class GiantShroomYellow : ModTile
    {
        //reminder:
        //X frame 0 = root segment
        //X frame 18 = normal tree segment
        //X frame 36 = top segment
        //X frame 54 = branches segment
        //X frame 72 = stubby top segment

        private Asset<Texture2D> TopTexture;
        private Asset<Texture2D> CapTexture;
		private Asset<Texture2D> BranchTexture;
        private Asset<Texture2D> SideFungusTexture;
        private Asset<Texture2D> RootTexture;
        private Asset<Texture2D> StemTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(208, 162, 44), name);
            DustType = DustID.Slush;
			HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            //create light at the top of the tree
            if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
                r = 255f / 450f;
                g = 185f / 450f;
                b = 0f;
            }
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
                if (SolidTile(i, j - k))
                {
                    height = k - 2;
                    break;
                }
            }

            //if the trees height is too short, dont let it grow
            if (height < minSize) 
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if ((SolidTopTile(i, j + 1) || SolidTile(i, j + 1)) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<GiantShroomYellow>(), true);
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
                //place tree segments
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<GiantShroomYellow>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                //place root segment at the bottom
                if (numSegments == 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                }

                if (numSegments > 1 && numSegments < height - 1)
                {
                    if (Main.rand.NextBool(6))
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 54;
                    }
                    else
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                    }
                }

                //place the tree top at the top of the tree
                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
                }

                if (Framing.GetTileSafely(i, j - numSegments + 1).TileType != ModContent.TileType<MushroomMoss>() && Framing.GetTileSafely(i, j - numSegments).TileFrameX == 0)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
                }

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, i, j, 1, 1, TileChangeType.None);
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

                int NewItem = Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), (new Vector2(i, j) * 16), ModContent.ItemType<SpookyGlowshroom>());

                if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
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
            if (belowFrame < 72)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 72;
            }
        }

        public static void DrawTreeStuff(int i, int j, Texture2D tex, Rectangle? source, Vector2 scaleVec, Vector2? offset = null, Vector2? origin = null, bool shake = false)
        {
            if (shake)
            {
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 15;
                scaleVec = new Vector2(1f, -MathF.Sin(-i / 8f + sin));
            }

            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomYellowTop");
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomYellowTopCap");
			RootTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomYellowRoots");
			SideFungusTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomYellowSides");
			BranchTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomYellowBranches");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//X frame 0 = root segment
			//X frame 18 = normal tree segment
			//X frame 36 = top segment
			//X frame 54 = branches segment
			//X frame 72 = stubby top segment

			//draw the actual tree

			spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY),
			new Color(col.R, col.G, col.B, 255), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//draw branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 54)
			{
				//left branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2((BranchTexture.Width() / 2) + 8, -(BranchTexture.Height() / 4) + 40);

					DrawTreeStuff(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 32 * frame, 34, 30), default, TileGlobal.TileOffset, offset, false);
				}

				//right branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 36 || Framing.GetTileSafely(i, j).TileFrameY == 54)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2(-(BranchTexture.Width() / 2) - 6, -(BranchTexture.Height() / 4) + 40);

					DrawTreeStuff(i - 1, j - 1, BranchTexture.Value, new Rectangle(0, 32 * frame, 34, 30), default, TileGlobal.TileOffset, offset, false);
				}
			}

			//left side fungus
			if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 18)
			{
				Vector2 offset = new Vector2(-(SideFungusTexture.Width() / 2) + 7, -(SideFungusTexture.Height() / 2));

				DrawTreeStuff(i - 1, j - 1, SideFungusTexture.Value, new Rectangle(0, 0, 14, 10), default, TileGlobal.TileOffset, offset, false);
			}

			//right side fungus
			if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 36)
			{
				Vector2 offset = new Vector2(-(SideFungusTexture.Width() / 2) - 11, -(SideFungusTexture.Height() / 2));

				DrawTreeStuff(i - 1, j - 1, SideFungusTexture.Value, new Rectangle(0, 12, 14, 10), default, TileGlobal.TileOffset, offset, false);
			}

			//draw roots at the bottom of the tree
			if (Framing.GetTileSafely(i, j).TileFrameX == 0)
			{
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 18)
				{
					Vector2 offset = new Vector2(-(RootTexture.Width() / 2) + 21, -(RootTexture.Height() / 2) - 2);

					DrawTreeStuff(i - 1, j - 1, RootTexture.Value, new Rectangle(0, 0, 38, 14), default, TileGlobal.TileOffset, offset, false);
				}

				if (Framing.GetTileSafely(i, j).TileFrameY == 36 || Framing.GetTileSafely(i, j).TileFrameY == 54)
				{
					Vector2 offset = new Vector2(-(RootTexture.Width() / 2) + 21, -(RootTexture.Height() / 2) - 2);

					DrawTreeStuff(i - 1, j - 1, RootTexture.Value, new Rectangle(0, 16, 36, 14), default, TileGlobal.TileOffset, offset, false);
				}
			}

			//draw the tree tops
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
				Vector2 treeOffset = new Vector2((TopTexture.Width() / 2) - 17, TopTexture.Height() - 10);
				Vector2 capOffset = new Vector2((TopTexture.Width() / 2) - 17, TopTexture.Height() - 12);

				DrawTreeStuff(i - 1, j - 1, TopTexture.Value, new Rectangle(0, 0, 112, 74), default, TileGlobal.TileOffset, treeOffset, false);
				DrawTreeStuff(i - 1, j - 1, CapTexture.Value, new Rectangle(0, 0, 112, 74), default, TileGlobal.TileOffset, capOffset, true);
			}

			return false;
        }
    }
}