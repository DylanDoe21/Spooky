using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpiderCave.Tree
{
    internal class GiantRoot : ModTile
    {
        //reminder:
        //X frame 0 = normal segment
        //X frame 18 = bottom segment
        //X frame 36 = root base
        //X frame 54 = root sides
        //X frame 72 = root stubby bottom

        private Asset<Texture2D> BottomTexture;
        private Asset<Texture2D> RootTexture;

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
            AddMapEntry(new Color(201, 175, 139), name);
            DustType = DustID.Web;
			HitSound = SoundID.Dig;
            RegisterItemDrop(ModContent.ItemType<RootWoodItem>());
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

        public static bool NoSlopedTile(int i, int j)
        {
            return !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock;
        }

        public static bool Grow(int i, int j, int minSize, int maxSize)
        {
            int height = WorldGen.genRand.Next(minSize, maxSize);

            //if the root grows too low, set its height to be higher above the ground
            for (int k = 1; k < height; k++)
            {
                if (SolidTile(i, j + k + 3))
                {
                    height = k - 1;
                    break;
                }
            }

            //dont allow the root to grow if its height ends up being too small
            if (height < minSize)
            {
                return false;
            }

            //make sure the block is valid for the tree to place on
            if (SolidTile(i, j - 1) && NoSlopedTile(i, j - 1) && !Framing.GetTileSafely(i, j).HasTile)
            {
                WorldGen.PlaceTile(i, j, ModContent.TileType<GiantRoot>(), true);
                Framing.GetTileSafely(i, j).TileFrameX = 0;
                Framing.GetTileSafely(i, j).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                //both side roots
                if (SolidTile(i - 1, j - 1) && SolidTile(i + 1, j - 1) && NoSlopedTile(i + 1, j - 1) && NoSlopedTile(i - 1, j - 1) && !Framing.GetTileSafely(i - 1, j).HasTile && !Framing.GetTileSafely(i + 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 0;

                    //left
                    WorldGen.PlaceTile(i - 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i - 1, j).TileFrameY = 18;

                    //right
                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i + 1, j).TileFrameY = 0;
                }
                //left root only
                else if (SolidTile(i - 1, j - 1) && NoSlopedTile(i - 1, j - 1) && !Framing.GetTileSafely(i - 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 18;

                    //left
                    WorldGen.PlaceTile(i - 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i - 1, j).TileFrameY = 18;
                }
                //right root only
                else if (SolidTile(i + 1, j - 1) && NoSlopedTile(i + 1, j - 1) && !Framing.GetTileSafely(i + 1, j).HasTile)
                {
                    //base frame
                    Framing.GetTileSafely(i, j).TileFrameX = 36;
                    Framing.GetTileSafely(i, j).TileFrameY = 36;

                    //right
                    WorldGen.PlaceTile(i + 1, j, ModContent.TileType<GiantRoot>(), true);
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 54;
                    Framing.GetTileSafely(i + 1, j).TileFrameY = 0;
                }
            }
            //otherwise dont allow the tree to grow
            else
            {
                return false;
            }

            //grow the tree itself
            for (int numSegments = 1; numSegments < height; numSegments++)
            {
                WorldGen.PlaceTile(i, j + numSegments, ModContent.TileType<GiantRoot>(), true);
                Framing.GetTileSafely(i, j + numSegments).TileFrameX = 0;
                Framing.GetTileSafely(i, j + numSegments).TileFrameY = (short)(WorldGen.genRand.Next(3) * 18);

                if (numSegments == height - 1)
                {
                    Framing.GetTileSafely(i, j + numSegments).TileFrameX = 18;
                }
            }

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            //kill the tree if there are no tiles above it
            if (!Framing.GetTileSafely(i, j - 1).HasTile)
            {
                WorldGen.KillTile(i, j, false, false, false);
            }

            //kill the side roots of the root if they arent attached to the main root itself
            if (Framing.GetTileSafely(i + 1, j).TileType != ModContent.TileType<GiantRoot>() && Framing.GetTileSafely(i - 1, j).TileType != ModContent.TileType<GiantRoot>() && Framing.GetTileSafely(i, j).TileFrameX == 54)
            {
                WorldGen.KillTile(i, j, false, false, false);
            }
        }

        private void CheckEntireRoot(ref int x, ref int y)
        {
            while (Main.tile[x, y].TileType == Type)
			{
                y++;
			}

            y--;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
                CheckEntireRoot(ref x, ref y);
            }

            if (fail)
            {
                return;
            }

            int aboveFrame = Framing.GetTileSafely(i, j - 1).TileFrameX;

            //if theres any remaining segments below, turn it into a stub bottom segment
            if (aboveFrame < 72)
            {
                Framing.GetTileSafely(i, j - 1).TileFrameX = 72;
            }
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        public static void DrawRootBottom(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
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

            Vector2 offset = (tile.TileFrameX == 0 || tile.TileFrameX == 18) ? new Vector2((xOff * 2) - (frameOff / 2), 0) : Vector2.Zero;
            Vector2 pos = TileCustomPosition(i, j);

            if (tile.TileFrameX == 18)
            {
                BottomTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/GiantRootBottom");
                int frame = tile.TileFrameY / 18;

                Vector2 RootBottomOffset = new Vector2(34, -14);

                //draw tree tops
                DrawRootBottom(i - 1, j - 1, BottomTexture.Value, new Rectangle(88 * frame, 0, 86, 82), TileOffset.ToWorldCoordinates(), RootBottomOffset);
            }

            RootTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/Tree/GiantRoot");

            Vector2 treeNormalOffset = new Vector2(0, 4);

            //draw the actual tree
            spriteBatch.Draw(RootTexture.Value, pos + offset, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}