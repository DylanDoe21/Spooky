using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
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
        int NoseTempleSlabColor;
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
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 198; Y++)
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

            //first do an upward check to see how high the terrain is at the position where the blood lake will generate, so that the water level is adjusted with the terrain height
            while (!foundValidPosition && attempts++ < 100000)
            {
                while (WorldGen.SolidTile(origin.X, WaterHeightLimit))
                {
                    WaterHeightLimit--;
                }
                if (!WorldGen.SolidTile(origin.X, WaterHeightLimit))
                {
                    //increase the water level limit to be lower so it doesnt reach over the top of the terrain
                    WaterHeightLimit += 15;
                    foundValidPosition = true;
                }
            }

			//place an oval and fill it with the water producing walls based on where the water height limit is
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist, false))
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
								if (WorldGen.genRand.NextBool(3) && Y >= WaterHeightLimit + 25)
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

            //reset everything for the circle clearing
            bool IsSmallWorld = Main.maxTilesX < 6400;
            bool IsMediumWorld = Main.maxTilesX < 8400 && Main.maxTilesX > 6400;

            origin = new Point(LakeX, WaterHeightLimit - (IsSmallWorld ? 25 : (IsMediumWorld ? 28 : 34)));
			center = origin.ToVector2() * 16f + new Vector2(8f);

			angle = MathHelper.Pi * 0.15f;
			otherAngle = MathHelper.PiOver2 - angle;

			InitialSize = 32;
			biomeSize = InitialSize + (Main.maxTilesX / 180);
			actualSize = biomeSize * 16f;
			constant = actualSize * 2f / (float)Math.Sin(angle);

			biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
			verticalRadius = (int)(constant / 16f);

			biomeOffset = Vector2.UnitY * biomeSpacing;
			biomeTop = center - biomeOffset;
			biomeBottom = center + biomeOffset;

            //make another circle at the water level of the lake and clear an ellipse of tiles to make the surface around it dip down into it
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist, true))
					{
                        WorldGen.KillTile(X, Y);
                    }
                }
            }
		}

		//method to make sure things only generate in the biome's circle
		public static bool CheckInsideOval(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool stretch)
		{
			Vector2 point = tile.ToWorldCoordinates();

            if (!stretch)
            {
                float distX = center.X - point.X;
                point.X -= distX * 3f;
                float distY = center.Y - point.Y;
                point.Y -= distY * 3f;
            }
            else
            {
                float distY = center.Y - point.Y;
                point.Y -= distY * 4f;
            }

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
                        if (WorldGen.genRand.NextBool(25))
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

                    //get rid of single tiles on the ground since it looks weird
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                    {
                        WorldGen.KillTile(X, Y);
                    }

                    //slope tiles
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<ValleyStone>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>() || Main.tile[X, Y].TileType == TileID.ObsidianBrick)
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
            int HouseX = (GenVars.JungleX > Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 - (Main.maxTilesX / 55) : (XMiddle + BiomeEdge) / 2 + (Main.maxTilesX / 55);
            GenerateStructure(HouseX, StartPosY, "LittleEyeHouse", 46, 45);

            //place orroboro nest
            GenerateStructure(XMiddle, StartPosY, "OrroboroNest", 23, 23);

            //place random structures after the main ones are generated
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 6; Y++)
                {
                    //ground structures
                    if (Main.tile[X, Y].HasTile && Main.tile[X - 1, Y].HasTile && Main.tile[X - 2, Y].HasTile && 
                    Main.tile[X + 1, Y].HasTile && Main.tile[X + 2, Y].HasTile && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y - 2].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(15) && CanPlaceStructure(X, Y))
                        {
                            switch (WorldGen.genRand.Next(4))
                            {
                                case 0:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 11);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/FloorTendril" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 1:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 10);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/FloorSkull" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 2:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 3);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/FloorEye" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 3:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 11, Y - 6);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/FloorRibs", structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                            }
                        }
                    }

                    //ceiling structures
                    if (Main.tile[X, Y].HasTile && Main.tile[X - 1, Y].HasTile && Main.tile[X - 2, Y].HasTile && 
                    Main.tile[X + 1, Y].HasTile && Main.tile[X + 2, Y].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X, Y + 2].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(18) && CanPlaceStructure(X, Y))
                        {
                            switch (WorldGen.genRand.Next(6))
                            {
                                case 0:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 5);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingEye" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 1:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 3);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingEyePair" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 2:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 6, Y - 3);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingEyeForward", structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 3:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 5, Y - 5);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingEyeStalk" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 4:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 5, Y - 5);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingEyeStalkLong", structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                                case 5:
                                {
                                    Vector2 structureOrigin = new Vector2(X - 8, Y - 6);
                                    Generator.GenerateStructure("Content/Structures/SpookyHell/CeilingMouth" + WorldGen.genRand.Next(1, 3), structureOrigin.ToPoint16(), Mod);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
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
                    Flags.EyeValleyCenter =  new Vector2(startX * 16, startY * 16);

                    Flags.EggPosition = new Vector2(startX * 16, startY * 16);
                    
                    int Egg = NPC.NewNPC(null, (int)Flags.EggPosition.X, (int)Flags.EggPosition.Y, ModContent.NPCType<OrroboroEgg>());
                    Main.npc[Egg].position.X += 1;
                    Main.npc[Egg].position.Y -= 32;
                }

                placed = true;
            }
        }

        //determine if a structure can be placed at a set position
        public static bool CanPlaceStructure(int X, int Y)
        {
            int[] InvalidTiles = { ModContent.TileType<NoseTempleBrickGray>(), ModContent.TileType<NoseTempleBrickGreen>(), ModContent.TileType<NoseTempleBrickPurple>(), ModContent.TileType<NoseTempleBrickRed>(),
            ModContent.TileType<NoseTempleFancyBrickGray>(), ModContent.TileType<NoseTempleFancyBrickGreen>(), ModContent.TileType<NoseTempleFancyBrickPurple>(), ModContent.TileType<NoseTempleFancyBrickRed>(),
            ModContent.TileType<NoseTemplePlatformGray>(), ModContent.TileType<NoseTemplePlatformGreen>(), ModContent.TileType<NoseTemplePlatformPurple>(), ModContent.TileType<NoseTemplePlatformRed>(),
            ModContent.TileType<LivingFlesh>(), ModContent.TileType<ValleyStone>(), ModContent.TileType<OrroboroNestBlock>(), TileID.SlimeBlock };

            for (int i = X - 35; i < X + 35; i++)
            {
                for (int j = Y - 25; j < Y + 25; j++)
                {
                    if (WorldGen.InWorld(i, j) && InvalidTiles.Contains(Main.tile[i, j].TileType))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void GenerateNoseTemple(GenerationProgress progress, GameConfiguration configuration)
        {
            int DungeonX = (StartPosition < Main.maxTilesX / 2 ? 250 : Main.maxTilesX - 250);
            int ArenaX = (StartPosition < Main.maxTilesX / 2 ? 250 : Main.maxTilesX - 250);

            int StartPosY = Main.maxTilesY - 130;

            bool IsSmallWorld = Main.maxTilesX < 6400;

            GenerateNoseTempleStructure(DungeonX, Main.maxTilesY - 131, "Entrance", 49, 23);

            GenerateNoseTempleStructure(DungeonX, NoseTemplePositionY + 3, "EntranceTunnel", 9, 0);

            NoseTempleEntranceTunnelX = DungeonX;

            GenerateNoseTempleStructure(DungeonX + (DungeonX < (Main.maxTilesX / 2) ? -3 : 3), NoseTemplePositionY + 28, "MinibossRoomBarrier", 0, 6);

            //place the miniboss arena and hallways leading to it
            for (int ArenaLoop = 0; ArenaLoop <= 3; ArenaLoop++)
            {
                //place the arena at the end of the loop
                if (ArenaLoop == 3)
                {
                    ArenaX += (StartPosition < Main.maxTilesX / 2 ? -27 : 25);

                    GenerateNoseTempleStructure(ArenaX, NoseTemplePositionY, "MinibossArena", 44, 41);

                    Flags.LeaderIdolPositon = new Vector2((ArenaX) * 16, (NoseTemplePositionY + 20) * 16);

                    //when the very end of the dungeon is reached, place a wall on the entrance opening of the last room 
                    if (ArenaX > (Main.maxTilesX / 2))
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
                    if (ArenaLoop == 0)
                    {
                        ArenaX += (StartPosition < Main.maxTilesX / 2 ? -19 : 20);
                    }

                    GenerateNoseTempleStructure(ArenaX, NoseTemplePositionY + 27, "HallwayMiniboss", 10, 10);

                    ArenaX += (StartPosition < Main.maxTilesX / 2 ? -20 : 20);
                }
            }

            //place the actual dungeon rooms and hallways on the opposite side of the Arena
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

                        SpookyWorldMethods.PlaceCircle(DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27), NoseTemplePositionY + 3, ModContent.TileType<SpookyMush>(), 0, 35, false, false);

                        GenerateNoseTempleStructure(DungeonX + (StartPosition < Main.maxTilesX / 2 ? 26 : -27), NoseTemplePositionY + 18, "CombatRoom-" + WorldGen.genRand.Next(1, 6), 36, 19);

                        //when the very end of the dungeon is reached, place the fire exit room and hallway tunnel that loops back to the entrance
                        if (dungeonRoomLoop == MaxDungeonRooms)
                        {
                            if (DungeonX > (Main.maxTilesX / 2))
                            {
                                GenerateNoseTempleStructure(DungeonX - 70, NoseTemplePositionY + 27, "FireExitLeft", 7, 10);
                                GenerateNoseTempleStructure(DungeonX - 68, NoseTemplePositionY + 38, "FireExitTunnelLeft", 4, 8);

                                for (int X = DungeonX - 60; X <= NoseTempleEntranceTunnelX + 2; X++)
                                {
                                    GenerateNoseTempleStructure(X, NoseTemplePositionY + 45, "FireExitTunnelSegment", 4, 8);

                                    if (X == ((DungeonX - 60) + (NoseTempleEntranceTunnelX + 2)) / 2)
                                    {
                                        GenerateNoseTempleStructure(X - 1, NoseTemplePositionY + 45, "FireExitTunnelSegmentChest", 4, 8);
                                    }

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

                                    if (X == ((DungeonX + 68) + (NoseTempleEntranceTunnelX + 2)) / 2)
                                    {
                                        GenerateNoseTempleStructure(X + 1, NoseTemplePositionY + 45, "FireExitTunnelSegmentChest", 4, 8);
                                    }

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

            //place surface ruins structure
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                int SurfaceDungeonX = (StartPosition < Main.maxTilesX / 2 ? 250 : Main.maxTilesX - 250);
                int X = SurfaceDungeonX <= (Main.maxTilesX / 2) ? SurfaceDungeonX + 75 : SurfaceDungeonX - 75;
                int Y = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(X, Y) && Y < Main.maxTilesY)
				{
					Y++;
				}
                if (WorldGen.SolidTile(X, Y))
				{
                    GenerateNoseTempleStructure(X, Y, "SurfaceRuins", 22, 20);

                    placed = true;
				}
            }

            //brick color variance
            int Brick = ModContent.TileType<NoseTempleBrickPurple>();
            int FancyBrick = ModContent.TileType<NoseTempleFancyBrickPurple>();
            int BrickSafe = ModContent.TileType<NoseTempleBrickPurpleSafe>();
            int FancyBrickSafe = ModContent.TileType<NoseTempleFancyBrickPurpleSafe>();
            int BrickWall = ModContent.WallType<NoseTempleWallPurple>();
            int BrickWallSafe = ModContent.WallType<NoseTempleWallPurpleSafe>();
            int FancyBrickWall = ModContent.WallType<NoseTempleFancyWallPurple>();
            int FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallPurpleSafe>();
            int BGBrickWall = ModContent.WallType<NoseTempleWallBGPurple>();
            int Platform = ModContent.TileType<NoseTemplePlatformPurple>();

            //for debugging: 0 = gray, 1 = green, 2 = purple, 3 = red (applies to both brick colors)
            NoseTempleBrickColor = WorldGen.genRand.Next(0, 4);
            NoseTempleSlabColor = WorldGen.genRand.Next(0, 4);

            switch (NoseTempleBrickColor)
            {
                case 0:
                {
                    Brick = ModContent.TileType<NoseTempleBrickGray>();
                    BrickSafe = ModContent.TileType<NoseTempleBrickGraySafe>();
                    BrickWall = ModContent.WallType<NoseTempleWallGray>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallGraySafe>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGGray>();
                    break;
                }
                case 1:
                {
                    Brick = ModContent.TileType<NoseTempleBrickGreen>();
                    BrickSafe = ModContent.TileType<NoseTempleBrickGreenSafe>();
                    BrickWall = ModContent.WallType<NoseTempleWallGreen>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallGreenSafe>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGGreen>();
                    break;
                }
                case 2:
                {
                    Brick = ModContent.TileType<NoseTempleBrickPurple>();
                    BrickSafe = ModContent.TileType<NoseTempleBrickPurpleSafe>();
                    BrickWall = ModContent.WallType<NoseTempleWallPurple>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallPurpleSafe>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGPurple>();
                    break;
                }
                case 3:
                {
                    Brick = ModContent.TileType<NoseTempleBrickRed>();
                    BrickSafe = ModContent.TileType<NoseTempleBrickRedSafe>();
                    BrickWall = ModContent.WallType<NoseTempleWallRed>();
                    BrickWallSafe = ModContent.WallType<NoseTempleWallRedSafe>();
                    BGBrickWall = ModContent.WallType<NoseTempleWallBGRed>();
                    break;
                }
            }

            switch (NoseTempleSlabColor)
            {
                case 0:
                {
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickGray>();
                    FancyBrickSafe = ModContent.TileType<NoseTempleFancyBrickGraySafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallGray>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallGraySafe>();
                    Platform = ModContent.TileType<NoseTemplePlatformGray>();
                    break;
                }
                case 1:
                {
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickGreen>();
                    FancyBrickSafe = ModContent.TileType<NoseTempleFancyBrickGreenSafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallGreen>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallGreenSafe>();
                    Platform = ModContent.TileType<NoseTemplePlatformGreen>();
                    break;
                }
                case 2:
                {
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickPurple>();
                    FancyBrickSafe = ModContent.TileType<NoseTempleFancyBrickPurpleSafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallPurple>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallPurpleSafe>();
                    Platform = ModContent.TileType<NoseTemplePlatformPurple>();
                    break;
                }
                case 3:
                {
                    FancyBrick = ModContent.TileType<NoseTempleFancyBrickRed>();
                    FancyBrickSafe = ModContent.TileType<NoseTempleFancyBrickRedSafe>();
                    FancyBrickWall = ModContent.WallType<NoseTempleFancyWallRed>();
                    FancyBrickWallSafe = ModContent.WallType<NoseTempleFancyWallRedSafe>();
                    Platform = ModContent.TileType<NoseTemplePlatformRed>();
                    break;
                }
            }

            //actual tile conversions
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
                    //regular bricks safe
                    if (tile.TileType == ModContent.TileType<NoseTempleBrickPurpleSafe>())
                    {
                        tile.TileType = (ushort)BrickSafe;
                    }
                    //fancy bricks safe
                    if (tile.TileType == ModContent.TileType<NoseTempleFancyBrickPurpleSafe>())
                    {
                        tile.TileType = (ushort)FancyBrickSafe;
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
                    //platforms
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
            int GenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			if (GenIndex == -1)
			{
				return;
			}

            tasks.Insert(GenIndex + 1, new PassLegacy("Eye Valley", GenerateSpookyHell));
			tasks.Insert(GenIndex + 2, new PassLegacy("Blood Lake", GenerateBloodLake));
			tasks.Insert(GenIndex + 3, new PassLegacy("Nose Cultist Dungeon", GenerateNoseTemple));
            tasks.Insert(GenIndex + 4, new PassLegacy("Eye Valley Polish", SpookyHellPolish));
            tasks.Insert(GenIndex + 5, new PassLegacy("Eye Valley Structures", GenerateStructures));
            tasks.Insert(GenIndex + 6, new PassLegacy("Eye Valley Grass", SpreadSpookyHellGrass));
            tasks.Insert(GenIndex + 7, new PassLegacy("Eye Valley Trees", SpookyHellTrees));
            tasks.Insert(GenIndex + 8, new PassLegacy("Eye Valley Ambient Tiles", SpookyHellAmbience));
        }
    }
}