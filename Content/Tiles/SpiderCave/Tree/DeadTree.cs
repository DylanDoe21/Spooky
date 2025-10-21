using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Items.Food;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    public class DeadTree1 : ModTile
    {
        //reminder:
        //X frame 0 = tree segment
        //X frame 18 = top segment
        //X frame 36 = stub top segment

        public override string Texture => "Spooky/Content/Tiles/SpiderCave/Tree/DeadTree";

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
            AddMapEntry(new Color(151, 145, 170), name);
            DustType = DustID.WoodFurniture;
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
            return Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        }

        public static bool Grow(int i, int j, int minSize, int maxSize, int type)
        {
            int height = WorldGen.genRand.Next(minSize, maxSize);
            for (int k = 1; k < height; k++)
            {
                if (SolidTile(i, j - k - 1) || SolidTile(i, j - k - 2) || SolidTile(i, j - k - 3) || SolidTile(i, j - k - 4) || SolidTile(i, j - k - 5))
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
                WorldGen.PlaceTile(i, j, type, true);
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
                WorldGen.PlaceTile(i, j - numSegments, type, true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

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
            if (belowFrame < 36)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 36;
            }
        }

        public static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops1");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopTexture.Width() / 3) / 2) - 17, TopTexture.Height() - 10);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopTexture.Value, new Rectangle(158 * frame, 0, 156, 156), TileGlobal.TileOffset, offset);
            }

            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class DeadTree2 : DeadTree1
    {
        public override string Texture => "Spooky/Content/Tiles/SpiderCave/Tree/DeadTree";

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
            AddMapEntry(new Color(151, 145, 170), name);
            DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops2");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopTexture.Width() / 3) / 2) - 17, TopTexture.Height() - 10);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopTexture.Value, new Rectangle(154 * frame, 0, 152, 162), TileGlobal.TileOffset, offset);
            }

            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class DeadTree3 : DeadTree1
    {
        public override string Texture => "Spooky/Content/Tiles/SpiderCave/Tree/DeadTree";

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
            AddMapEntry(new Color(151, 145, 170), name);
            DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
			TopTexture ??= ModContent.Request<Texture2D>(Texture + "Tops3");
			StemTexture ??= ModContent.Request<Texture2D>(Texture);

			Tile tile = Framing.GetTileSafely(i, j);
            Color col = TileGlobal.GetTileColorWithPaint(i, j, Lighting.GetColor(i, j));

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (Framing.GetTileSafely(i, j).TileFrameX == 18)
            {
                int frame = tile.TileFrameY / 18;

				//reminder: offset negative numbers are right and down, while positive is left and up

				//divide the top width by 3 first since there are 3 horizontal frames, then divide it further after that
				Vector2 offset = new Vector2(((TopTexture.Width() / 3) / 2) - 17, TopTexture.Height() - 10);

				//draw tree tops
				DrawTreeTop(i - 1, j - 1, TopTexture.Value, new Rectangle(174 * frame, 0, 172, 154), TileGlobal.TileOffset, offset);
            }

            spriteBatch.Draw(StemTexture.Value, pos, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), col, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}