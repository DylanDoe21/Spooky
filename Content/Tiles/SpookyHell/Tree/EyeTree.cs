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
                r = Main.rand;

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
                return false;

            bool[] extraPlaces = new bool[5];
            for (int k = -2; k < 3; ++k) //Checks base
            {
                extraPlaces[k + 2] = false;
                if ((CustomTreeUtil.SolidTopTile(i + k, j + 1) || CustomTreeUtil.SolidTile(i + k, j + 1)) && !Framing.GetTileSafely(i + k, j).HasTile)
                    extraPlaces[k + 2] = true;
            }

            if (!extraPlaces[1]) extraPlaces[0] = false;
            if (!extraPlaces[3]) extraPlaces[4] = false;

            if (!extraPlaces[2]) return false;

            if (j > Main.worldSurface) //Base only exists on surface
                extraPlaces = new bool[5] { false, false, true, false, false };

            for (int k = -2; k < 3; ++k) //Places base
            {
                if (extraPlaces[k + 2])
                    WorldGen.PlaceTile(i + k, j, type, true);
                else
                    continue;

                Framing.GetTileSafely(i + k, j).TileFrameX = (short)((k + 2) * 18);
                Framing.GetTileSafely(i + k, j).TileFrameY = (short)(r.Next(3) * 18);

                if (!extraPlaces[0] && k == -1)
                    Framing.GetTileSafely(i + k, j).TileFrameX = 216;
                if (!extraPlaces[3] && k == 1)
                    Framing.GetTileSafely(i + k, j).TileFrameX = 234;

                if (!extraPlaces[1] && !extraPlaces[3] && k == 0) 
                    Framing.GetTileSafely(i + k, j).TileFrameX = 90;
                if (extraPlaces[1] && !extraPlaces[3] && k == 0) 
                    Framing.GetTileSafely(i + k, j).TileFrameX = 252;
                if (!extraPlaces[1] && extraPlaces[3] && k == 0) 
                    Framing.GetTileSafely(i + k, j).TileFrameX = 270;
            }

            for (int k = 1; k < height; ++k)
            {
                WorldGen.PlaceTile(i, j - k, type, true);
                Framing.GetTileSafely(i, j - k).TileFrameX = 90;
                Framing.GetTileSafely(i, j - k).TileFrameY = (short)(r.Next(3) * 18);

                if (k == height - 1)
                {
                    if (r.NextBool(12)) 
                        Framing.GetTileSafely(i, j - k).TileFrameX = 180;
                    else 
                        Framing.GetTileSafely(i, j - k).TileFrameX = 198;
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

                if (leaves)
                {
                    if (r.Next(4) <= 1)
                    {
                        int rnd = r.Next(2, 5);
                        for (int l = 0; l < rnd; ++l)
                            Gore.NewGore(Entity.GetSource_NaturalSpawn(), (new Vector2(i, j - k) * 16) + new Vector2(8 + r.Next(-4, 5), 8), new Vector2(Main.rand.NextFloat(3), Main.rand.NextFloat(-5, 5)), leavesType);
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
                int tot = Main.rand.Next(6, 11);
                for (int k = 0; k < tot; ++k)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ModContent.ItemType<LivingFleshItem>(), Main.rand.Next(1, 5));
                }

                tot = Main.rand.Next(1, 4);
                for (int k = 0; k < tot; ++k)
                {
                    Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(-46, 46), 
                    Main.rand.Next(-40, 40) - 66), ItemID.Acorn, Main.rand.Next(1, 5));
                }
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 108 || Framing.GetTileSafely(i, j).TileFrameX == 126)
            {
                int side = Framing.GetTileSafely(i, j).TileFrameX == 108 ? -1 : 1;

                Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16) + new Vector2(Main.rand.Next(40) * side, Main.rand.Next(-10, 10)), ItemID.Acorn, Main.rand.Next(1, 3));
            }

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

                /*
				//tree shake stuff, will probably be useful for later
                int rand = random;

				//spawn an item out of the tree
                if (rand == 1)
                {
                    Item.NewItem(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), (new Vector2(x, y) * 16) + new Vector2(Main.rand.Next(-56, 56), 
					Main.rand.Next(-44, 44) - 66), ModContent.ItemType<EyeFruit>(), Main.rand.Next(1, 4));
                }

				//spawn an npc out of the tree
                else if (rand == 2)
                {
                    for (int numNPCs = 0; numNPCs < Main.rand.Next(1, 2); numNPCs++)
					{
                        NPC.NewNPC(new EntitySource_TileInteraction(Main.LocalPlayer, x, y), x * 16, y * 16, ModContent.NPCType<EyeBat>());
					}
                }

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
            Tile t = Framing.GetTileSafely(i, j);

            if (fail && !effectOnly && !noItem)
            {
                (int x, int y) = (i, j);
                CheckEntireTree(ref x, ref y);
            }

            if (fail)
                return;

            //if you value your sanity don't bother reading this

            int[] rootFrames = new int[] { 0, 18, 54, 72, 216, 234, 108, 126 };
            if (CustomTreeUtil.ActiveType(i, j - 1, Type) && !rootFrames.Contains(t.TileFrameX))
                WorldGen.KillTile(i, j - 1, fail, false, false);

            if (t.TileFrameX == 0 && CustomTreeUtil.ActiveType(i + 1, j, Type)) //Leftmost root
                Framing.GetTileSafely(i + 1, j).TileFrameX = 216;
            else if (t.TileFrameX == 72 && CustomTreeUtil.ActiveType(i - 1, j, Type)) //Rightmost root
                Framing.GetTileSafely(i - 1, j).TileFrameX = 234;
            else if (t.TileFrameX == 18 || t.TileFrameX == 216) //Left root & cut left root
            {
                if (t.TileFrameX == 18)
                    WorldGen.KillTile(i - 1, j, fail, false, false);

                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 36)
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 270;
                else if (Framing.GetTileSafely(i + 1, j).TileFrameX == 252)
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
            }
            else if (t.TileFrameX == 54 || t.TileFrameX == 234) //Right root & cut right root
            {
                if (t.TileFrameX == 54)
                    WorldGen.KillTile(i + 1, j, fail, false, false);

                if (Framing.GetTileSafely(i - 1, j).TileFrameX == 36)
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 252;
                else if (Framing.GetTileSafely(i - 1, j).TileFrameX == 270)
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 90;
            }
            else if (t.TileFrameX == 36)
            {
                WorldGen.KillTile(i - 1, j, false, false, false);
                WorldGen.KillTile(i + 1, j, false, false, false);
            }
            else if (t.TileFrameX == 90 || t.TileFrameX == 144 || t.TileFrameX == 162 || t.TileFrameX == 180 || t.TileFrameX == 198 || t.TileFrameX == 288 || t.TileFrameX == 306 || t.TileFrameX == 324) //Main tree cut
            {
                int nFrameX = Framing.GetTileSafely(i, j + 1).TileFrameX;
                if (nFrameX == 90) Framing.GetTileSafely(i, j + 1).TileFrameX = 288;
                if (t.TileFrameX == 144) //right branch
                {
                    WorldGen.KillTile(i + 1, j, fail);
                    Framing.GetTileSafely(i, j + 1).TileFrameX = 306;
                }
                if (nFrameX == 162) Framing.GetTileSafely(i, j + 1).TileFrameX = 324;
            }
            else if (t.TileFrameX == 252)
                WorldGen.KillTile(i - 1, j, fail, false, false);
            else if (t.TileFrameX == 270)
                WorldGen.KillTile(i + 1, j, fail, false, false);
            else if (t.TileFrameX == 108)
            {
                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 162)
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
                else
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 324;
            }
            else if (t.TileFrameX == 126)
            {
                if (Framing.GetTileSafely(i + 1, j).TileFrameX == 144)
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 90;
                else
                    Framing.GetTileSafely(i + 1, j).TileFrameX = 306;
            }

            if (!fail && Framing.GetTileSafely(i, j + 2).HasTile && !Framing.GetTileSafely(i - 1, j - 1).HasTile && !Framing.GetTileSafely(i + 1, j - 1).HasTile)
            {
                if (Framing.GetTileSafely(i - 1, j + 1).TileType == Type)
                    Framing.GetTileSafely(i - 1, j).TileFrameX = 180;
                else
                    WorldGen.KillTile(i - 1, j);
            }
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile t = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;
            if (xOff == 1 && (j / 4f) == 0)
                xOff = 0;

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;
            if (t.TileFrameX == 108 || t.TileFrameX < 36 || t.TileFrameX == 216 || t.TileFrameX == 270) frameSize = 18;
            if (t.TileFrameX == 126 || t.TileFrameX == 52 || t.TileFrameX == 72 || t.TileFrameX == 232 || t.TileFrameX == 252)
            {
                frameSize = 18;
                frameOff = -2;
            }
            if (t.TileFrameX < 90 || t.TileFrameX == 216 || t.TileFrameX == 234 || t.TileFrameX == 252 || t.TileFrameX == 270) frameSizeY = 18;

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = CustomTreeUtil.TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 108) //Draw branches so it has to do less logic later
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranches").Value;
                int frame = t.TileFrameY / 18;
                spriteBatch.Draw(tops, pos, new Rectangle(0, 52 * frame, 56, 50), new Color(col.R, col.G, col.B, 255), 0f, new Vector2(38, 16), 1f, SpriteEffects.None, 0f);
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 126) //Draw branches so it has to do less logic later
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranches").Value;
                int frame = t.TileFrameY / 18;
                spriteBatch.Draw(tops, pos, new Rectangle(58, 52 * frame, 56, 50), new Color(col.R, col.G, col.B, 255), 0f, new Vector2(4, 16), 1f, SpriteEffects.None, 0f);
            }

			if (Framing.GetTileSafely(i, j).TileFrameX == 198)
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTops").Value;
                int frame = t.TileFrameY / 18;

                CustomTreeUtil.DrawTreeTop(i - 1, j - 1, tops, new Rectangle(222 * frame, 0, 220, 108), CustomTreeUtil.TileOffset.ToWorldCoordinates(), new Vector2(120, 106));
            }

            spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTree").Value, pos, new Rectangle(t.TileFrameX + frameOff, t.TileFrameY, frameSize, frameSizeY), 
            new Color(col.R, col.G, col.B, 255), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile t = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;
            if (xOff == 1 && (j / 4f) == 0)
                xOff = 0;

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;
            if (t.TileFrameX == 108 || t.TileFrameX < 36 || t.TileFrameX == 216 || t.TileFrameX == 270) frameSize = 18;
            if (t.TileFrameX == 126 || t.TileFrameX == 52 || t.TileFrameX == 72 || t.TileFrameX == 232 || t.TileFrameX == 252)
            {
                frameSize = 18;
                frameOff = -2;
            }
            if (t.TileFrameX < 90 || t.TileFrameX == 216 || t.TileFrameX == 234 || t.TileFrameX == 252 || t.TileFrameX == 270) frameSizeY = 18;

            Vector2 offset = new((xOff * 2) - (frameOff / 2), 0);
            Vector2 pos = CustomTreeUtil.TileCustomPosition(i, j) - offset;

            if (Framing.GetTileSafely(i, j).TileFrameX == 108) //Draw branches so it has to do less logic later
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranchesGlow").Value;
                int frame = t.TileFrameY / 18;
                spriteBatch.Draw(tops, pos, new Rectangle(0, 52 * frame, 56, 50), Color.White, 0f, new Vector2(38, 16), 1f, SpriteEffects.None, 0f);
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 126) //Draw branches so it has to do less logic later
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeBranchesGlow").Value;
                int frame = t.TileFrameY / 18;
                spriteBatch.Draw(tops, pos, new Rectangle(58, 52 * frame, 56, 50), Color.White, 0f, new Vector2(4, 16), 1f, SpriteEffects.None, 0f);
            }

			if (Framing.GetTileSafely(i, j).TileFrameX == 198)
            {
                Texture2D tops = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeTopsGlow").Value;
                int frame = t.TileFrameY / 18;

                CustomTreeUtil.DrawTreeTopGlow(i - 1, j - 1, tops, new Rectangle(222 * frame, 0, 220, 108), CustomTreeUtil.TileOffset.ToWorldCoordinates(), new Vector2(120, 106));
            }

            spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Tree/EyeTreeGlow").Value, pos, 
            new Rectangle(t.TileFrameX + frameOff, t.TileFrameY, frameSize, frameSizeY), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
        }
    }

	public class CustomTreeUtil
    {
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;

        public static void Set(ModTile t, int minPick, int dustType, SoundStyle soundType, Color mapColor, int drop, string mapName = "")
        {
            t.MinPick = minPick;
            t.DustType = dustType;
            t.HitSound = soundType;
            t.ItemDrop = drop;

            ModTranslation name = t.CreateMapEntryName();
            name.SetDefault(mapName);
            t.AddMapEntry(mapColor, name);
        }

        public static void SetProperties(ModTile t, bool solid, bool mergeDirt, bool lighted, bool blockLight)
        {
            Main.tileMergeDirt[t.Type] = mergeDirt;
            Main.tileSolid[t.Type] = solid;
            Main.tileLighted[t.Type] = lighted;
            Main.tileBlockLight[t.Type] = blockLight;
        }

        public static void SetAll(ModTile t, int minPick, int dust, SoundStyle sound, Color mapColour, int drop = 0, string mapName = "", bool solid = true, bool mergeDirt = true, bool lighted = true, bool blockLight = true)
        {
            Set(t, minPick, dust, sound, mapColour, drop, mapName);
            SetProperties(t, solid, mergeDirt, lighted, blockLight);
        }

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0));
        }

        internal static void DrawTreeTop(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Tile tile = Main.tile[i, j];
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? Vector2.Zero);
            Color col = Lighting.GetColor(i, j);

            if (tile.TileColor == 31)
                col = Color.White;

            Main.spriteBatch.Draw(tex, drawPos, source, col, 0, origin ?? source.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        internal static void DrawTreeTopGlow(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
        {
            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? Vector2.Zero);

            Main.spriteBatch.Draw(tex, drawPos, source, Color.White, 0, origin ?? source.Value.Size() / 2f, 1f, SpriteEffects.None, 0f);
        }

        public static bool SolidTile(int i, int j) => Framing.GetTileSafely(i, j).HasTile && Main.tileSolid[Framing.GetTileSafely(i, j).TileType];
        public static bool SolidTopTile(int i, int j) => Framing.GetTileSafely(i, j).HasTile && (Main.tileSolidTop[Framing.GetTileSafely(i, j).TileType] || Main.tileSolid[Framing.GetTileSafely(i, j).TileType]);
        public static bool ActiveType(int i, int j, int t) => Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == t;
        public static bool SolidType(int i, int j, int t) => ActiveType(i, j, t) && Framing.GetTileSafely(i, j).HasTile;
        public static bool ActiveTypeNoTopSlope(int i, int j, int t) => Framing.GetTileSafely(i, j).HasTile && Framing.GetTileSafely(i, j).TileType == t && !Framing.GetTileSafely(i, j).TopSlope;
    }
}
