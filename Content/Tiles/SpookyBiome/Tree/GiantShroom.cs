using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyBiome.Tree
{
    internal class GiantShroom : ModTile
    {
        //reminder:
        //X frame 0 = normal tree segment
        //X frame 16 = tree top draw segment
        //X frame 36 = stubby top segment

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
            AddMapEntry(new Color(104, 95, 128), name);
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

        public static bool Spawn(int i, int j, int minSize = 5, int maxSize = 20, bool saplingExists = false)
        {
            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);
            }

            int height = WorldGen.genRand.Next(minSize, maxSize); //Height & trunk
            for (int k = 1; k < height; ++k)
            {
                if (SolidTile(i, j - k))
                {
                    height = k - 2;
                    break;
                }
            }

            if (height < 4 || height < minSize) 
            {
                return false;
            }

            bool[] extraPlaces = new bool[5];
            for (int k = -2; k <= 2; k++) //check base
            {
                extraPlaces[k + 2] = false;

                if ((SolidTopTile(i + k, j + 1) || SolidTile(i + k, j + 1)) && !Framing.GetTileSafely(i + k, j).HasTile)
                {
                    extraPlaces[k + 2] = true;
                }
            }

            if (!extraPlaces[1]) extraPlaces[0] = false;
            if (!extraPlaces[3]) extraPlaces[4] = false;

            if (!extraPlaces[2]) 
            {
                return false;
            }

            extraPlaces = new bool[5] { false, false, true, false, false };

            for (int k = -2; k <= 2; k++) //place base
            {
                if (extraPlaces[k + 2])
                {
                    WorldGen.PlaceTile(i + k, j, ModContent.TileType<GiantShroom>(), true);
                }
                else
                {
                    continue;
                }

                Framing.GetTileSafely(i + k, j).TileFrameX = (short)((k + 2) * 18);
                Framing.GetTileSafely(i + k, j).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                if (!extraPlaces[1] && !extraPlaces[3] && k == 0) 
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 0;
                }
            }

            for (int k = 1; k < height; k++)
            {
                WorldGen.PlaceTile(i, j - k, ModContent.TileType<GiantShroom>(), true);
                Framing.GetTileSafely(i, j - k).TileFrameX = 0;
                Framing.GetTileSafely(i, j - k).TileFrameY = (short)(WorldGen.genRand.Next(4) * 18);

                if (k == height - 1)
                {
                    Framing.GetTileSafely(i, j - k).TileFrameX = 16;
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

            if (Main.tile[x, y].TileFrameX == 16)
            {
                //spawn a seed from the tree
                if (Main.rand.Next(10) == 0)
                {
                    Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<GiantShroomSeed>(), Main.rand.Next(1, 2));
                }
            }
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
            if (belowFrame == 0 || belowFrame == 16)
            {
                Framing.GetTileSafely(i, j + 1).TileFrameX = 36;
            }
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        public static void DrawTreestuff(int i, int j, Texture2D tex, Rectangle? source, Vector2 scaleVec, Vector2? offset = null, Vector2? origin = null, bool shake = false)
        {
            if (shake)
            {
                float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 15;
                scaleVec = new Vector2(1f, -MathF.Sin(sin));
            }

            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
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

            Vector2 pos = TileCustomPosition(i, j);

            //draw the actual tree
            Texture2D treeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroom").Value;

            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //draw the tree tops
            if (Framing.GetTileSafely(i, j).TileFrameX == 16)
            {
                Lighting.AddLight(new Vector2(i * 16, (j - 5) * 16), 0.45f, 0.25f, 0.45f);

                Texture2D topTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomTop").Value;
                Texture2D capTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomTopCap").Value;

                Vector2 treeOffset = new Vector2(48, 70);
                Vector2 capOffset = new Vector2(49, 66);

                DrawTreestuff(i - 1, j - 1, topTex, new Rectangle(0, 0, 112, 74), default, TileOffset.ToWorldCoordinates(), treeOffset, false);
                DrawTreestuff(i - 1, j - 1, capTex, new Rectangle(0, 0, 112, 74), default, TileOffset.ToWorldCoordinates(), capOffset, true);
            }

            //left side thing
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 18)
            {
                Texture2D sideTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomSide1").Value;

                Vector2 offset = new Vector2(6, 0);

                DrawTreestuff(i - 1, j - 1, sideTex, new Rectangle(0, 0, 14, 16), default, TileOffset.ToWorldCoordinates(), offset, false);
            }

            //right side thing
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 36)
            {
                Texture2D sideTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Tree/GiantShroomSide2").Value;

                Vector2 offset = new Vector2(-10, 0);

                DrawTreestuff(i - 1, j - 1, sideTex, new Rectangle(0, 0, 14, 18), default, TileOffset.ToWorldCoordinates(), offset, false);
            }

            return false;
        }
    }
}