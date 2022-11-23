using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyHell;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
    internal class EyeTree : ModTile
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
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eye Stalk");
            AddMapEntry(new Color(86, 2, 28), name);
            DustType = DustID.Blood;
			HitSound = SoundID.NPCHit13;
            ItemDrop = ModContent.ItemType<LivingFleshItem>();
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

        public static bool Spawn(int i, int j, int type = -1, UnifiedRandom random = null, int minSize = 5, 
        int maxSize = 18, bool leaves = false, int leavesType = -1, bool saplingExists = false)
        {
            if (type == -1) 
            {
                type = ModContent.TileType<EyeTree>();
            }

            if (random == null)
            {
                random = Main.rand;
            }

            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);
            }

            int height = random.Next(minSize, maxSize); //Height & trunk
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
                    WorldGen.PlaceTile(i + k, j, type, true);
                }
                else
                {
                    continue;
                }

                Framing.GetTileSafely(i + k, j).TileFrameX = (short)((k + 2) * 18);
                Framing.GetTileSafely(i + k, j).TileFrameY = (short)(random.Next(3) * 18);

                if (!extraPlaces[1] && !extraPlaces[3] && k == 0) 
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 0;
                }
            }

            for (int k = 1; k < height; k++)
            {
                WorldGen.PlaceTile(i, j - k, type, true);
                Framing.GetTileSafely(i, j - k).TileFrameX = 0;
                Framing.GetTileSafely(i, j - k).TileFrameY = (short)(random.Next(3) * 18);

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

        public override bool Drop(int i, int j)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 16)
            {
                //drop living flesh blocks
                int totalFlesh = Main.rand.Next(5, 10);
                for (int numFlesh = 0; numFlesh < totalFlesh; numFlesh++)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ModContent.ItemType<LivingFleshItem>(), Main.rand.Next(1, 5));
                }

                //drop this trees seeds
                int totalSeeds = Main.rand.Next(1, 3);
                for (int numSeed = 0; numSeed < totalSeeds; numSeed++)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ModContent.ItemType<EyeSeed>(), Main.rand.Next(1, 5));
                }
            }

            return true;
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
                for (int k = 0; k < WorldGen.numTreeShakes; k++)
				{
                    if (WorldGen.treeShakeX[k] == x && WorldGen.treeShakeY[k] == y)
					{
                        return;
					}
				}

                WorldGen.treeShakeX[WorldGen.numTreeShakes] = x;
                WorldGen.treeShakeY[WorldGen.numTreeShakes] = y;
                WorldGen.numTreeShakes++;

                WeightedRandom<int> random = new WeightedRandom<int>(Main.rand);

                random.Add(0, 1);
                random.Add(1, 0.7f);
                random.Add(2, 0.85f);

                int rand = random;

                //spawn an item out of the tree
                if (rand == 5)
                {
                    Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<EyeFruit>(), Main.rand.Next(1, 4));
                }

				//spawn eye bats out of the tree sometimes
                if (rand == 15)
                {
                    for (int numNPCs = 0; numNPCs < Main.rand.Next(1, 2); numNPCs++)
					{
                        NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), x * 16, y * 16, ModContent.NPCType<EyeBat>());
					}
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

            if (tile.TileFrameX == 16)
            {
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                //spawn gores out of the tree
                for (int numGores = 0; numGores <= Main.rand.Next(8, 15); numGores++)
                {
                    Gore.NewGore(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), (new Vector2(i, j - 2) * 16),
                    new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore" + Main.rand.Next(3)).Type);
                }
            }

            if (tile.TileFrameX == 36)
            {
                SoundEngine.PlaySound(SoundID.NPCHit20, (new Vector2(i, j) * 16));

                Gore.NewGore(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), (new Vector2(i, j - 2) * 16),
                new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)), ModContent.Find<ModGore>("Spooky/EyeTreeGore3").Type);
            }
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        internal static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
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

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 16)
            {
                Texture2D topsTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTops").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(118, 104); //new Vector2(248, 230);

                //draw tree tops
                DrawTreeTop(i - 1, j - 1, topsTex, new Rectangle(254 * frame, 0, 252, 108), 
                TileOffset.ToWorldCoordinates(), treeOffset, false);
            }

            Texture2D treeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTree").Value;

            Vector2 treeNormalOffset = new Vector2(0, 0); //new Vector2(130, 126);

            //draw the actual tree
            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);

            return false; 
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
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

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 16)
            {
                Texture2D topsTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTopsGlow").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(118, 104); //new Vector2(248, 230);

                //draw tree tops
                DrawTreeTop(i - 1, j - 1, topsTex, new Rectangle(254 * frame, 0, 252, 108), 
                TileOffset.ToWorldCoordinates(), treeOffset, true);
            }

            Texture2D treeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeGlow").Value;

            Vector2 treeNormalOffset = new Vector2(0, 0); //new Vector2(130, 126);

            //draw the actual tree
            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            Color.White, 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);
        }
    }
}