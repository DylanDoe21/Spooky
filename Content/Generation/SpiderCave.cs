using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Tree;
using Spooky.Content.Tiles.SpookyBiome;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class SpiderCave : ModSystem
    {
        private void PlaceSpiderCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpiderCave").Value;

            //biome position stuff
            int SnowMiddle = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;

            int BiomeCenterX = (SnowMiddle + (Main.maxTilesX / 2)) / 2;

            int ExtraHeight = WorldGen.genRand.Next(20, 55);

            int startPosX = BiomeCenterX + (BiomeCenterX < (Main.maxTilesX / 2) ? -(Main.maxTilesX / 25) : (Main.maxTilesX / 25));
            int startPosY = (Main.maxTilesY - (Main.maxTilesY / 3)) - ExtraHeight;

            int cavePerlinSeed = WorldGen.genRand.Next();
            int cavePerlinSeedWalls = WorldGen.genRand.Next();

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int biomeSize = 250 + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            //first, place a large barrier of stone along where the bottom of the biome will be
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
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
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
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
                        float caveCreationWallThreshold = horizontalOffsetNoise * 3.5f + 0.335f;

                        //kill or place tiles depending on the noise map
                        if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
                        {
                            WorldGen.KillTile(X, Y);
                        }
                        else
                        {
                            WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<DampSoil>());
                        }

                        //place a layer of grass walls around blocks
                        if (caveNoiseMap * caveNoiseMap <= caveCreationWallThreshold)
                        {
                            WorldGen.PlaceWall(X, Y, ModContent.WallType<DampGrassWall>());
                        }
                    }
                }
            }

            //after the main biome is done, generate some more smaller features
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
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
                            WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(25, 35), ModContent.TileType<SpiderStone>(), false, 0f, 0f, true, true);
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
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
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

            //place clumps of vanilla ores
            //spider grotto is deeper underground, so the tier 1 ore doesnt need to generate here

            //determine which ores to place based on the opposite of what ores generate
            ushort OppositeTier2Ore = WorldGen.SavedOreTiers.Iron == TileID.Iron ? TileID.Lead : TileID.Iron;
            ushort OppositeTier3Ore = WorldGen.SavedOreTiers.Silver == TileID.Silver ? TileID.Tungsten : TileID.Silver;
            ushort OppositeTier4Ore = WorldGen.SavedOreTiers.Gold == TileID.Gold ? TileID.Platinum : TileID.Gold;

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpiderStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(4, 10), OppositeTier2Ore, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 6E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpiderStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), OppositeTier3Ore, false, 0f, 0f, false, true);
                }
            }

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpiderStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), OppositeTier4Ore, false, 0f, 0f, false, true);
                }
            }

            //generate structures
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) + 10; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //ground structures
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X - 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                        Main.tile[X - 2, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X + 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                        Main.tile[X + 2, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y - 2].HasTile)
                        {
                            if (WorldGen.genRand.NextBool(35) && CanPlaceStructure(X, Y))
                            {
                                switch (WorldGen.genRand.Next(12))
                                {
                                    case 0:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 12, Y - 13);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HouseSmall", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 1:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 13, Y - 25);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HouseMedium", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 2:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 17, Y - 23);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HouseLarge", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 3:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 21, Y - 28);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HouseGiant", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 4:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 12, Y - 14);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/GraveSmall", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 5:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 15, Y - 16);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/GraveLarge", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 6:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 18, Y - 22);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsSmall", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 7:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 15, Y - 25);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsMedium", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 8:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 12, Y - 23);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/RuinsLarge", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 9:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 9, Y - 20);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/TowerSmall", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 10:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 9, Y - 26);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/TowerLarge", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 11:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 10, Y - 15);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/SmallShrine", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                }
                            }
                        }

                        //ceiling structures
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X - 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                        Main.tile[X - 2, Y].TileType == ModContent.TileType<DampSoil>() && Main.tile[X + 1, Y].TileType == ModContent.TileType<DampSoil>() &&
                        Main.tile[X + 2, Y].TileType == ModContent.TileType<DampSoil>() && !Main.tile[X, Y + 1].HasTile && !Main.tile[X, Y + 2].HasTile)
                        {
                            if (WorldGen.genRand.NextBool(40) && CanPlaceStructure(X, Y))
                            {
                                switch (WorldGen.genRand.Next(5))
                                {
                                    case 0:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 7, Y - 4);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerSmall", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 1:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 7, Y - 4);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerLarge", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 2:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 18, Y - 8);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HangingTowerGiant", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 3:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 10, Y - 5);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HangingLootRoom", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                    case 4:
                                    {
                                        Vector2 structureOrigin = new Vector2(X - 16, Y - 6);
                                        Generator.GenerateStructure("Content/Structures/SpiderCave/HangingLootWeb", structureOrigin.ToPoint16(), Mod);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //spread grass after placing structures, but before placing ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //spread grass onto the dirt blocks throughout the biome
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<DampSoil>(), ModContent.TileType<DampGrass>(), false);
                    }
                }
            }

            //place ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
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
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>() || Main.tile[X, Y].TileType == ModContent.TileType<SpiderStone>())
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
                            if (WorldGen.genRand.NextBool(20))
                            {
                                ushort[] LightRoots = new ushort[] { (ushort)ModContent.TileType<LightRoot1>(), (ushort)ModContent.TileType<LightRoot2>(), 
                                (ushort)ModContent.TileType<LightRoot3>(), (ushort)ModContent.TileType<LightRoot4>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(LightRoots));
                            }

                            //light root on the ceiling
                            if (WorldGen.genRand.NextBool(15))
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
                                ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBrown1>(), (ushort)ModContent.TileType<MushroomBrown2>(), 
                                (ushort)ModContent.TileType<MushroomBrown3>(), (ushort)ModContent.TileType<MushroomBrown4>(), 
                                (ushort)ModContent.TileType<MushroomRed1>(), (ushort)ModContent.TileType<MushroomRed2>(), 
                                (ushort)ModContent.TileType<MushroomRed3>(), (ushort)ModContent.TileType<MushroomRed4>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Mushrooms));
                            }

                            /*
                            //giant mushrooms
                            if (WorldGen.genRand.NextBool(6))
                            {
                                ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroomGreen1>(), (ushort)ModContent.TileType<GiantShroomGreen2>(), 
                                (ushort)ModContent.TileType<GiantShroomOrange1>(), (ushort)ModContent.TileType<GiantShroomOrange2>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Mushrooms));
                            }
                            */
                        }

                        //place spider webs on walls
                        if (WorldGen.genRand.NextBool(120) && Main.tile[X, Y].WallType == ModContent.WallType<DampGrassWall>() && !Main.tile[X, Y].HasTile)
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

        private void DeleteAnnoyingTraps(GenerationProgress progress, GameConfiguration configuration)
        {
            //biome position stuff
            int SnowMiddle = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;

            int BiomeCenterX = (SnowMiddle + (Main.maxTilesX / 2)) / 2;

            int startPosX = BiomeCenterX + (BiomeCenterX < (Main.maxTilesX / 2) ? -(Main.maxTilesX / 25) : (Main.maxTilesX / 25));
            int startPosY = (Main.maxTilesY - (Main.maxTilesY / 3)) - 30;

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int biomeSize = 260 + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            //first, place a large barrier of stone along where the bottom of the biome will be
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideCircle(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        WorldGen.KillWire(X, Y);
                        Main.tile[X, Y].LiquidAmount = 0;
                    }
                }
            }
        }

        //determine if a structure can be placed at a set position
        public static bool CanPlaceStructure(int X, int Y)
        {
            int structureNearby = 0;

            //check a 70 by 70 square for other structures before placing
            for (int i = X - 35; i < X + 35; i++)
            {
                for (int j = Y - 35; j < Y + 35; j++)
                {
                    //if any mossy stone bricks are found in the square then another structure is too close, so dont allow it to place
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<SpookyStoneBricks>())
                    {
                        structureNearby++;
                        if (structureNearby > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        //determine if a giant root can be grown on a set block
        public static bool CanGrowGiantRoot(int X, int Y, int tileType, int minSize, int maxSize)
        {
            int canPlace = 0;

            //check a 10 by 10 square for other giant roots before placing
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    //if another root is found in the square, then dont allow the new root to be placed
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

            //place the root itself if no other roots are found in the area
            GiantRoot.Grow(X, Y + 1, minSize, maxSize);

            return true;
        }

        //method to make sure things only generate in the biome's circle
        public static bool CheckInsideCircle(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance)
        {
            Vector2 point = tile.ToWorldCoordinates();
            float distY = center.Y - point.Y;

            //squish the circle vertically to create an oval shape
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

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spider Grotto", PlaceSpiderCave));

            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
            if (GenIndex2 == -1)
            {
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spider Grotto Trap Removal", DeleteAnnoyingTraps));
        }

        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null) 
                {
					continue;
				}

				Tile chestTile = Main.tile[chest.x, chest.y];

                int[] MainItems = new int[] { ItemID.BandofRegeneration, ItemID.AnkletoftheWind, ItemID.HermesBoots, ItemID.CloudinaBottle, ItemID.Aglet, ItemID.LuckyHorseshoe };

                if (chestTile.TileFrameX == 28 * 36 && MainItems.Contains(chest.item[0].type))
                {
                    //potions
                    int[] Potions1 = new int[] { ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

                    //more potions
                    int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

                    //recorvery potions
                    int[] RecoveryPotions = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };

                    //bars
                    int[] Bars = new int[] { ItemID.GoldBar, ItemID.PlatinumBar };

                    //bars
                    chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
                    chest.item[1].stack = WorldGen.genRand.Next(5, 16);
                    //potions
                    chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions1));
                    chest.item[2].stack = WorldGen.genRand.Next(1, 3);
                    //even more potions
                    chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions2));
                    chest.item[3].stack = WorldGen.genRand.Next(1, 3);
                    //recovery potions
                    chest.item[4].SetDefaults(WorldGen.genRand.Next(RecoveryPotions));
                    chest.item[4].stack = WorldGen.genRand.Next(3, 7);
                    //goodie bags
                    chest.item[5].SetDefaults(ItemID.GoodieBag);
                    chest.item[5].stack = WorldGen.genRand.Next(1, 3);
                    //gold coins
                    chest.item[6].SetDefaults(ItemID.GoldCoin);
                    chest.item[6].stack = WorldGen.genRand.Next(1, 3);
                }
            }
        }
    }
}