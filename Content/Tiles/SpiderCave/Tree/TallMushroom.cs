using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    internal class TallMushroom : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 16 = tree top draw segment

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = false;
            Main.tileBlockLight[Type] = false;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(122, 113, 98), name);
            DustType = DustID.Slush;
			HitSound = SoundID.Dig;
            MineResist = 0.65f;
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
                if (SolidTile(i, j - k))
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
                WorldGen.PlaceTile(i, j, ModContent.TileType<TallMushroom>(), true);
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(8) * 18);
            }
            //otherwise dont allow the tree to grow
            else
            {
                return false;
            }

            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                WorldGen.PlaceTile(i, j - numSegments, ModContent.TileType<TallMushroom>(), true);
                Framing.GetTileSafely(i, j - numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j - numSegments).TileFrameY = (short)(WorldGen.genRand.Next(8) * 18);

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j - numSegments).TileFrameX = 18;
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

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        public static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 offset = new Vector2((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = TileCustomPosition(i, j + 1);

            if (tile.TileFrameX == 18)
            {
                Texture2D topsTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/TallMushroomTops").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 TopOffset = new Vector2(2, 2);

                if (tile.TileFrameY == 0 || tile.TileFrameY == 18)
                {
                    Lighting.AddLight(new Vector2(i * 16, j * 16), new Color(186, 255, 51).ToVector3() * 0.45f);
                }
                if (tile.TileFrameY == 36 || tile.TileFrameY == 54)
                {
                    Lighting.AddLight(new Vector2(i * 16, j * 16), new Color(255, 36, 45).ToVector3() * 0.45f);
                }
                if (tile.TileFrameY == 72 || tile.TileFrameY == 90)
                {
                    Lighting.AddLight(new Vector2(i * 16, j * 16), new Color(255, 174, 50).ToVector3() * 0.45f);
                }
                if (tile.TileFrameY == 108 || tile.TileFrameY == 126)
                {
                    Lighting.AddLight(new Vector2(i * 16, j * 16), new Color(74, 68, 255).ToVector3() * 0.45f);
                }

                //draw tree tops
                DrawTreeTop(i - 1, j - 1, topsTex, new Rectangle(22 * frame, 0, 20, 16), TileOffset.ToWorldCoordinates(), TopOffset);
            }

            Texture2D treeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/TallMushroom").Value;

            Vector2 treeNormalOffset = new Vector2(0, 4);

            //draw the actual tree
            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}