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

using Spooky.Content.NPCs.SpookyHell;

namespace Spooky.Content.Tiles.SpookyHell.Tree
{
    internal class EyeTree : ModTile
    {
        public override void SetStaticDefaults()
        {
            CustomTreeUtil.SetAll(this, 0, DustID.Blood, SoundID.Dig, new Color(86, 2, 28), ModContent.ItemType<LivingFleshItem>(), "Tree", false, false, false, false);
            TileID.Sets.IsATreeTrunk[Type] = true;
			Main.tileFrameImportant[Type] = true;
            Main.tileAxe[Type] = true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = (fail ? 1 : 3);
		}

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            resetFrame = false;
            noBreak = true;
            return false;
        }

        public static bool Spawn(int i, int j, int type = -1, UnifiedRandom r = null, int minSize = 5, 
        int maxSize = 18, bool leaves = false, int leavesType = -1, bool saplingExists = false)
        {
            if (type == -1) type = ModContent.TileType<EyeTree>(); //Sets default types

            if (r == null) //For use in worldgen instead of Main.rand
            {
                r = Main.rand;
            }

            if (saplingExists)
            {
                WorldGen.KillTile(i, j, false, false, true);
                WorldGen.KillTile(i, j - 1, false, false, true);
            }

            int height = r.Next(minSize, maxSize); //Height & trunk
            for (int k = 1; k < height; ++k)
            {
                if (CustomTreeUtil.SolidTile(i, j - k))
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
            for (int k = -2; k < 3; ++k) //Checks base
            {
                extraPlaces[k + 2] = false;

                if ((CustomTreeUtil.SolidTopTile(i + k, j + 1) || CustomTreeUtil.SolidTile(i + k, j + 1)) && !Framing.GetTileSafely(i + k, j).HasTile)
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

            for (int k = -2; k < 3; ++k) //Places base
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
                Framing.GetTileSafely(i + k, j).TileFrameY = (short)(r.Next(3) * 18);

                if (!extraPlaces[0] && k == -1)
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 216;
                }

                if (!extraPlaces[3] && k == 1)
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 234;
                }

                if (!extraPlaces[1] && !extraPlaces[3] && k == 0) 
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 90;
                }

