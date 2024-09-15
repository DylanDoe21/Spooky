using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Gourds;
using Spooky.Content.Tiles.SpookyBiome.Mushrooms;
using Spooky.Content.Tiles.SpookyBiome.Tree;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class SpookyForest : ModSystem
    {
        //default positions, edit based on worldsize below
        static int PositionX = Main.maxTilesX / 2;
        static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

        static int Size = Main.maxTilesX / 15;

        private void GenerateSpookyForest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpookyForest").Value;

            //decide whether or not to use the alt background
            if (WorldGen.genRand.NextBool(2))
            {
                Flags.SpookyBackgroundAlt = true;
            }
            else
            {
                Flags.SpookyBackgroundAlt = false;
            }

            //if config is enabled, place it at spawn
            if (ModContent.GetInstance<SpookyConfig>().SpookyForestSpawn)
            {
                PositionX = Main.maxTilesX / 2;
            }
            //otherwise place it in front of the dungeon
            else
            {
                PositionX = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;

                //attempt to find a valid position for the biome to place in
                bool foundValidPosition = false;
                int attempts = 0;

                //the biomes initial position is the very center of the snow biome
                //this code basically looks for snow biome blocks, and if it finds any, keep moving the biome over until it is far enough away from the snow biome
                while (!foundValidPosition && attempts++ < 100000)
                {
                    while (!NoSnowBiomeNearby(PositionX, PositionY))
                    {
                        PositionX += (PositionX > (Main.maxTilesX / 2) ? 100 : -100);
                    }
                    if (NoSnowBiomeNearby(PositionX, PositionY))
                    {
                        foundValidPosition = true;
                    }
                }
            }

            //set y position again so it is always correct before placing
            PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            //set size and height
            Size = Main.maxTilesX / 15;
            int BiomeHeight = Main.maxTilesY / 10;

            //place the actual biome
            for (int Y = 0; Y < BiomeHeight; Y += 50)
            {
                //loop to make the sides of the spooky forest more smooth
                for (int cutOff = 0; cutOff < Main.maxTilesX / 28; cutOff += 50)
                {
                    SpookyWorldMethods.ModifiedTileRunner(PositionX, PositionY + Y + cutOff, (double)Size + Y / 2, 1, 
                    ModContent.TileType<SpookyDirt>(), ModContent.WallType<SpookyGrassWall>(), true, 0f, 0f, true, true, true, false);
                }
            }

            //dig crater to lead to the underground
            for (int CraterDepth = PositionY; CraterDepth <= (int)Main.worldSurface + 55; CraterDepth += 5)
            {
                TileRunner runner = new TileRunner(new Vector2(PositionX - WorldGen.genRand.Next(45, 55), CraterDepth), new Vector2(0, 5), new Point16(-5, 5), 
                new Point16(-5, 5), 15f, WorldGen.genRand.Next(5, 10), 0, false, true);
                runner.Start();
            }
            
            //place clumps of stone in the underground
            for (int stone = 0; stone < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 12E-05); stone++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(35, 45), WorldGen.genRand.Next(35, 45), 
                        ModContent.TileType<SpookyStone>(), true, 0f, 0f, true, true);
                    }
                }
            }

            //place clumps of green grass using a temporary dirt tile clone that will be replaced later in generation
            for (int moss = 0; moss < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); moss++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                
                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    //surface clumps
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 20), WorldGen.genRand.Next(15, 20), 
                        ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
                    }

                    //bigger clumps underground
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 20), WorldGen.genRand.Next(15, 20), 
                        ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
                    }
                }
            }

            //generate caves
            for (int caves = 0; caves < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); caves++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                {
                    TileRunner runner = new TileRunner(new Vector2(X, Y), new Vector2(0, 5), new Point16(-35, 35), 
                    new Point16(-12, 12), 15f, WorldGen.genRand.Next(25, 50), 0, false, true);
                    runner.Start();
                }
            }

            //generate patches of vanilla glowing mushroom biomes, then convert them to spooky forest blocks
            int extraMushroomDepth = Main.maxTilesX >= 8400 ? 100 : (Main.maxTilesX >= 6400 ? 45 : 0);
            int mushroomDepth = ((int)Main.worldSurface + Main.maxTilesY / 9) + extraMushroomDepth;

            WorldGen.ShroomPatch(PositionX, mushroomDepth - 10);
            WorldGen.ShroomPatch(PositionX - 50, mushroomDepth);
            WorldGen.ShroomPatch(PositionX + 50, mushroomDepth);
            WorldGen.ShroomPatch(PositionX, mushroomDepth + 10);

            //convert the mushroom patch generation to spooky forest blocks
            for (int mushroomX = PositionX - 150; mushroomX <= PositionX + 150; mushroomX++)
            {
                for (int mushroomY = mushroomDepth - 75; mushroomY <= mushroomDepth + 75; mushroomY++)
                {
                    if (Main.tile[mushroomX, mushroomY].TileType == TileID.Mud)
                    {
                        if (!Main.tile[mushroomX - 1, mushroomY].HasTile || !Main.tile[mushroomX + 1, mushroomY].HasTile ||
                        !Main.tile[mushroomX, mushroomY - 1].HasTile || !Main.tile[mushroomX, mushroomY + 1].HasTile)
                        {
                            Main.tile[mushroomX, mushroomY].TileType = (ushort)ModContent.TileType<MushroomMoss>();
                        }
                        else
                        {
                            Main.tile[mushroomX, mushroomY].TileType = (ushort)ModContent.TileType<SpookyStone>();
                        }
                    }
                }
            }

            //place clumps of vanilla ores

            //determine which ores to place based on the opposite of what ores generate
            ushort OppositeTier1Ore = WorldGen.SavedOreTiers.Copper == TileID.Copper ? TileID.Tin : TileID.Copper;
            ushort OppositeTier2Ore = WorldGen.SavedOreTiers.Iron == TileID.Iron ? TileID.Lead : TileID.Iron;
            ushort OppositeTier3Ore = WorldGen.SavedOreTiers.Silver == TileID.Silver ? TileID.Tungsten : TileID.Silver;
            ushort OppositeTier4Ore = WorldGen.SavedOreTiers.Gold == TileID.Gold ? TileID.Platinum : TileID.Gold;

            for (int copper = 0; copper < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 10E-05); copper++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(5, 12), WorldGen.genRand.Next(5, 12), OppositeTier1Ore, false, 0f, 0f, false, true);
                }
            }

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(4, 10), OppositeTier2Ore, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 6E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), OppositeTier3Ore, false, 0f, 0f, false, true);
                }
            }

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), OppositeTier4Ore, false, 0f, 0f, false, true);
                }
            }
        }

        private void SpreadSpookyGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            //spread grass on all spooky dirt tiles
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
			{
                for (int Y = PositionY - 100; Y <= Main.maxTilesY - 100; Y++)
				{ 
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];
                    Tile tileLeft = Main.tile[X - 1, Y];
                    Tile tileRight = Main.tile[X + 1, Y];

                    if (tile.TileType == ModContent.TileType<SpookyDirt>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SpookyGrass>();
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt2>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt>() && (tileAbove.TileType == ModContent.TileType<SpookyGrass>() || tileBelow.TileType == ModContent.TileType<SpookyGrass>() || 
                    tileLeft.TileType == ModContent.TileType<SpookyGrass>() || tileRight.TileType == ModContent.TileType<SpookyGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt>(), ModContent.TileType<SpookyGrass>(), false);
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt2>() && (tileAbove.TileType == ModContent.TileType<SpookyGrassGreen>() || tileBelow.TileType == ModContent.TileType<SpookyGrassGreen>() || 
                    tileLeft.TileType == ModContent.TileType<SpookyGrassGreen>() || tileRight.TileType == ModContent.TileType<SpookyGrassGreen>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt2>(), ModContent.TileType<SpookyGrassGreen>(), false);
                    }
                }
            }
        }

        private void GrowSpookyTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            //grow trees
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
			{
                //regular surface trees
                for (int Y = 0; Y < (int)Main.worldSurface - 50; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrass>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }

                //grow giant mushrooms
                for (int Y = (int)Main.worldSurface + 25; Y < Main.maxTilesY - 200; Y++)
                {
                    if ((Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyStone>()) &&
                    !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                    {
                        if (WorldGen.genRand.NextBool(18))
                        {
                            GrowGiantMushroom(X, Y, ModContent.TileType<GiantShroom>(), 5, 8);
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<MushroomMoss>() && !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                    {
                        if (WorldGen.genRand.NextBool(5))
                        {
                            GrowGiantMushroom(X, Y, ModContent.TileType<GiantShroom>(), 6, 10);
                        }
                    }
                }
            }
        }

        private void SpookyForestAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            //place ambient objects
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
			{
                for (int Y = PositionY - 100; Y < Main.maxTilesY - 100; Y++)
                {  
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];
                    Tile tileLeft = Main.tile[X - 1, Y];
                    Tile tileRight = Main.tile[X + 1, Y];

                    //kill any single floating tiles so things dont look ugly
                    if (tile.TileType == ModContent.TileType<SpookyGrass>() || tile.TileType == ModContent.TileType<SpookyGrassGreen>() || tile.TileType == ModContent.TileType<SpookyStone>())
                    {
                        if (!tileAbove.HasTile && !tileBelow.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }
                    }

                    //convert leftover green grass dirt back into regular dirt
                    if (tile.TileType == ModContent.TileType<SpookyDirt2>())
                    {
                        tile.TileType = (ushort)ModContent.TileType<SpookyDirt>();
                    }

                    //orange spooky vines
                    if (tile.TileType == ModContent.TileType<SpookyGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVines>());
                        }
                    }
                    if (tile.TileType == ModContent.TileType<SpookyVines>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyVines>());
                    }

                    //green spooky vines
                    if (tile.TileType == ModContent.TileType<SpookyGrassGreen>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVinesGreen>());
                        }
                    }
                    if (tile.TileType == ModContent.TileType<SpookyVinesGreen>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyVinesGreen>());
                    }

                    //spooky fungus vines
                    if (tile.TileType == ModContent.TileType<MushroomMoss>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyFungusVines>());
                        }
                    }
                    if (tile.TileType == ModContent.TileType<SpookyFungusVines>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyFungusVines>());
                    }

                    //place gourds and weeds
                    if (tile.TileType == (ushort)ModContent.TileType<SpookyGrass>())
                    {
                        //gourds
                        if (WorldGen.genRand.NextBool(3) && CanGrowGourd(X, Y))
                        {
                            ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(), 
                            (ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(), 
                            (ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Gourds), true, WorldGen.genRand.Next(0, 2));
                        }

                        //grow weeds
                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyWeedsOrange>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                            WorldGen.SquareTileFrame(X, Y + 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
                            }
                        }
                    }
                    if (tile.TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
                    {
                        //gourds
                        if (WorldGen.genRand.NextBool(3) && CanGrowGourd(X, Y))
                        {
                            ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(), 
                            (ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(), 
                            (ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Gourds), true, WorldGen.genRand.Next(0, 2));
                        }

                        //grow weeds
                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyWeedsGreen>());
                            tileAbove.TileFrameY = 0;
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                            WorldGen.SquareTileFrame(X, Y + 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
                            }
                        }
                    }
                }
            }

            //place stuff underground
            for (int X = 20; X <= Main.maxTilesX - 20; X++)
			{
                for (int Y = (int)Main.worldSurface; Y < (int)Main.worldSurface + 250; Y++)
                { 
                    //grow hanging glow vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() || Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        if (WorldGen.genRand.NextBool(8))
                        {    
                            ushort[] Vines = new ushort[] { (ushort)ModContent.TileType<HangingVine1>(), 
                            (ushort)ModContent.TileType<HangingVine2>(), (ushort)ModContent.TileType<HangingVine3>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Vines));           
                        }

                        if (WorldGen.genRand.NextBool(5))
                        {
                            WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<MossyRock>());           
                        }
                    }

                    //place stuff on mushroom moss
                    if (Main.tile[X, Y].TileType == ModContent.TileType<MushroomMoss>())
                    {
                        //grow big mushrooms
                        if (WorldGen.genRand.NextBool(20))
                        {    
                            ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroom1>(), (ushort)ModContent.TileType<GiantShroom2>(), 
                            (ushort)ModContent.TileType<GiantShroom3>(), (ushort)ModContent.TileType<GiantShroom4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Shrooms));           
                        }

                        //grow big yellow mushrooms
                        if (WorldGen.genRand.NextBool(25))
                        {    
                            ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroomYellow1>(), (ushort)ModContent.TileType<GiantShroomYellow2>(), 
                            (ushort)ModContent.TileType<GiantShroomYellow3>(), (ushort)ModContent.TileType<GiantShroomYellow4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Shrooms));           
                        }

                        //place mushroom rock piles
                        if (WorldGen.genRand.NextBool(8))
                        {    
                            ushort[] RockPiles = new ushort[] { (ushort)ModContent.TileType<MushroomRockGiant>(), 
                            (ushort)ModContent.TileType<MushroomRockBig>(), (ushort)ModContent.TileType<MushroomRockSmall>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(RockPiles));       
                        }
                    }
                }
            }
        }

        public void ClearStuffAroundMushroomMoss(GenerationProgress progress, GameConfiguration configuration)
        {
            //statues and traps are annoying, so clear out everything from the mushroom area in the spooky forest
            for (int mushroomX = 20; mushroomX <= Main.maxTilesX - 20; mushroomX++)
			{
                for (int mushroomY = PositionY - 100; mushroomY < Main.maxTilesY / 2 - 50; mushroomY++)
                {
                    //whitelist so tiles meant to be on mushroom moss dont get cleared
                    int[] ClearWhitelist = { ModContent.TileType<MushroomMoss>(), ModContent.TileType<SpookyMushroom>(), 
                    ModContent.TileType<GiantShroom>(), ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(),
                    ModContent.TileType<SpookyDirt>(), ModContent.TileType<SpookyStone>(), ModContent.TileType<MushroomRockGiant>(), 
                    ModContent.TileType<MushroomRockBig>(), ModContent.TileType<MushroomRockSmall>(), ModContent.TileType<GiantShroom1>(),
                    ModContent.TileType<GiantShroom2>(), ModContent.TileType<GiantShroom3>(), ModContent.TileType<GiantShroom4>(),
                    ModContent.TileType<GiantShroomYellow1>(), ModContent.TileType<GiantShroomYellow2>(), ModContent.TileType<GiantShroomYellow3>(), 
                    ModContent.TileType<GiantShroomYellow4>() };

                    if (Main.tile[mushroomX, mushroomY].TileType == ModContent.TileType<MushroomMoss>() && !ClearWhitelist.Contains(Main.tile[mushroomX, mushroomY - 1].TileType))
                    {
                        WorldGen.KillTile(mushroomX, mushroomY - 1);
                    }

                    //also get rid of any liquids
                    if (Main.tile[mushroomX, mushroomY].TileType == ModContent.TileType<MushroomMoss>() && Main.tile[mushroomX, mushroomY - 1].LiquidAmount > 0)
                    {
                        for (int checkY = mushroomY; checkY >= mushroomY - 12; checkY--)
                        {
                            Main.tile[mushroomX, checkY].LiquidAmount = 0;
                        }
                    }
                }
            }
        }

        public void GenerateStarterHouse(GenerationProgress progress, GameConfiguration configuration)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                //place starter house
                int x = PositionX <= (Main.maxTilesX / 2) ? PositionX + ((Main.maxTilesX / 12) / 6) : PositionX - ((Main.maxTilesX / 12) / 6);
                int y = PositionY; //start here to not touch floating islands

                while ((!WorldGen.SolidTile(x, y) || !Cemetery.NoFloatingIsland(x, y)) && y <= Main.worldSurface)
				{
					y++;
				}
                if (WorldGen.SolidTile(x, y) && Cemetery.NoFloatingIsland(x, y))
				{
                    Vector2 origin = new Vector2(x - 10, y - 25);

                    //clear trees around the house since it is placed after them
                    for (int i = (int)origin.X - 15; i <= (int)origin.X + 15; i++)
                    {
                        for (int j = (int)origin.Y - 50; j <= (int)origin.Y + 50; j++)
                        {
                            if (Main.tile[i, j].TileType == 5)
                            {
                                WorldGen.KillTile(i, j);
                            }
                        }
                    }

                    //place starter house
                    Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestHouse", origin.ToPoint16(), Mod);

                    //place little bone in the house
                    NPC.NewNPC(null, (x + 1) * 16, (y - 9) * 16, ModContent.NPCType<LittleBoneSleeping>());

                    placed = true;
				}
            }
        }

        public void GenerateUndergroundCabins(GenerationProgress progress, GameConfiguration configuration)
        {
            //how much distance should be inbetween each loot room
            int ChestDistance = (Main.maxTilesX / 75);

            //depth of each loot room
            int InitialDepth = (int)Main.worldSurface + (Main.maxTilesY / 28);
            int ChestDepth = (Main.maxTilesY / 30) / 2;

            //actual loot room positions
            int x = PositionX;
            int y = InitialDepth + (ChestDepth + 35);

            int extraChestDepth = Main.maxTilesX >= 6400 ? 45 : 25;

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + extraChestDepth);
            Vector2 origin1 = new Vector2((x - (ChestDistance * 2)) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin-1", origin1.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + extraChestDepth);
            Vector2 origin2 = new Vector2(((x - ChestDistance) - 8) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin-2", origin2.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + extraChestDepth);
            Vector2 origin3 = new Vector2(x - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin-3", origin3.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + extraChestDepth);
            Vector2 origin4 = new Vector2((x + ChestDistance) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin-4", origin4.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + extraChestDepth);
            Vector2 origin5 = new Vector2((x + (ChestDistance * 2)) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin-5", origin5.ToPoint16(), Mod);
        }

        public static bool GrowGiantMushroom(int X, int Y, int tileType, int minSize, int maxSize)
        {
            int canPlace = 0;

            //do not allow giant mushrooms to place if another one is too close
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

            //make sure the area is large enough for it to place in both horizontally and vertically
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 12; j < Y - 2; j++)
                {
                    //only check for solid blocks, ambient objects dont matter
                    if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        canPlace++;
                        if (canPlace > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            GiantShroom.Grow(X, Y - 1, minSize, maxSize, false);

            return true;
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool NoSnowBiomeNearby(int X, int Y)
        {
            for (int i = X - 300; i < X + 300; i++)
            {
                for (int j = Y; j < Y + 300; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.SnowBlock || Main.tile[i, j].TileType == TileID.IceBlock))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //make sure normal gourds cant place close to each other
        public static bool CanGrowGourd(int X, int Y)
        {
            ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(), 
            (ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(), 
            (ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

            for (int i = X - 12; i < X + 12; i++)
            {
                for (int j = Y - 7; j < Y + 7; j++)
                {
                    if (Main.tile[i, j].HasTile && Gourds.Contains(Main.tile[i, j].TileType))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //make sure rotten gourds cannot place too close to each other
        public static bool CanGrowRottenGourd(int X, int Y)
        {
            for (int i = X - 25; i < X + 25; i++)
            {
                for (int j = Y - 25; j < Y + 25; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<GourdRotten>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
            //generate biome
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spooky Forest", GenerateSpookyForest));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("Little Bone House", GenerateStarterHouse));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("Spooky Forest Grass", SpreadSpookyGrass));

            //place house again because stupid ahh walls
            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (GenIndex2 == -1)
			{
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spooky Forest Cabins", GenerateUndergroundCabins));
            tasks.Insert(GenIndex2 + 2, new PassLegacy("Spooky Forest Grass Again", SpreadSpookyGrass));
            tasks.Insert(GenIndex2 + 3, new PassLegacy("Glowshroom Cleanup", ClearStuffAroundMushroomMoss));
            tasks.Insert(GenIndex2 + 4, new PassLegacy("Spooky Forest Trees", GrowSpookyTrees));
            tasks.Insert(GenIndex2 + 5, new PassLegacy("Spooky Forest Ambient Tiles", SpookyForestAmbience));
        }

        //post worldgen to place items in the spooky biome chests
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

                if (chestTile.TileType == ModContent.TileType<OldWoodChest>())
                {
                    int[] Bars = new int[] { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar };
                    int[] LightSources = new int[] { ItemID.OrangeTorch, ModContent.ItemType<CandleItem>() };
                    int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };

                    //iron or lead bars
                    chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
                    chest.item[1].stack = WorldGen.genRand.Next(5, 10);
                    //light sources
                    chest.item[2].SetDefaults(WorldGen.genRand.Next(LightSources));
                    chest.item[2].stack = WorldGen.genRand.Next(3, 8);
                    //potions
                    chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions));
                    chest.item[3].stack = WorldGen.genRand.Next(2, 3);
                    //goodie bags
                    chest.item[4].SetDefaults(ItemID.GoodieBag);
                    chest.item[4].stack = WorldGen.genRand.Next(1, 2);
                    //coins
                    chest.item[5].SetDefaults(ItemID.GoldCoin);
                    chest.item[5].stack = WorldGen.genRand.Next(1, 2);
                }
            }
        }
    }
}