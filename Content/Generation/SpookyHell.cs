using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.NoseTemple;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Ambient;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.SpookyHell.Tree;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class SpookyHell : ModSystem
    {
        int NoseTempleBrickColor;
        int NoseTempleBrickWallColor;
        static int NoseTemplePositionY;
        static int NoseTempleEntranceTunnelX;

        static int StartPosition = (GenVars.JungleX < Main.maxTilesX / 2) ? 70 : Main.maxTilesX - (Main.maxTilesX / 6) - 80;
        static int BiomeEdge = StartPosition + (Main.maxTilesX / 6);

        private void GenerateSpookyHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.EyeValley").Value;

            //set these to their intended values again just to be safe
            StartPosition = (GenVars.JungleX < Main.maxTilesX / 2) ? 70 : Main.maxTilesX - (Main.maxTilesX / 6) - 80;
            BiomeEdge = StartPosition + (Main.maxTilesX / 6);

            //extra clear width depending on the side of the world its on
            int extraClearStart = (GenVars.JungleX < Main.maxTilesX / 2) ? 50 : 0;
            int extraClearEnd = (GenVars.JungleX > Main.maxTilesX / 2) ? 50 : 0;

            //clear everything in the area the biome generates in to prevent unwanted collisions with obsidian houses or lava flooding
            for (int X = StartPosition - extraClearStart; X <= BiomeEdge + extraClearEnd; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y < Main.maxTilesY - 15; Y++)
                {
                    Tile tile = Main.tile[X, Y];

                    tile.ClearEverything();
                    WorldGen.KillWall(X, Y);
                }
            }

            //generate the surface
            int width = BiomeEdge;
            int height = Main.maxTilesY - 150;

            int[] terrainContour = new int[width * height];

            double rand1 = WorldGen.genRand.NextDouble() + 1;
            double rand2 = WorldGen.genRand.NextDouble() + 2;
            double rand3 = WorldGen.genRand.NextDouble() + 3;

            float peakheight = 10;
            float flatness = 50;
            int offset = Main.maxTilesY - 130;

            //genrate a random wave
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                double BiomeHeight = peakheight / rand1 * Math.Sin((float)X / flatness * rand1 + rand1);
                BiomeHeight += peakheight / rand2 * Math.Sin((float)X / flatness * rand2 + rand2);
                BiomeHeight += peakheight / rand3 * Math.Sin((float)X / flatness * rand3 + rand3);

                BiomeHeight += offset;

                terrainContour[X] = (int)BiomeHeight;
            }

            //place the randomized wave of blocks, with walls behind them
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 6; Y++)
                {
                    if (Y > terrainContour[X] && WorldGen.InWorld(X, Y))
                    {
                        WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<SpookyMush>());
                        Main.tile[X, Y + 5].WallType = (ushort)ModContent.WallType<SpookyMushWall>();
                    }
                }
            }

            //place clumps of blocks along both edges of the biome so it transitions with the rest of the underworld more nicely
            for (int X = StartPosition - 50; X <= StartPosition; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }
            for (int X = BiomeEdge; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }

            //place ceiling of blocks across the top of the biome
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 192; Y++)
                {
                    if (WorldGen.genRand.NextBool(15))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), WorldGen.genRand.Next(5, 7), true, true);
                    }
                }
            }

            //roughen up the terrain so it looks more natural
            for (int roughPass = 0; roughPass < 2; roughPass++)
            {
                int lastMaxTileX = 0;
                int lastXRadius = 0;

                for (int i = StartPosition - 50; i <= BiomeEdge + 50; i++)
                {
                    // flat ellipses
                    if (WorldGen.genRand.NextBool(40) && i > lastMaxTileX + lastXRadius)
                    {
                        int roughingPosition = 0;
                        // Look for a Y position to put ellipses
                        for (int lookupY = Main.maxTilesY - 150; lookupY <= Main.maxTilesY - 130; lookupY++)
                        {
                            if (Framing.GetTileSafely(i, lookupY).HasTile)
                            {
                                roughingPosition = lookupY;
                                break;
                            }
                        }

                        int radiusX;
                        int radiusY;

                        //randomize the size of the craters that are dug
                        switch (roughPass)
                        {
                            case 0:
                            {
                                radiusX = WorldGen.genRand.Next(8, 18);
                                radiusY = WorldGen.genRand.Next(3, 6);
                                break;
                            }
                            case 1:
                            {
                                radiusX = WorldGen.genRand.Next(8, 15);
                                radiusY = WorldGen.genRand.Next(2, 4);
                                break;
                            }
                            default:
                            {
                                radiusX = 0;
                                radiusY = 0;
                                break;
                            }
                        }

                        int minTileX = i - radiusX;
                        int maxTileX = i + radiusX;
                        int minTileY = roughingPosition - radiusY;
                        int maxTileY = roughingPosition + radiusY;

                        int diameterX = Math.Abs(minTileX - maxTileX);
                        int diameterY = Math.Abs(minTileY - maxTileY);
                        float centerX = (minTileX + maxTileX - 1) / 2f;
                        float centerY = (minTileY + maxTileY - 1) / 2f;

                        //make the ellipse
                        for (int ellipseTileX = minTileX; ellipseTileX < maxTileX; ellipseTileX++)
                        {
                            for (int ellipseTileY = minTileY; ellipseTileY < maxTileY; ellipseTileY++)
                            {
                                if ((Math.Pow(ellipseTileX - centerX, 2) / Math.Pow(diameterX / 2, 2)) + (Math.Pow(ellipseTileY - centerY, 2) / Math.Pow(diameterY / 2, 2)) <= 1)
                                {
                                    if (ellipseTileX < Main.maxTilesX && ellipseTileY < Main.maxTilesY && ellipseTileX >= 0 && ellipseTileY >= 0)
                                    {
                                        Main.tile[ellipseTileX, ellipseTileY].ClearEverything();
                                        WorldGen.KillWall(ellipseTileX + 1, ellipseTileY);
                                        WorldGen.KillWall(ellipseTileX - 1, ellipseTileY);
                                        WorldGen.KillWall(ellipseTileX, ellipseTileY + 1);
                                        WorldGen.KillWall(ellipseTileX, ellipseTileY - 1);
                                    }
                                }
                            }
                            
                            for (int ellipseTileY = minTileY - 20; ellipseTileY < maxTileY - diameterY / 2; ellipseTileY++)
                            {
                                if (ellipseTileX < Main.maxTilesX && ellipseTileY < Main.maxTilesY && ellipseTileX >= 0 && ellipseTileY >= 0)
                                {
                                    Main.tile[ellipseTileX, ellipseTileY].ClearEverything();
                                    WorldGen.KillWall(ellipseTileX + 1, ellipseTileY);
                                    WorldGen.KillWall(ellipseTileX - 1, ellipseTileY);
                                    WorldGen.KillWall(ellipseTileX, ellipseTileY + 1);
                                    WorldGen.KillWall(ellipseTileX, ellipseTileY - 1);
                                }
                            }
                        }

                        lastMaxTileX = maxTileX;
                        lastXRadius = diameterX / 2;
                    }
                }
            }

            //erode edges of blocks to make it look more natural
            for (int clearPass = 0; clearPass < 2; clearPass++)
            {
                List<Point> list = new();
                for (int tileX = StartPosition - 50; tileX <= BiomeEdge + 50; tileX++)
                {
                    for (int tileY = Main.maxTilesY - 200; tileY <= Main.maxTilesY - 130; tileY++)
                    {
                        // check if there's a tile, so it won't check all the surrounding tiles for nothing
                        if (Main.tile[tileX, tileY].HasTile)
                        {
                            bool tilesBelow = Main.tile[tileX, tileY + 1].HasTile && Main.tile[tileX, tileY + 2].HasTile;
                            bool tilesAbove = Main.tile[tileX - 1, tileY - 1].HasTile && Main.tile[tileX, tileY - 1].HasTile && Main.tile[tileX + 1, tileY - 1].HasTile;
                            bool tilesRight = Main.tile[tileX + 1, tileY].HasTile && Main.tile[tileX + 1, tileY + 1].HasTile;
                            bool tilesLeft = Main.tile[tileX - 1, tileY].HasTile && Main.tile[tileX - 1, tileY + 1].HasTile;

                            if (tilesBelow && !tilesAbove && (tilesRight ^ tilesLeft))
                            {
                                list.Add(new Point(tileX, tileY));
                            }

                            break;
                        }
                    }
                }

                foreach (Point p in list)
                {
                    Main.tile[p.X, p.Y].ClearEverything();
                    WorldGen.KillWall(p.X + 1, p.Y);
                    WorldGen.KillWall(p.X - 1, p.Y);
                    WorldGen.KillWall(p.X, p.Y + 1);
                    WorldGen.KillWall(p.X, p.Y - 1);
                }
            }

            //place pillars of flesh walls with plateaus in them for more varied terrain
            for (int X = StartPosition + 70; X <= BiomeEdge - 70; X++)
            {
                if (WorldGen.genRand.NextBool(75))
                {
                    PlaceWallPillar(X);

                    X += 45;
                }
            }

            //place blocks at the edge of the biome where the nose miniboss arena will generate
            if (StartPosition < (Main.maxTilesX / 2))
            {
                for (int X = 20; X <= 180; X += 5)
                {
                    for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 10; Y += 5)
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), WorldGen.genRand.Next(5, 7), true, false);
                    }
                }
            }
            else
            {
                for (int X = Main.maxTilesX - 180; X <= Main.maxTilesX - 20; X += 5)
                {
                    for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 10; Y += 5)
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), WorldGen.genRand.Next(5, 7), true, false);
                    }
                }
            }

            //place clumps of eye blocks throughout the biome after everything else is done
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-05); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.maxTilesY - 15);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Y >= Main.maxTilesY - 160)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(35, 65), WorldGen.genRand.Next(35, 65), 
                        ModContent.TileType<EyeBlock>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        public static void PlaceWallPillar(int StartPosition)
        {
            for (int Y = Main.maxTilesY - 190; Y <= Main.maxTilesY - 30; Y += 5)
            {
                if (WorldGen.SolidTile(StartPosition, Y))
                {
                    SpookyWorldMethods.PlaceCircle(StartPosition + 10, Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
                    SpookyWorldMethods.PlaceCircle(StartPosition - 10, Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
                }

                SpookyWorldMethods.PlaceCircle(StartPosition + WorldGen.genRand.Next(-5, 6), Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
            }
        }

		private void GenerateBloodLake(GenerationProgress progress, GameConfiguration configuration)
		{
			//define the center of the biome
			int XMiddle = (StartPosition + BiomeEdge) / 2;

			///place little eye's house
			int LakeX = (GenVars.JungleX > Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 : (XMiddle + BiomeEdge) / 2;

			Point origin = new Point(LakeX, Main.maxTilesY - 80);
			Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

			float angle = MathHelper.Pi * 0.15f;
			float otherAngle = MathHelper.PiOver2 - angle;

			int InitialSize = 80;
			int biomeSize = InitialSize + (Main.maxTilesX / 180);
			float actualSize = biomeSize * 16f;
			float constant = actualSize * 2f / (float)Math.Sin(angle);

			float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
			int verticalRadius = (int)(constant / 16f);

			Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
			Vector2 biomeTop = center - biomeOffset;
			Vector2 biomeBottom = center + biomeOffset;

            //attempt to find a valid position for the biome to place in
            bool foundValidPosition = false;
            int attempts = 0;

            int WaterHeightLimit = Main.maxTilesY - 50;

            //first do an upward check to see how high the terrain is at the position where the blood lake will generate
            while (!foundValidPosition && attempts++ < 100000)
            {
                while (WorldGen.SolidTile(origin.X, WaterHeightLimit))
                {
                    WaterHeightLimit--;
                }
                if (!WorldGen.SolidTile(origin.X, WaterHeightLimit))
                {
                    WaterHeightLimit += 10;
                    foundValidPosition = true;
                }
            }

			//first place a bunch of spider caves as a barrier around the biome
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						if (Y <= Main.maxTilesY - 60)
						{
							float percent = dist / constant;
							float blurPercent = 0.98f;

							if (percent < blurPercent)
							{
								if (Main.tileSolid[Main.tile[X, Y].TileType])
								{
									WorldGen.KillTile(X, Y);

									if (Y >= WaterHeightLimit)
									{
										Main.tile[X, Y].WallType = (ushort)ModContent.WallType<SpookyMushLakeWall>();
									}
								}
							}
							else
							{
								if (WorldGen.genRand.NextBool(3) && Y >= Main.maxTilesY - 105)
								{
									double RootAngle = Y / 3.0 * 2.0 + 0.57075;
									WorldUtils.Gen(new Point(X, Y), new ShapeRoot((int)RootAngle, WorldGen.genRand.Next(80, 120), WorldGen.genRand.Next(5, 8)),
									Actions.Chain(new Actions.ClearTile(), new Actions.ClearWall(), new Actions.PlaceWall((ushort)ModContent.WallType<SpookyMushLakeWall>(), true)));
								}
							}
						}
					}
				}
			}
		}

		//method to make sure things only generate in the biome's circle
		public static bool CheckInsideOval(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance)
		{
			Vector2 point = tile.ToWorldCoordinates();
			float distX = center.X - point.X;
			float distY = center.Y - point.Y;
			point.Y -= distY * 3f;
			point.X -= distX * 3f;

			float distance1 = Vector2.Distance(point, focus1);
			float distance2 = Vector2.Distance(point, focus2);
			distance = distance1 + distance2;

			return distance <= distanceConstant;
		}

		private void SpreadSpookyHellGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            //spread grass on all mush tiles
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 250; Y < Main.maxTilesY - 15; Y++)
                {
                    Tile up = Main.tile[X, Y - 1];
                    Tile down = Main.tile[X, Y + 1];
                    Tile left = Main.tile[X - 1, Y];
                    Tile right = Main.tile[X + 1, Y];

                    if (Main.tile[X, Y].HasTile)
                    {
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>() && ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
                        {
                            Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyMushGrass>();
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>() &&
                    (up.TileType == ModContent.TileType<SpookyMushGrass>() || down.TileType == ModContent.TileType<SpookyMushGrass>() || 
                    left.TileType == ModContent.TileType<SpookyMushGrass>() || right.TileType == ModContent.TileType<SpookyMushGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyMush>(), ModContent.TileType<SpookyMushGrass>(), false);
                    }
                }
            }
        }

        public static void SpookyHellTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 155; Y < Main.maxTilesY - 120; Y++)
                {
                    if ((Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>()) && CanPlaceTree(X, Y))
                    {
                        if (WorldGen.genRand.NextBool(7) && (Main.tile[X, Y - 1].WallType <= 0 || Main.tile[X, Y - 1].WallType == ModContent.WallType<SpookyMushWall>()) && 
                        !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                        {
                            EyeTree.Grow(X, Y - 1, 12, 35, false);
                        }
                    }
                }
            }
        }

        public static bool CanPlaceTree(int X, int Y)
        {
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<EyeTree>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SpookyHellAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y < Main.maxTilesY - 15; Y++)
                {
                    //eye vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMushGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(8))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<EyeVine>());
                        }
                    }
                    if (Main.tile[X, Y].TileType == ModContent.TileType<EyeVine>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<EyeVine>());
                    }

                    //plants that can grow on both blocks
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMushGrass>() || Main.tile[X, Y].TileType == ModContent.TileType<EyeBlock>())
                    {
						//place flesh pots
						if (WorldGen.genRand.NextBool(4))
						{
							WorldGen.PlacePot(X, Y - 1, 28, WorldGen.genRand.Next(22, 25));
						}

                        //eye stalks
                        if (WorldGen.genRand.NextBool(20))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkThinShort>(), (ushort)ModContent.TileType<EyeStalkThin>(), 
                            (ushort)ModContent.TileType<EyeStalkThinTall>(), (ushort)ModContent.TileType<EyeStalkThinVeryTall>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(25))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkSmall1>(), (ushort)ModContent.TileType<EyeStalkSmall2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(30))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkMedium1>(), (ushort)ModContent.TileType<EyeStalkMedium2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(35))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkBig1>(), (ushort)ModContent.TileType<EyeStalkBig2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(35))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkGiant1>(), (ushort)ModContent.TileType<EyeStalkGiant2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }

                        //purple eye stalks
                        if (WorldGen.genRand.NextBool(25))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkPurple1>(), (ushort)ModContent.TileType<EyeStalkPurple2>(), 
                            (ushort)ModContent.TileType<EyeStalkPurple3>(), (ushort)ModContent.TileType<EyeStalkPurple4>(),
                            (ushort)ModContent.TileType<EyeStalkPurple5>(), (ushort)ModContent.TileType<EyeStalkPurple6>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }

                        //bones
                        if (WorldGen.genRand.NextBool(40))
                        {
                            ushort[] Bones = new ushort[] { (ushort)ModContent.TileType<Bone1>(), (ushort)ModContent.TileType<Bone2>(), (ushort)ModContent.TileType<Bone3>(), (ushort)ModContent.TileType<Bone4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Bones), true);
                        }

                        //arteries
                        if (WorldGen.genRand.NextBool(12))
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<Artery1>(), (ushort)ModContent.TileType<Artery2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Arteries), true);
                        }
                        if (WorldGen.genRand.NextBool(15))
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<ArteryHanging1>(), (ushort)ModContent.TileType<ArteryHanging2>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Arteries), true);
                            WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(Arteries), true);
                        }

                        //hanging fingers
                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] HangingFinger = new ushort[] { (ushort)ModContent.TileType<FingerHanging1>(), (ushort)ModContent.TileType<FingerHanging2>() };

                            //cannot be bothered to check which one actually works
                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(HangingFinger));
                            WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(HangingFinger));    
                            WorldGen.PlaceObject(X, Y + 3, WorldGen.genRand.Next(HangingFinger));
                            WorldGen.PlaceObject(X, Y + 4, WorldGen.genRand.Next(HangingFinger));
                        }
                    }

                    //mush grass plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>())
                    {
                        if (WorldGen.genRand.NextBool(10) && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyHellWeeds>());
                            Main.tile[X, Y - 1].TileFrameX = (short)(WorldGen.genRand.Next(6) * 18);
                            WorldGen.SquareTileFrame(X, Y + 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
                            }
                        }

                        //ambient manhole teeth
                        if (WorldGen.genRand.NextBool(20))
                        {
                            WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<Tooth>(), true);
                        }
                    }

                    //eye block plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //fingers
                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] Fingers = new ushort[] { (ushort)ModContent.TileType<Finger1>(), (ushort)ModContent.TileType<Finger2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Fingers));    
                        }
                    }
                }
            }
        }

        private void SpookyHellPolish(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 230; Y < Main.maxTilesY - 15; Y++)
                {
                    //get rid of any other left over lava
                    if (Main.tile[X, Y].LiquidType == LiquidID.Lava && !Main.tile[X, Y].HasTile)
                    {
                        Tile tile = Main.tile[X, Y];
                        tile.ClearEverything();
                    }

                    //get rid of random floating singular tiles
                    if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile &&
                    !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                    {
                        WorldGen.KillTile(X, Y);
                    }

                    //get rid of 1x2 tiles on the ground since it looks weird
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() && Main.tile[X - 1, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() && 
                    !Main.tile[X - 2, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                    {
                        WorldGen.KillTile(X, Y);
                        WorldGen.KillTile(X - 1, Y);
                    }

                    //slope tiles
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<ValleyStone>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<LivingFlesh>() ||
                    Main.tile[X, Y].TileType == TileID.ObsidianBrick)
                    {
                        Tile.SmoothSlope(X, Y);
                    }
                }
            }
        }

        public void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            //define the center of the biome
            int XMiddle = (StartPosition + BiomeEdge) / 2;

            int StartPosY = Main.maxTilesY - 150;

            ///place little eye's house
            int HouseX = (GenVars.JungleX > Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 - (Main.maxTilesX / 60): (XMiddle + BiomeEdge) / 2 + (Main.maxTilesX / 60);
            GenerateStructure(HouseX, StartPosY, "LittleEyeHouse", 46, 45);

            //place orroboro nest
            GenerateStructure(XMiddle, StartPosY, "OrroboroNest", 23, 19);
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(startX, startY) && Main.tile[startX, startY].TileType != ModContent.TileType<SpookyMushGrass>() && startY < Main.maxTilesY - 50)
				{
					startY++;
				}
                if (WorldGen.SolidTile(startX, startY) && Main.tile[startX, startY].TileType == ModContent.TileType<SpookyMushGrass>())
                {
					continue;
                }

                Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                Generator.GenerateStructure("Content/Structures/SpookyHell/" + StructureFile, origin.ToPoint16(), Mod);

                if (StructureFile == "LittleEyeHouse")
                {
                    NPC.NewNPC(null, (startX - 12) * 16, (startY) * 16, ModContent.NPCType<LittleEyeSleeping>());
                }

                if (StructureFile == "OrroboroNest")
                {
                    Flags.EggPosition = new Vector2(startX * 16, startY * 16);
                    int Egg = NPC.NewNPC(null, (int)Flags.EggPosition.X, (int)Flags.EggPosition.Y, ModContent.NPCType<OrroboroEgg>());
                    Main.npc[Egg].position.X += 2;
                }

                placed = true;
            }
        }

        public void GenerateNoseTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            int DungeonX = (StartPosition < Main.maxTilesX / 2 ? 250 : Main.maxTilesX - 250);
            int CathedralX = (StartPosition < Main.maxTilesX / 2 ? 250 : Main.maxTilesX - 250);

            int StartPosY = Main.maxTilesY - 130;

            bool IsSmallWorld = Main.maxTilesX < 6400;

            GenerateNoseTempleStructure(DungeonX, Main.maxTilesY - 131, "Entrance", 49, 23);

            GenerateNoseTempleStructure(DungeonX, NoseTemplePositionY + 3, "EntranceTunnel", 9, 0);

            NoseTempleEntranceTunnelX = DungeonX;

            GenerateNoseTempleStructure(DungeonX + (DungeonX < (Main.maxTilesX / 2) ? -3 : 3), NoseTemplePositionY + 28, "MinibossRoomBarrier", 0, 6);

            //place the cathedral arena and hallways leading to it
            for (int cathedralArenaLoop = 0; cathedralArenaLoop <= 3; cathedralArenaLoop++)
            {
                //place the arena at the end of the loop
                if (cathedralArenaLoop == 3)
                {
                    CathedralX += (StartPosition < Main.maxTilesX / 2 ? -27 : 25);

                    GenerateNoseTempleStructure(CathedralX, NoseTemplePositionY, "MinibossArena", 44, 41);

                    Flags.LeaderIdolPositon = new Vector2((CathedralX) * 16, (NoseTemplePositionY + 20) * 16);

                    //when the very end of the dungeon is reached, place a wall on the entrance opening of the last room 
                    if (CathedralX > (Main.maxTilesX / 2))
                    {
                        GenerateNoseTempleStructure(DungeonX + 148, NoseTemplePositionY + 27, "RoomEndRight", 2, 6);
                    }
                    else
                    {
                        GenerateNoseTempleStructure(DungeonX - 148, NoseTemplePositionY + 27, "RoomEndLeft", 2, 6);
                    }
                }
                //otherwise place hallways
                else
                {
                    if (cathedralArenaLoop == 0)
                    {
                        CathedralX += (StartPosition < Main.maxTilesX / 2 ? -19 : 20);
                    }

                    GenerateNoseTempleStructure(CathedralX, NoseTemplePositionY + 27, "HallwayMiniboss", 10, 10);

                    CathedralX += (StartPosition < Main.maxTilesX / 2 ? -20 : 20);
                }
            }

            //place the actual dungeon rooms and hallways on the opposite side of the cathedral
            int MaxDungeonRooms = IsSmallWorld ? 2 : 4;

            for (int dungeonRoomLoop = 0; dungeonRoomLoop <= MaxDungeonRooms; dungeonRoomLoop++)
            {
                int numHallsBeforeRoom = Main.maxTilesX >= 8400 ? WorldGen.genRand.Next(2, 4) : (Main.maxTilesX >= 6400 ? WorldGen.genRand.Next(1, 3) : 1);

                for (int numLoops = 0; numLoops <= numHallsBeforeRoom; numLoops++)
                {
                    if (numLoops < numHallsBeforeRoom)
                    {
                        if (numLoops == 0)
                        {
                            DungeonX += (StartPosition < Main.maxTilesX / 2 ? 20 : -19);
                        }
                        else
                        {
                            SpookyWorldMethods.PlaceCircle(DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -26), NoseTemplePositionY - 3, ModContent.TileType<SpookyMush>(), ModContent.WallType<SpookyMushWall>(), 30, false, false);
                        }

                        GenerateNoseTempleStructure(DungeonX, NoseTemplePositionY + 27, "Hallway-" + WorldGen.genRand.Next(1, 9), 10, 10);

                        DungeonX += (StartPosition < Main.maxTilesX / 2 ? 20 : -20);
                    }
                    if (numLoops >= numHallsBeforeRoom)
                    {
                        switch (dungeonRoomLoop)
                        {
                            case 0:
                            {
                                Flags.MocoIdolPosition1 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                break;
                            }
                            case 1:
                            {
                                if (IsSmallWorld)
                                {
                                    Flags.MocoIdolPosition3 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                }
                                else
                                {
                                    Flags.MocoIdolPosition2 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                }
                                break;
                            }
                            case 2:
                            {
                                if (IsSmallWorld)
                                {
                                    Flags.MocoIdolPosition4 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                }
                                else
                                {
                                    Flags.MocoIdolPosition3 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                }
                                break;
                            }
                            case 3:
                            {
                                Flags.MocoIdolPosition4 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                break;
                            }
                            case 4:
                            {
                                Flags.MocoIdolPosition5 = new Vector2((DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27)) * 16, (NoseTemplePositionY + 25) * 16);
                                break;
                            }
                        }

                        SpookyWorldMethods.PlaceCircle(DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27), NoseTemplePositionY - 3, ModContent.TileType<SpookyMush>(), 0, 30, false, false);

                        GenerateNoseTempleStructure(DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27), NoseTemplePositionY + 18, "CombatRoom-" + WorldGen.genRand.Next(1, 6), 36, 19);

                        //when the very end of the dungeon is reached, place a wall on the entrance opening of the last room 
                        if (dungeonRoomLoop == MaxDungeonRooms)
                        {
                            if (DungeonX > (Main.maxTilesX / 2))
                            {
                                GenerateNoseTempleStructure(DungeonX - 70, NoseTemplePositionY + 27, "FireExitLeft", 7, 10);
                                GenerateNoseTempleStructure(DungeonX - 68, NoseTemplePositionY + 38, "FireExitTunnelLeft", 4, 8);

                                for (int X = DungeonX - 60; X <= NoseTempleEntranceTunnelX + 2; X++)
                                {
                                    GenerateNoseTempleStructure(X, NoseTemplePositionY + 45, "FireExitTunnelSegment", 4, 8);

                                    if (X == NoseTempleEntranceTunnelX + 2)
                                    {
                                        GenerateNoseTempleStructure(X - 1, NoseTemplePositionY + 38, "FireExitTunnelRightBarrier", 4, 8);
                                    }
                                }
                            }
                            else
                            {
                                GenerateNoseTempleStructure(DungeonX + 70, NoseTemplePositionY + 27, "FireExitRight", 7, 10);
                                GenerateNoseTempleStructure(DungeonX + 68, NoseTemplePositionY + 38, "FireExitTunnelRight", 4, 8);

                                for (int X = DungeonX + 68; X >= NoseTempleEntranceTunnelX + 2; X--)
                                {
                                    GenerateNoseTempleStructure(X, NoseTemplePositionY + 45, "FireExitTunnelSegment", 4, 8);

                                    if (X == NoseTempleEntranceTunnelX + 2)
                                    {
                                        GenerateNoseTempleStructure(X - 2, NoseTemplePositionY + 38, "FireExitTunnelLeftBarrier", 4, 8);
                                    }
                                }
                            }
                        }

                        DungeonX += (StartPosition < Main.maxTilesX / 2 ? 53 : -54);
                    }
                }
            }

            //brick color variance
            int Brick = ModContent.TileType<NoseTempleBrickPurple>();
            int FancyBrick = ModContent.TileType<NoseTempleFancyBrickPurple>();
            int BrickWall = ModContent.WallType<NoseTempleWallPurple>();
            int BrickWallSafe = ModContent.WallType<NoseTempleWallPurpleSafe>();
            int FancyBrickWall = ModContent.WallType<NoseTempleFancyWallPurple>();
            int FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallPurpleSafe>();
            int BGBrickWall = ModContent.WallType<NoseTempleWallBGPurple>();
            int Platform = ModContent.TileType<NoseTemplePlatformPurple>();

            NoseTempleBrickColor = WorldGen.genRand.Next(0, 3);

            switch (NoseTempleBrickColor)
            {
                case 0:
                {
                    Brick = ModContent.TileType<NoseTempleBrickGreen>();
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickGreen>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGGreen>();
                    Platform = ModContent.TileType<NoseTemplePlatformGreen>();
                    break;
                }
                case 1:
                {
                    Brick = ModContent.TileType<NoseTempleBrickPurple>();
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickPurple>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGPurple>();
                    Platform = ModContent.TileType<NoseTemplePlatformPurple>();
                    break;
                }
                case 2:
                {
                    Brick = ModContent.TileType<NoseTempleBrickGray>();
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickGray>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGGray>();
                    Platform = ModContent.TileType<NoseTemplePlatformGray>();
                    break;
                }
            }

            NoseTempleBrickWallColor = Main.rand.Next(0, 3);

            switch (NoseTempleBrickWallColor)
            {
                case 0:
                {
                    BrickWall = ModContent.WallType<NoseTempleWallGreen>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallGreenSafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallGreen>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallGreenSafe>();
                    break;
                }
                case 1:
                {
                    BrickWall = ModContent.WallType<NoseTempleWallPurple>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallPurpleSafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallPurple>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallPurpleSafe>();
                    break;
                }
                case 2:
                {
                    BrickWall = ModContent.WallType<NoseTempleWallGray>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallGraySafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallGray>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallGraySafe>();
                    break;
                }
            }

            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 6; Y++)
                {
                    Tile tile = Main.tile[X, Y];

                    //regular bricks
                    if (tile.TileType == ModContent.TileType<NoseTempleBrickPurple>())
                    {
                        tile.TileType = (ushort)Brick;
                    }
                    //fancy bricks
                    if (tile.TileType == ModContent.TileType<NoseTempleFancyBrickPurple>())
                    {
                        tile.TileType = (ushort)FancyBrick;
                    }
                    //walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallPurple>())
                    {
                        tile.WallType = (ushort)BrickWall;
                    }
                    //walls safe
                    if (tile.WallType == ModContent.WallType<NoseTempleWallPurpleSafe>())
                    {
                        tile.WallType = (ushort)BrickWallSafe;
                    }
                    //fancy walls
                    if (tile.WallType == ModContent.WallType<NoseTempleFancyWallPurple>())
                    {
                        tile.WallType = (ushort)FancyBrickWall;
                    }
                    //fancy walls safe
                    if (tile.WallType == ModContent.WallType<NoseTempleFancyWallPurpleSafe>())
                    {
                        tile.WallType = (ushort)FancyBrickWallSafe;
                    }
                    //BG walls
                    if (tile.WallType == ModContent.WallType<NoseTempleWallBGPurple>())
                    {
                        tile.WallType = (ushort)BGBrickWall;
                    }
                    //Platforms
                    if (tile.TileType == ModContent.TileType<NoseTemplePlatformPurple>())
                    {
                        tile.TileType = (ushort)Platform;
                    }
                }
            }
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateNoseTempleStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            while (!placed)
            {
                if (StructureFile == "Entrance")
                {
                    NoseTemplePositionY = startY + 25;
                }

                Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                Generator.GenerateStructure("Content/Structures/NoseTemple/" + StructureFile, origin.ToPoint16(), Mod);

                placed = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Eye Valley", GenerateSpookyHell));
			tasks.Insert(GenIndex1 + 2, new PassLegacy("Blood Lake", GenerateBloodLake));
			tasks.Insert(GenIndex1 + 3, new PassLegacy("Nose Cultist Dungeon", GenerateNoseTemple));
            tasks.Insert(GenIndex1 + 4, new PassLegacy("Eye Valley Polish", SpookyHellPolish));
            tasks.Insert(GenIndex1 + 5, new PassLegacy("Eye Valley Structures", GenerateStructures));
            tasks.Insert(GenIndex1 + 6, new PassLegacy("Eye Valley Grass", SpreadSpookyHellGrass));
            tasks.Insert(GenIndex1 + 7, new PassLegacy("Eye Valley Trees", SpookyHellTrees));
            tasks.Insert(GenIndex1 + 8, new PassLegacy("Eye Valley Ambient Tiles", SpookyHellAmbience));
        }
    }
}