                if (extraPlaces[1] && !extraPlaces[3] && k == 0) 
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 252;
                }

                if (!extraPlaces[1] && extraPlaces[3] && k == 0) 
                {
                    Framing.GetTileSafely(i + k, j).TileFrameX = 270;
                }
            }

            for (int k = 1; k < height; ++k)
            {
                WorldGen.PlaceTile(i, j - k, type, true);
                Framing.GetTileSafely(i, j - k).TileFrameX = 90;
                Framing.GetTileSafely(i, j - k).TileFrameY = (short)(r.Next(3) * 18);

                if (k == height - 1)
                {
                    if (r.NextBool(12)) 
                    {
                        Framing.GetTileSafely(i, j - k).TileFrameX = 180;
                    }
                    else 
                    {
                        Framing.GetTileSafely(i, j - k).TileFrameX = 198;
                    }
                }
                else if (r.NextBool(4))
                {
                    int side = r.Next(2);

                    if (side == 0 && !Framing.GetTileSafely(i - 1, j - k).HasTile)
                    {
                        WorldGen.PlaceTile(i - 1, j - k, type, true);
                        Framing.GetTileSafely(i, j - k).TileFrameX = 162;
                        Framing.GetTileSafely(i - 1, j - k).TileFrameX = 108;
                        Framing.GetTileSafely(i - 1, j - k).TileFrameY = (short)(r.Next(3) * 18);
                    }

                    else if (side == 1 && !Framing.GetTileSafely(i + 1, j - k).HasTile)
                    {
                        WorldGen.PlaceTile(i + 1, j - k, type, true);
                        Framing.GetTileSafely(i, j - k).TileFrameX = 144;
                        Framing.GetTileSafely(i + 1, j - k).TileFrameX = 126;
                        Framing.GetTileSafely(i + 1, j - k).TileFrameY = (short)(r.Next(3) * 18);
                    }
                }
            }

            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Framing.GetTileSafely(i, j + 1).HasTile && !Framing.GetTileSafely(i - 1, j).HasTile && !Framing.GetTileSafely(i + 1, j).HasTile)
            {
                WorldGen.KillTile(i, j, false, false, false);
            }
        }

        public override bool Drop(int i, int j)
        {
            if (Framing.GetTileSafely(i, j).TileFrameX == 198)
            {
                //drop living flesh blocks
                int totalFlesh = Main.rand.Next(6, 11);
                for (int numFlesh = 0; numFlesh < totalFlesh; numFlesh++)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ModContent.ItemType<LivingFleshItem>(), Main.rand.Next(1, 5));
                }

                //drop this trees acorns
                int totalSeeds = Main.rand.Next(1, 2);
                for (int numSeed = 0; numSeed < totalSeeds; numSeed++)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ModContent.ItemType<EyeSeed>(), Main.rand.Next(1, 5));
                }
            }

            /*
            if (Framing.GetTileSafely(i, j).TileFrameX == 108 || Framing.GetTileSafely(i, j).TileFrameX == 126)
            {
                int side = Framing.GetTileSafely(i, j).TileFrameX == 108 ? -1 : 1;

                Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(40) * side, Main.rand.Next(-10, 10)), ModContent.ItemType<EyeSeed>(), Main.rand.Next(1, 3));
            }
            */

            return true;
        }

        private void CheckEntireTree(ref int x, ref int y)
        {
            while (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == Type)
			{
                y--;
			}

            y++;

            if (Main.tile[x, y].TileFrameX == 198)
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

				//tree shake stuff, will probably be useful for later

                /*
				//spawn an item out of the tree
                if (rand == 1)
                {
                    Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<EyeFruit>(), Main.rand.Next(1, 4));
                }
                */
				//spawn an npc out of the tree, in this case eye bats
                if (rand == 2)
                {
                    for (int numNPCs = 0; numNPCs < Main.rand.Next(1, 2); numNPCs++)
					{
                        NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), x * 16, y * 16, ModContent.NPCType<EyeBat>());
					}
                }
                /*
				//spawn gores out of the tree
                for (int numGores = 0; numGores < 20; numGores++)
				{
                    Gore.NewGore(new EntitySource_TileUpdate(x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), new Vector2(Main.rand.NextFloat(3), Main.rand.NextFloat(-5, 5)), Mod.Find<ModGore>("LushLeaf").Type);
				}
                */
            }
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

            //tree break code, this is really confusing so i just copied it
            int[] rootFrames = new int[] { 0, 18, 54, 72, 216, 234, 108, 126 };
            if (CustomTreeUtil.ActiveType(i, j - 1, Type) && !rootFrames.Contains(tile.TileFrameX))
            {
                WorldGen.KillTile(i, j - 1, fail, false, false);
            }

            if (tile.TileFrameX == 0 && CustomTreeUtil.ActiveType(i + 1, j, Type)) //Leftmost root
            {
                Framing.GetTileSafely(i + 1, j).TileFrameX = 216;
            }
            else if (tile.TileFrameX == 72 && CustomTreeUtil.ActiveType(i - 1, j, Type)) //Rightmost root
            {
                Framing.GetTileSafely(i - 1, j).TileFrameX = 234;
            }
            else if (tile.TileFrameX == 18 || tile.TileFrameX == 216) //Left root & cut left root
            {
                if (tile.TileFrameX == 18)
                {
                    WorldGen.KillTile(i - 1, j, fail, false, false);
                }

                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 36)
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 270;
                }
                else if (Framing.GetTileSafely(i + 1, j).TileFrameX == 252)
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
                }
            }
            else if (tile.TileFrameX == 54 || tile.TileFrameX == 234) //Right root & cut right root
            {
                if (tile.TileFrameX == 54 || tile.TileFrameX == 234)
                {
                    WorldGen.KillTile(i + 1, j, fail, false, false);
                }

                if (Framing.GetTileSafely(i - 1, j).TileFrameX == 36)
                {
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 252;
                }

                else if (Framing.GetTileSafely(i - 1, j).TileFrameX == 270)
                {
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 90;
                }
            }
            else if (tile.TileFrameX == 36)
            {
                WorldGen.KillTile(i - 1, j, false, false, false);
                WorldGen.KillTile(i + 1, j, false, false, false);
            }

            else if (tile.TileFrameX == 90 || tile.TileFrameX == 144 || tile.TileFrameX == 162 || tile.TileFrameX == 180 || 
            tile.TileFrameX == 198 || tile.TileFrameX == 288 || tile.TileFrameX == 306 || tile.TileFrameX == 324) //Main tree cut
            {
                int nFrameX = Framing.GetTileSafely(i, j + 1).TileFrameX;

                if (nFrameX == 90) 
                {
                    Framing.GetTileSafely(i, j + 1).TileFrameX = 288;
                }

                if (tile.TileFrameX == 144) //right branch
                {
                    WorldGen.KillTile(i + 1, j, fail);
                    Framing.GetTileSafely(i, j + 1).TileFrameX = 306;
                }

                if (nFrameX == 162) 
                {
                    Framing.GetTileSafely(i, j + 1).TileFrameX = 324;
                }
            }
            else if (tile.TileFrameX == 252)
            {
                WorldGen.KillTile(i - 1, j, fail, false, false);
            }
            else if (tile.TileFrameX == 270)
            {
                WorldGen.KillTile(i + 1, j, fail, false, false);
            }
            else if (tile.TileFrameX == 108)
            {
                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 162)
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
                }
                else
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 324;
                }
            }
            else if (tile.TileFrameX == 126)
            {
                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 144)
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
                }
                else
                {
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 306;
                }
            }

            if (!fail && Framing.GetTileSafely(i, j + 2).HasTile && !Framing.GetTileSafely(i - 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 1).HasTile)
            {
                if (Framing.GetTileSafely(i - 1, j + 1).TileType == Type)
                {
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 180;
                }
                else
                {
                    WorldGen.KillTile(i - 1, j);
                }
            }
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
            
            if (tile.TileFrameX == 108 || tile.TileFrameX < 36 || tile.TileFrameX == 216 || tile.TileFrameX == 270) 
            {
                frameSize = 18;
            }
            if (tile.TileFrameX == 126 || tile.TileFrameX == 52 || tile.TileFrameX == 72 || tile.TileFrameX == 232 || tile.TileFrameX == 252)
            {
                frameSize = 18;
                frameOff = -2;
            }
            if (tile.TileFrameX < 90 || tile.TileFrameX == 216 || tile.TileFrameX == 234 || tile.TileFrameX == 252 || tile.TileFrameX == 270) 
            {
                frameSizeY = 18;
            }

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = CustomTreeUtil.TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 108)
            {
                Texture2D branchTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranches").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(38, 16); //new Vector2(166, 144);

                //draw the branches
                spriteBatch.Draw(branchTex, pos, new Rectangle(0, 52 * frame, 56, 50), new Color(col.R, col.G, col.B, 255), 0f, treeOffset, 1f, SpriteEffects.None, 0f);
            
                return false;
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 126)
            {
                Texture2D branchTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranches").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(4, 16); //new Vector2(132, 144);

                //draw the branches
                spriteBatch.Draw(branchTex, pos, new Rectangle(58, 52 * frame, 56, 50), new Color(col.R, col.G, col.B, 255), 0f, treeOffset, 1f, SpriteEffects.None, 0f);
            
                return false;
            }

			if (Framing.GetTileSafely(i, j).TileFrameX == 198)
            {
                Texture2D topsTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTops").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(118, 104); //new Vector2(248, 230);

                //draw tree tops
                CustomTreeUtil.DrawTreeTop(i - 1, j - 1, topsTex, new Rectangle(222 * frame, 0, 220, 108), 
                CustomTreeUtil.TileOffset.ToWorldCoordinates(), treeOffset, false);
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
            
            if (tile.TileFrameX == 108 || tile.TileFrameX < 36 || tile.TileFrameX == 216 || tile.TileFrameX == 270) 
            {
                frameSize = 18;
            }
            if (tile.TileFrameX == 126 || tile.TileFrameX == 52 || tile.TileFrameX == 72 || tile.TileFrameX == 232 || tile.TileFrameX == 252)
            {
                frameSize = 18;
                frameOff = -2;
            }
            if (tile.TileFrameX < 90 || tile.TileFrameX == 216 || tile.TileFrameX == 234 || tile.TileFrameX == 252 || tile.TileFrameX == 270) 
            {
                frameSizeY = 18;
            }

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = CustomTreeUtil.TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 108)
            {
                Texture2D branchTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranchesGlow").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(38, 16); //new Vector2(166, 144);

                //draw the branches
                spriteBatch.Draw(branchTex, pos, new Rectangle(0, 52 * frame, 56, 50), Color.White, 0f, treeOffset, 1f, SpriteEffects.None, 0f);
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 126)
            {
                Texture2D branchTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranchesGlow").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(4, 16); //new Vector2(132, 144);

                //draw the branches
                spriteBatch.Draw(branchTex, pos, new Rectangle(58, 52 * frame, 56, 50), Color.White, 0f, treeOffset, 1f, SpriteEffects.None, 0f);
            }

			if (Framing.GetTileSafely(i, j).TileFrameX == 198)
            {
                Texture2D topsTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTopsGlow").Value;
                int frame = tile.TileFrameY / 18;

                Vector2 treeOffset = new Vector2(118, 104); //new Vector2(248, 230);

                //draw tree tops
                CustomTreeUtil.DrawTreeTop(i - 1, j - 1, topsTex, new Rectangle(222 * frame, 0, 220, 108), 
                CustomTreeUtil.TileOffset.ToWorldCoordinates(), treeOffset, true);
            }

            Texture2D treeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeGlow").Value;

            Vector2 treeNormalOffset = new Vector2(0, 0); //new Vector2(130, 126);

            //draw the actual tree
            spriteBatch.Draw(treeTex, pos, new Rectangle(tile.TileFrameX + frameOff, tile.TileFrameY, frameSize, frameSizeY), 
            Color.White, 0f, treeNormalOffset, 1f, SpriteEffects.None, 0f);
        }
    }

	public class CustomTreeUtil
    {
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static void Set(ModTile tile, int minPick, int dustType, SoundStyle soundType, Color mapColor, int drop, string mapName = "")
        {
            tile.MinPick = minPick;
            tile.DustType = dustType;
            tile.HitSound = soundType;
            tile.ItemDrop = drop;

            ModTranslation name = tile.CreateMapEntryName();
            name.SetDefault(mapName);
            tile.AddMapEntry(mapColor, name);
        }

        public static void SetProperties(ModTile tile, bool solid, bool mergeDirt, bool lighted, bool blockLight)
        {
            Main.tileMergeDirt[tile.Type] = mergeDirt;
            Main.tileSolid[tile.Type] = solid;
            Main.tileLighted[tile.Type] = lighted;
            Main.tileBlockLight[tile.Type] = blockLight;
        }

        public static void SetAll(ModTile tile, int minPick, int dust, SoundStyle sound, Color mapColour, int drop = 0, 
        string mapName = "", bool solid = true, bool mergeDirt = true, bool lighted = true, bool blockLight = true)
        {
            Set(tile, minPick, dust, sound, mapColour, drop, mapName);
            SetProperties(tile, solid, mergeDirt, lighted, blockLight);
        }

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0));
        }

        internal static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null, bool Glow = false)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? Vector2.Zero);
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, Glow ? Color.White : color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
        }

        public static bool SolidTile(int i, int j) => Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        public static bool SolidTopTile(int i, int j) => Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        public static bool ActiveType(int i, int j, int t) => Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == t;
        public static bool SolidType(int i, int j, int t) => ActiveType(i, j, t) && Framing.GetTileSafely(i, j).HasTile;
        public static bool ActiveTypeNoTopSlope(int i, int j, int t) => Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == t && !Framing.GetTileSafely(i, j).TopSlope;
    }
}