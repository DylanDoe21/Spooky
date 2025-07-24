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
using Spooky.Content.Items.Minibiomes.Ocean;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Tree
{
    public class BoneFishTree : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 18 = tree top segment
        //X frame 36 = stubby top segment

        private static Asset<Texture2D> TopTexture;
        private static Asset<Texture2D> StemTexture;
        private static Asset<Texture2D> TopGlowTexture;
        private static Asset<Texture2D> StemGlowTexture;

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
            AddMapEntry(new Color(148, 151, 114), name);
            DustType = DustID.Bone;
			HitSound = SoundID.DD2_SkeletonHurt;
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
                if (SolidTile(i, j - k - 1))
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
                WorldGen.PlaceTile(i, j, ModContent.TileType<BoneFishTree>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(2) * 18);

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
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<BoneFishTree>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(2) * 18);

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
                int NewItem = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), ModContent.ItemType<FishboneChunk>());

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
            if (belowFrame < 36)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 36;
            }

            if (tile.TileFrameX == 18)
            {
                if (tile.TileFrameY == 0)
                {
                    //spawn gores out of the tree
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeTopGore1").Type);
                    }
                }
                else
                {
                    //spawn gores out of the tree
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j - 2) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeTopGore2").Type);
                    }
                }
            }
            
            if (tile.TileFrameX == 0)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), Vector2.Zero, ModContent.Find<ModGore>("Spooky/BoneFishTreeGore").Type);
                }
            }
        }

        public static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

            if (!Glow)
            {
                Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                float glowspeed = Main.GameUpdateCount * 0.01f;
			    float glowbrightness = (float)MathF.Sin(j / 5f - glowspeed);

                for (int circle = 0; circle < 360; circle += 90)
                {
                    Vector2 circular = new Vector2(2.5f, 0).RotatedBy(MathHelper.ToRadians(circle));

                    Main.spriteBatch.Draw(tex, drawPos + circular, source, (Color.Lime * 0.5f) * glowbrightness, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
                }
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TopGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Tree/BoneFishTreeTopGlow");
			StemGlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			Tile tile = Framing.GetTileSafely(i, j);

            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;
            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameSizeY = 16;

            Vector2 WavyOffset = new Vector2((xOff * 2), 0);
            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 2 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopGlowTexture.Width() / 2) / 2) - 17, TopGlowTexture.Height() - 10);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopGlowTexture.Value, new Rectangle(30 * frame, 0, 30, 32), TileGlobal.TileOffset + WavyOffset, offset, true);
            }

            float glowspeed = Main.GameUpdateCount * 0.01f;
			float glowbrightness = (float)MathF.Sin(j / 5f - glowspeed);

			if (glowbrightness > 0)
			{
				Lighting.AddLight(new Vector2(i * 16, j * 16), Color.Lime.ToVector3() * 0.35f * glowbrightness);
			}

			spriteBatch.Draw(StemGlowTexture.Value, pos + WavyOffset + new Vector2(2, 0), new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY),
			Color.Lime * 0.85f * glowbrightness, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(StemGlowTexture.Value, pos + WavyOffset + new Vector2(-2, 0), new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY),
			Color.Lime * 0.85f * glowbrightness, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			return false;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TopTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Tree/BoneFishTreeTop");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

            Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameSizeY = 16;

            Vector2 WavyOffset = new Vector2((xOff * 2), 0);
            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            //draw the actual tree itself
            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 2 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopTexture.Width() / 2) / 2) - 17, TopTexture.Height() - 10);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopTexture.Value, new Rectangle(30 * frame, 0, 30, 32), TileGlobal.TileOffset + WavyOffset, offset, false);
            }

            spriteBatch.Draw(StemTexture.Value, pos + WavyOffset, new Rectangle(tile.TileFrameX, tile.TileFrameY, frameSize, frameSizeY), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}