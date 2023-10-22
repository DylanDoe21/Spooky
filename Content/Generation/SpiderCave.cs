using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Generation
{
    public class SpiderCave : ModSystem
    {
        private void PlaceSpiderCaves(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpiderCave").Value;

            int biomeExtraDepth;

            int StartPositionX1 = (Main.maxTilesX / 2) - (Main.maxTilesX / 5) + WorldGen.genRand.Next(-200, 200);
            int StartPositionX2 = (Main.maxTilesX / 2) + WorldGen.genRand.Next(-200, 200);
            int StartPositionX3 = (Main.maxTilesX / 2) + (Main.maxTilesX / 5) + WorldGen.genRand.Next(-200, 200);

            //place only 2 caves if the world size anything smaller than a medium world
            if (Main.maxTilesY < 1800)
            {
                StartPositionX1 = (Main.maxTilesX / 2) - (Main.maxTilesX / 9) + WorldGen.genRand.Next(-100, 100);
                StartPositionX3 = (Main.maxTilesX / 2) + (Main.maxTilesX / 9) + WorldGen.genRand.Next(-100, 100);

                PlaceIndividualSpiderBiome(StartPositionX1, (Main.maxTilesY / 2) + WorldGen.genRand.Next(130, 150), WorldGen.genRand.Next(150, 200));

                PlaceIndividualSpiderBiome(StartPositionX3, (Main.maxTilesY / 2) + WorldGen.genRand.Next(130, 150), WorldGen.genRand.Next(150, 200));
            }
            //place 3 caves if the world is medium or anything larger
            else
            {
                biomeExtraDepth = Main.maxTilesY >= 2400 ? WorldGen.genRand.Next(550, 650) : WorldGen.genRand.Next(360, 400);
                PlaceIndividualSpiderBiome(StartPositionX1, (Main.maxTilesY / 2) + biomeExtraDepth, WorldGen.genRand.Next(150, 200));

                biomeExtraDepth = Main.maxTilesY >= 2400 ? WorldGen.genRand.Next(550, 650) : WorldGen.genRand.Next(360, 400);
                PlaceIndividualSpiderBiome(StartPositionX2, (Main.maxTilesY / 2) + biomeExtraDepth, WorldGen.genRand.Next(150, 200));

                biomeExtraDepth = Main.maxTilesY >= 2400 ? WorldGen.genRand.Next(550, 650) : WorldGen.genRand.Next(360, 400);
                PlaceIndividualSpiderBiome(StartPositionX3, (Main.maxTilesY / 2) + biomeExtraDepth, WorldGen.genRand.Next(150, 200));
            }
        }

        public static void PlaceIndividualSpiderBiome(int startPosX, int startPosY, int Size)
        {
            int cavePerlinSeed = WorldGen.genRand.Next();
            int cavePerlinSeedWalls = WorldGen.genRand.Next();

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = Size + (Main.maxTilesX - 4200) / 4200 * 400;
            float distance = distanceInTiles * 16f;
            float constant = distance * 2f / (float)Math.Sin(angle);

            float biomeSpacing = distance * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            //first, place a large barrier of stone along where the bottom of the biome will be
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.99f;

                        if (percent > blurPercent && Y >= origin.Y)
                        {
                            WorldGen.TileRunner(X, Y + 30, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), TileID.Stone, true, 0f, 0f, true, true);
                            //SpookyWorldMethods.ModifiedTileRunner(X, Y + 30, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), TileID.Stone, 0, true, 0f, 0f, true, false, true, false);
                        }
                    }
                }
            }

            //dig out all the caverns and place walls
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //generate perlin noise caves
                        float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
                        float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 800f, Y / 900f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
                        float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 800f, Y / 900f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
                        float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
                        float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

                        //clear absolutely everything before generating the caverns
                        Main.tile[X, Y].ClearEverything();

                        //kill or place tiles depending on the noise map
                        if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
                        {
                            WorldGen.KillTile(X, Y);
                        }
                        else
                        {
                            WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<DampSoil>());
                        }

                        //place walls in the biome using a different "seed" so it differs from the cave generation
                        //this creates a neat effect where walls worm their way through the caverns while leaving openings for the background to show through
                        float horizontalOffsetNoiseWalls = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(cavePerlinSeedWalls + 1)) * 0.01f;
                        float cavePerlinValueWalls = SpookyWorldMethods.PerlinNoise2D(X / 800f, Y / 900f, 5, cavePerlinSeedWalls) + 0.5f + horizontalOffsetNoiseWalls;
                        float cavePerlinValue2Walls = SpookyWorldMethods.PerlinNoise2D(X / 800f, Y / 900f, 5, unchecked(cavePerlinSeedWalls - 1)) + 0.5f;
                        float caveNoiseMapWalls = (cavePerlinValueWalls + cavePerlinValue2Walls) * 0.5f;
                        float caveCreationThresholdWalls = horizontalOffsetNoiseWalls * 8.5f + 0.235f;

                        if (caveNoiseMapWalls * caveNoiseMapWalls > caveCreationThresholdWalls)
                        {
                            WorldGen.PlaceWall(X, Y, ModContent.WallType<DampGrassWall>());
                        }
                    }
                }
            }

            //after the main biome is done, generate some more smaller features
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.98f;

                        //place a thin layer of soil around the bottom edge of the biome, on top of the stone barrier
                        if (percent > blurPercent && Y >= origin.Y)
                        {
                            SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<DampSoil>(), WorldGen.genRand.Next(3, 5), true, false);
                        }

                        //occasionally place large chunks of stone blocks
                        if (WorldGen.genRand.NextBool(1000) && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>())
                        {
                            WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), TileID.Stone, false, 0f, 0f, true, true);
                        }

                        //place mounds of web blocks on the floor
                        if (WorldGen.genRand.NextBool(20) && Main.tile[X, Y].HasTile && !Main.tile[X, Y - 1].HasTile)
                        {
                            SpookyWorldMethods.PlaceMound(X, Y + 5, ModContent.TileType<WebBlock>(), WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(6, 15));
                        }

                        //place smaller chunks of web blocks on the ceiling
                        if (WorldGen.genRand.NextBool(20) && Main.tile[X, Y].HasTile && !Main.tile[X, Y + 1].HasTile)
                        {
                            SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<WebBlock>(), WorldGen.genRand.Next(1, 5), true, false);
                        }

                        /*
                        //place large clumps of vanilla cobwebs on the ceiling, but they will not replace existing tiles
                        if (WorldGen.genRand.NextBool(30) && Main.tile[X, Y].HasTile && !Main.tile[X, Y + 1].HasTile)
                        {
                            SpookyWorldMethods.PlaceCircle(X, Y, TileID.Cobweb, WorldGen.genRand.Next(4, 8), false, false);
                        }
                        */
                    }
                }
            }

            //some small last minute things, mainly clean up before ambient tiles are placed
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //spread grass onto the dirt blocks throughout the biome
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<DampSoil>(), ModContent.TileType<DampGrass>(), false);

                        //preform tile sloping
                        Tile.SmoothSlope(X, Y, true);

                        //delete any extra liquids that may still be in the biome
                        Main.tile[X, Y].LiquidAmount = 0;

                        //delete random single floating tiles
                        if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile &&
                        !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //delete random single floating walls
                        if (Main.tile[X, Y - 1].WallType <= 0 && Main.tile[X, Y + 1].WallType <= 0 &&
                        Main.tile[X - 1, Y].WallType <= 0 && Main.tile[X + 1, Y].WallType <= 0)
                        {
                            WorldGen.KillWall(X, Y);
                        }
                    }
                }
            }

            //place ambient stuff
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                    }
                }
            }
        }

        //method to make sure things only generate in the biome's circle
        public static bool CheckInsideCircle(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance)
        {
            Vector2 point = tile.ToWorldCoordinates();
            float distY = center.Y - point.Y;

            //squish the circle vertically
            point.Y -= distY * 2.5f;

            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            
            return distance <= distanceConstant;
        }

        //worldgenning tasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {  
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Oasis"));
            if (GenIndex1 == -1) 
            {
                return;
            }

            tasks.Insert(GenIndex1 + 1, new PassLegacy("SpiderCave", PlaceSpiderCaves));
        }
    }
}