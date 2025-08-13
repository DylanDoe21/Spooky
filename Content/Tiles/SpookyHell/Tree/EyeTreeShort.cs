using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Items.Food;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
    public class EyeTreeShort : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = tree top draw segment
        //X frame 36 = branch draw segment
        //X frame 54 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> StemTexture;
        private static Asset<Texture2D> BranchLeftTexture;
        private static Asset<Texture2D> BranchRightTexture;
        private static Asset<Texture2D> TopGlowTexture;
        private static Asset<Texture2D> StemGlowTexture;
        private static Asset<Texture2D> BranchLeftGlowTexture;
        private static Asset<Texture2D> BranchRightGlowTexture;

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
            AddMapEntry(new Color(168, 58, 96), name);
            DustType = DustID.Blood;
			HitSound = SoundID.NPCHit13;
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
                WorldGen.PlaceTile(i, j, ModContent.TileType<EyeTreeShort>(), true);
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
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<EyeTreeShort>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //randomly place branch segment
                if (numSegments > 1 && numSegments < height - 1 && Framing.GetTileSafely(i, j - numSegments + 1).TileFrameX != 36)
                {
                    if (Main.rand.NextBool())
                    {
                        Framing.GetTileSafely(i, j - numSegments).TileFrameX = 36;
                    }
                }

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
                int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<LivingFleshItem>());

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

            if (Main.tile[x, y].TileFrameX == 18)
            {
                //spawn a fruit from the tree
                if (Main.rand.NextBool(30))
                {
                    int NewItem = Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<EyeFruit>(), Main.rand.Next(1, 4));

                    if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
					}
                }
            }
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            //X frame 0 = normal tree segment
            //X frame 16 = tree top draw segment
            //X frame 36 = branch segments
            //X frame 54 = stubby top segment

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

            if (tile.TileFrameX == 18)
            {
                //play squishy sound
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                //spawn a seed from the tree
                if (Main.rand.NextBool())
                {
                    int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<EyeSeed>(), Main.rand.Next(1, 4));

                    if (Main.netMode == NetmodeID.MultiplayerClient && NewItem >= 0)
					{
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, NewItem, 1f);
					}
                }

                int EyeBlock = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<EyeballBlockItem>(), Main.rand.Next(5, 11));

                if (Main.netMode == NetmodeID.MultiplayerClient && EyeBlock >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, EyeBlock, 1f);
                }

                //spawn gores out of the tree
                for (int numGores = 0; numGores <= Main.rand.Next(2, 4); numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16),
                        new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                    }
                }
            }

            if (tile.TileFrameX == 36)
            {
                //left branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
                    for (int numGores = 0; numGores <= Main.rand.Next(1, 3); numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i - 2, j) * 16),
                            new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                        }
                    }
                }

                //right branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
                    for (int numGores = 0; numGores <= Main.rand.Next(1, 3); numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i + 2, j) * 16),
                            new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(1, 4)).Type);
                        }
                    }
                }
            }

            if (tile.TileFrameX == 54)
            {
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16),
                    new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore3").Type);
                }
            }
        }

        public static void DrawTreeStuff(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortTops");
            BranchLeftTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortBranchLeft");
            BranchRightTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortBranchRight");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopTexture.Width() / 3) / 2) - 16, TopTexture.Height() - 10);

				//draw tree tops
				DrawTreeStuff(i - 1, j - 1, TopTexture.Value, new Rectangle(60 * frame, 0, 58, 44), TileGlobal.TileOffset, offset, false);
            }

            //draw branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
				//left branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2((BranchLeftTexture.Width() / 2) + 18, -(BranchLeftTexture.Height() / 3) + 54);

					DrawTreeStuff(i - 1, j - 1, BranchLeftTexture.Value, new Rectangle(0, 46 * frame, 58, 44), TileGlobal.TileOffset, offset, false);
				}

				//right branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2(-(BranchRightTexture.Width() / 2) + 8, -(BranchRightTexture.Height() / 3) + 54);

					DrawTreeStuff(i - 1, j - 1, BranchRightTexture.Value, new Rectangle(0, 46 * frame, 58, 44), TileGlobal.TileOffset, offset, false);
				}
			}

            //draw extra tile below so it looks attached to the ground
            if (Main.tile[i, j + 1].TileType != Type)
            {
                spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, new Vector2(0, -6), 1f, SpriteEffects.None, 0f);
            }

            //draw the actual tree
            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TopGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortTopsGlow");
            BranchLeftGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortBranchLeftGlow");
            BranchRightGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeShortBranchRightGlow");
			StemGlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopGlowTexture.Width() / 3) / 2) - 16, TopGlowTexture.Height() - 10);

				//draw tree tops
				DrawTreeStuff(i - 1, j - 1, TopGlowTexture.Value, new Rectangle(60 * frame, 0, 58, 44), TileGlobal.TileOffset, offset, true);
            }

            //draw branches
			if (Framing.GetTileSafely(i, j).TileFrameX == 36)
			{
				//left branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 18 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2((BranchLeftGlowTexture.Width() / 2) + 18, -(BranchLeftGlowTexture.Height() / 3) + 54);

					DrawTreeStuff(i - 1, j - 1, BranchLeftGlowTexture.Value, new Rectangle(0, 46 * frame, 58, 44), TileGlobal.TileOffset, offset, true);
				}

				//right branches
				if (Framing.GetTileSafely(i, j).TileFrameY == 0 || Framing.GetTileSafely(i, j).TileFrameY == 36)
				{
					int frame = tile.TileFrameY / 18;

					Vector2 offset = new Vector2(-(BranchRightGlowTexture.Width() / 2) + 8, -(BranchRightGlowTexture.Height() / 3) + 54);

					DrawTreeStuff(i - 1, j - 1, BranchRightGlowTexture.Value, new Rectangle(0, 46 * frame, 58, 44), TileGlobal.TileOffset, offset, true);
				}
			}

            //draw the actual tree
            spriteBatch.Draw(StemGlowTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}