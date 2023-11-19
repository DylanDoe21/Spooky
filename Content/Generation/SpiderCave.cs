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

using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Tree;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Generation
{
    public class SpiderCave : ModSystem
    {
        private void PlaceSpiderCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpiderCave").Value;

            //get the center of the snow biome
            int SnowMiddle = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;

            int BiomeCenterX = (SnowMiddle + (Main.maxTilesX / 2)) / 2;

            int BiomeSize = Main.maxTilesY >= 2400 ? WorldGen.genRand.Next(300, 350) : (Main.maxTilesY >= 1800 ? WorldGen.genRand.Next(250, 300) : 140);

            int ExtraHeight = WorldGen.genRand.Next(20, 55);

            GenerateSpiderBiome(BiomeCenterX + (BiomeCenterX < (Main.maxTilesX / 2) ? -(Main.maxTilesX / 25) : (Main.maxTilesX / 25)), (Main.maxTilesY - (Main.maxTilesY / 3)) - ExtraHeight, BiomeSize);
        }

        public static void GenerateSpiderBiome(int startPosX, int startPosY, int Size)
        {
            int cavePerlinSeed = WorldGen.genRand.Next();
            int cavePerlinSeedWalls = WorldGen.genRand.Next();

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = Size + (Main.maxTilesX - 4200) / 4200;
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
                            WorldGen.TileRunner(X, Y + 20, WorldGen.genRand.Next(18, 25), WorldGen.genRand.Next(18, 25), TileID.Stone, true, 0f, 0f, true, true);
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
                        //clear absolutely everything before generating the caverns
                        Main.tile[X, Y].ClearEverything();

                        //generate perlin noise caves
                        float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 80f, Y / 80f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
                        float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 1350f, Y / 900f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
                        float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 1350f, Y / 900f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
                        float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
                        float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

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
                        float cavePerlinValueWalls = SpookyWorldMethods.PerlinNoise2D(X / 1200f, Y / 600f, 5, cavePerlinSeedWalls) + 0.5f + horizontalOffsetNoiseWalls;
                        float cavePerlinValue2Walls = SpookyWorldMethods.PerlinNoise2D(X / 1200f, Y / 600f, 5, unchecked(cavePerlinSeedWalls - 1)) + 0.5f;
                        float caveNoiseMapWalls = (cavePerlinValueWalls + cavePerlinValue2Walls) * 0.5f;
                        float caveCreationThresholdWalls = horizontalOffsetNoiseWalls * 3.5f + 0.235f;

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
                            WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), ModContent.TileType<SpiderLimestone>(), false, 0f, 0f, true, true);
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

                        //tile sloping
                        Tile.SmoothSlope(X, Y, true);

                        //remove any extra liquids that may still be in the biome
                        Main.tile[X, Y].LiquidAmount = 0;

                        //clean tiles that are sticking out (aka tiles only attached to one tile on one side)
                        bool OnlyRight = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile;
                        bool OnlyLeft = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyDown = !Main.tile[X, Y - 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyUp = !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;

                        if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating tiles
                        if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating walls
                        if (Main.tile[X, Y - 1].WallType <= 0 && Main.tile[X, Y + 1].WallType <= 0 && Main.tile[X - 1, Y].WallType <= 0 && Main.tile[X + 1, Y].WallType <= 0)
                        {
                            WorldGen.KillWall(X, Y);
                        }
                    }
                }
            }

            //place ambient tiles
            for (int X = origin.X - distanceInTiles - 2; X <= origin.X + distanceInTiles + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>())
                        {
                            //check if tiles around this block have grass
                            bool TopLeft = Main.tile[X - 1, Y - 1].TileType == ModContent.TileType<DampGrass>();
                            bool TopRight = Main.tile[X + 1, Y - 1].TileType == ModContent.TileType<DampGrass>();
                            bool BottomLeft = Main.tile[X - 1, Y + 1].TileType == ModContent.TileType<DampGrass>();
                            bool BottomRight = Main.tile[X + 1, Y + 1].TileType == ModContent.TileType<DampGrass>();
                            bool Left = Main.tile[X - 1, Y].TileType == ModContent.TileType<DampGrass>();
                            bool Right = Main.tile[X + 1, Y].TileType == ModContent.TileType<DampGrass>();
                            bool Top = Main.tile[X, Y - 1].TileType == ModContent.TileType<DampGrass>();
                            bool Bottom = Main.tile[X, Y + 1].TileType == ModContent.TileType<DampGrass>();

                            if (!TopLeft && !TopRight && !BottomLeft && !BottomRight && !Left && !Right && !Top && !Bottom)
                            {
                                WorldGen.PlaceWall(X, Y, (ushort)ModContent.WallType<SpiderRootWall>());
                                Main.tile[X, Y].WallType = (ushort)ModContent.WallType<SpiderRootWall>();
                            }
                        }

                        if (Main.tile[X, Y].TileType == ModContent.TileType<WebBlock>())
                        {
                            ushort[] CeilingWebs = new ushort[] { (ushort)ModContent.TileType<CeilingWeb1>(), (ushort)ModContent.TileType<CeilingWeb2>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(CeilingWebs));
                            WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(CeilingWebs));
                        }

                        //place ambient tiles that can spawn on stone and grass
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>() || Main.tile[X, Y].TileType == ModContent.TileType<SpiderLimestone>())
                        {
                            //large hanging roots
                            if (WorldGen.genRand.NextBool())
                            {
                                ushort[] HangingRoots = new ushort[] { (ushort)ModContent.TileType<HangingRoots1>(), (ushort)ModContent.TileType<HangingRoots2>(), 
                                (ushort)ModContent.TileType<HangingRoots3>(), (ushort)ModContent.TileType<HangingRoots4>() };

                                WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(HangingRoots));
                                WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(HangingRoots));
                            }
                            
                            //light roots on the ground
                            if (WorldGen.genRand.NextBool(10))
                            {
                                ushort[] LightRoots = new ushort[] { (ushort)ModContent.TileType<LightRoot1>(), (ushort)ModContent.TileType<LightRoot2>(), 
                                (ushort)ModContent.TileType<LightRoot3>(), (ushort)ModContent.TileType<LightRoot4>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(LightRoots));
                            }

                            //light root on the ceiling
                            if (WorldGen.genRand.NextBool(10))
                            {
                                ushort[] LightRoots = new ushort[] { (ushort)ModContent.TileType<HangingLightRoot1>(), (ushort)ModContent.TileType<HangingLightRoot2>(), 
                                (ushort)ModContent.TileType<HangingLightRoot3>(), (ushort)ModContent.TileType<HangingLightRoot4>() };

                                WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(LightRoots));
                            }
                        }

                        //grass only ambient tiles
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>())
                        {
                            if (WorldGen.genRand.NextBool(3) && !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                            {
                                CanGrowGiantRoot(X, Y, ModContent.TileType<GiantRoot>(), 6, 12);
                            }

                            //mushrooms
                            if (WorldGen.genRand.NextBool(5))
                            {
                                ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBlue1>(), (ushort)ModContent.TileType<MushroomBlue2>(), 
                                (ushort)ModContent.TileType<MushroomBlue3>(), (ushort)ModContent.TileType<MushroomBlue4>(), 
                                (ushort)ModContent.TileType<MushroomOrange1>(), (ushort)ModContent.TileType<MushroomOrange2>(), 
                                (ushort)ModContent.TileType<MushroomOrange3>(), (ushort)ModContent.TileType<MushroomOrange4>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Mushrooms));
                            }
                        }

                        //place spider webs on walls
                        if (WorldGen.genRand.NextBool(200) && Main.tile[X, Y].WallType == ModContent.WallType<DampGrassWall>() && !Main.tile[X, Y].HasTile)
                        {
                            ushort[] WallWebs = new ushort[] { (ushort)ModContent.TileType<WallWeb1>(), (ushort)ModContent.TileType<WallWeb2>() };

                            WorldGen.PlaceObject(X, Y, WorldGen.genRand.Next(WallWebs));
                        }
                    }
                }
            }

            //place vines
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
            {
                for (int Y = 20; Y <= Main.maxTilesY - 20; Y++)
                {
                    //green spooky vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<DampVines>());
                        }
                    }
                    if (Main.tile[X, Y].TileType == ModContent.TileType<DampVines>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<DampVines>());
                    }
                }
            }
        }

        public static bool CanGrowGiantRoot(int X, int Y, int tileType, int minSize, int maxSize)
        {
            int canPlace = 0;

            //do not allow trees to place if another one is too close
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        canPlace++;
                        if (canPlace > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            GiantRoot.Grow(X, Y + 1, minSize, maxSize);

            return true;
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

            tasks.Insert(GenIndex1 + 1, new PassLegacy("SpiderCave", PlaceSpiderCave));
        }
    }
}