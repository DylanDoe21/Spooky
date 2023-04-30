using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    //TODO list for catacomb update:
    //make the cemetery get longer based on worldsize, and make the catacombs get deeper based on worldsize
    //make the structures in the cemetery more spread out based on worldsize
    //make the cemetery generate further from the ocean based on worldsize
    //make new brick block for the cemetery structures or have it just use the catacomb layer one bricks
    public class Cemetery : ModSystem
    {
        //place a giant dirt area for the graveyard to generate on
        private void PlaceCemeteryArea(GenerationProgress progress, GameConfiguration configuration)
        {
            LocalizedText Description = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Cemetery");
            progress.Message = Description.Value;

            Catacombs.PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            //place biome based on opposite dungeon side
            if (GenVars.dungeonSide == -1)
			{
                Catacombs.PositionX = Main.maxTilesX - 850;
			}
			else
			{
                Catacombs.PositionX = 400;
            }

            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (Catacombs.BiomeWidth / 2);
            int XEdge = XStart + Catacombs.BiomeWidth;

            bool foundSurface = false;
            int attempts = 0;

            //place biome exactly on the surface
            while (!foundSurface && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(XMiddle, Catacombs.PositionY) && Catacombs.PositionY <= Main.worldSurface)
				{
					Catacombs.PositionY++;
				}
                if (!Main.tile[XMiddle, Catacombs.PositionY].HasTile)
                {
					continue;
				}

                foundSurface = true;
            }

            for (int X = XMiddle - Catacombs.BiomeWidth / 2; X <= XMiddle + Catacombs.BiomeWidth / 2; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileUp = Main.tile[X, Y - 1];
                    Tile tileDown = Main.tile[X, Y + 1];
                    Tile tileLeft = Main.tile[X - 1, Y];
                    Tile tileRight = Main.tile[X + 1, Y];

                    //place dirt blocks
                    if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                    {
                        ShapeData circle = new ShapeData();
                        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                        int radius = WorldGen.genRand.Next(2, 3);
                        WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                        {
                            blotchMod.Output(circle)
                        }));

                        WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(), new Actions.ClearWall(),
                            new Actions.PlaceTile((ushort)ModContent.TileType<CemeteryDirt>())
                        }));
                    }

                    //place dirt blocks where walls exist to prevent unwanted craters or caves
                    if (tile.WallType > 0 && !tile.HasTile)
                    {
                        ShapeData circle = new ShapeData();
                        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                        int radius = WorldGen.genRand.Next(2, 3);
                        WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                        {
                            blotchMod.Output(circle)
                        }));

                        WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                        {
                            new Actions.ClearTile(), new Actions.ClearWall(),
                            new Actions.PlaceTile((ushort)ModContent.TileType<CemeteryDirt>())
                        }));
                    }

                    //fill in any single empty tiles
                    if (!tile.HasTile && tileUp.HasTile && tileDown.HasTile && tileLeft.HasTile && tileRight.HasTile)
                    {
                        tile.ClearEverything();
                        WorldGen.PlaceTile(X, Y, ModContent.TileType<CemeteryDirt>());
                    }
                }

                //fill in right above the world surface to prevent weird holes that just get stopped by the catacombs
                for (int FillY = (int)Main.worldSurface - 10; FillY <= Main.worldSurface; FillY++)
                {
                    ShapeData circle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                    int radius = WorldGen.genRand.Next(2, 3);
                    WorldUtils.Gen(new Point(X, FillY), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));

                    WorldUtils.Gen(new Point(X, FillY), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile(), new Actions.ClearWall(),
                        new Actions.PlaceTile((ushort)ModContent.TileType<CemeteryDirt>())
                    }));
                }
            }

            //place clumps of stone
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-04); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY - 2);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<CemeteryDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(10, 18), WorldGen.genRand.Next(10, 18), 
                        ModContent.TileType<CemeteryStone>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        private void SpreadCemeteryGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (Catacombs.BiomeWidth / 2);
            int XEdge = XStart + Catacombs.BiomeWidth;

            //spread grass on all cemetery dirt tiles
            for (int X = XMiddle - (Catacombs.BiomeWidth / 2) - 100; X <= XMiddle + (Catacombs.BiomeWidth / 2) + 100; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile up = Main.tile[X, Y - 1];
                    Tile down = Main.tile[X, Y + 1];
                    Tile left = Main.tile[X - 1, Y];
                    Tile right = Main.tile[X + 1, Y];

                    //convert grass blocks that are covered up back into dirt
                    if (tile.TileType == ModContent.TileType<CemeteryGrass>() && up.HasTile && down.HasTile && left.HasTile && right.HasTile)
                    {
                        tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                    }

                    //convert grass
                    if (tile.TileType == ModContent.TileType<CemeteryDirt>() &&
                    ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
                    {
                        tile.TileType = (ushort)ModContent.TileType<CemeteryGrass>();
                    }
 
                    //extra spread grass just in case
                    if (tile.TileType == ModContent.TileType<CemeteryDirt>() &&
                    (up.TileType == ModContent.TileType<CemeteryGrass>() || down.TileType == ModContent.TileType<CemeteryGrass>() ||
                    left.TileType == ModContent.TileType<CemeteryGrass>() || right.TileType == ModContent.TileType<CemeteryGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<CemeteryDirt>(), ModContent.TileType<CemeteryGrass>(), false);
                    }

                    //place bushes
                    if (tile.TileType == ModContent.TileType<CemeteryDirt>() || tile.TileType == ModContent.TileType<CemeteryGrass>())
                    {
                        if (WorldGen.genRand.Next(12) == 0)
                        {
                            ushort[] Bushes = new ushort[] { (ushort)ModContent.TileType<CemeteryBush1>(), (ushort)ModContent.TileType<CemeteryBush2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Bushes));
                        }
                    }
                }
            }
        }

        private void GrowCemeteryTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (Catacombs.BiomeWidth / 2);
            int XEdge = XStart + Catacombs.BiomeWidth;

            //spread grass on all cemetery dirt tiles
            for (int X = XMiddle - (Catacombs.BiomeWidth / 2) - 100; X <= XMiddle + (Catacombs.BiomeWidth / 2) + 100; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<CemeteryGrass>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }
            }
        }

        public void GenerateCemeteryStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (Catacombs.BiomeWidth / 2);
            int XEdge = XStart + Catacombs.BiomeWidth;

            //first hut
            bool placedHut1 = false;
            int hut1Attempts = 0;
            while (!placedHut1 && hut1Attempts++ < 100000)
            {
                int HutX = XMiddle - 175;
                int HutY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(HutX, HutY) && HutY <= Main.worldSurface)
				{
					HutY++;
				}
                if (!Main.tile[HutX, HutY].HasTile)
                {
					continue;
                }

                ClearAreaAboveStructure(HutX, HutY - 20);
                PlaceBlocksBelowStructure(HutX, HutY, 7);

                Vector2 origin = new Vector2(HutX - 14, HutY - 15);
                Generator.GenerateStructure("Content/Structures/CemeteryHut1", origin.ToPoint16(), Mod);

                placedHut1 = true;
            }

            //first hole
            bool placedHole1 = false;
            int hole1Attempts = 0;
            while (!placedHole1 && hole1Attempts++ < 100000)
            {
                int HoleX = XMiddle - 100;
                int HoleY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(HoleX, HoleY) && HoleY <= Main.worldSurface)
				{
					HoleY++;
				}
                if (!Main.tile[HoleX, HoleY].HasTile)
                {
					continue;
                }

                ClearAreaAboveStructure(HoleX, HoleY - 27);
                PlaceBlocksBelowStructure(HoleX, HoleY, 15);

                Vector2 origin = new Vector2(HoleX - 22, HoleY - 22);
                Generator.GenerateStructure("Content/Structures/CemeteryHole1", origin.ToPoint16(), Mod);

                placedHole1 = true;
            }

            //crypt
            bool placedCrypt = false;
            int cryptAttempts = 0;
            while (!placedCrypt && cryptAttempts++ < 100000)
            {
                int CryptX = XMiddle;
                int CryptY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(CryptX, CryptY) && CryptY <= Main.worldSurface)
				{
					CryptY++;
				}
                if (!Main.tile[CryptX, CryptY].HasTile)
                {
					continue;
                }

                PlaceBlocksBelowStructure(CryptX, CryptY, 20);

                Vector2 origin = new Vector2(CryptX - 28, CryptY - 37);
                Generator.GenerateStructure("Content/Structures/CemeteryCrypt", origin.ToPoint16(), Mod);

                //set the catacomb entrance position so it places the tunnel down to the catacombs properly
                Catacombs.EntranceY = CryptY - 37;

                placedCrypt = true;
            }

            //lake
            bool placedlake = false;
            int lakeAttempts = 0;
            while (!placedlake && lakeAttempts++ < 100000)
            {
                int LakeX = XMiddle + 75;
                int LakeY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(LakeX, LakeY) && LakeY <= Main.worldSurface)
				{
					LakeY++;
				}
                if (!Main.tile[LakeX, LakeY].HasTile)
                {
					continue;
                }

                ClearAreaAboveStructure(LakeX, LakeY - 23);
                PlaceBlocksBelowStructure(LakeX, LakeY, 15);

                Vector2 origin = new Vector2(LakeX - 19, LakeY - 18);
                Generator.GenerateStructure("Content/Structures/CemeteryLake", origin.ToPoint16(), Mod);

                placedlake = true;
            }

            //second hole
            bool placedHole2 = false;
            int hole2Attempts = 0;
            while (!placedHole2 && hole2Attempts++ < 100000)
            {
                int HoleX = XMiddle + 135;
                int HoleY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(HoleX, HoleY) && HoleY <= Main.worldSurface)
				{
					HoleY++;
				}
                if (!Main.tile[HoleX, HoleY].HasTile)
                {
					continue;
                }

                ClearAreaAboveStructure(HoleX, HoleY - 27);
                PlaceBlocksBelowStructure(HoleX, HoleY, 15);

                Vector2 origin = new Vector2(HoleX - 22, HoleY - 22);
                Generator.GenerateStructure("Content/Structures/CemeteryHole2", origin.ToPoint16(), Mod);

                placedHole2 = true;
            }

            //second hut
            bool placedHut2 = false;
            int hut2Attempts = 0;
            while (!placedHut2 && hut2Attempts++ < 100000)
            {
                int HutX = XMiddle + 185;
                int HutY = Catacombs.PositionY - 50;

                while (!WorldGen.SolidTile(HutX, HutY) && HutY <= Main.worldSurface)
				{
					HutY++;
				}
                if (!Main.tile[HutX, HutY].HasTile)
                {
					continue;
                }

                ClearAreaAboveStructure(HutX, HutY - 16);
                PlaceBlocksBelowStructure(HutX, HutY, 7);

                Vector2 origin = new Vector2(HutX - 16, HutY - 11);
                Generator.GenerateStructure("Content/Structures/CemeteryHut2", origin.ToPoint16(), Mod);

                placedHut2 = true;
            }

            /*
            //clear extra dirt and grass because i accidentally removed tiles that the structure files had in them
            //i am not re-exporting the structures so this is going to have to do
            for (int X = XMiddle - Catacombs.BiomeWidth / 2; X <= XMiddle + Catacombs.BiomeWidth / 2; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];

                    if (tile.TileType == TileID.Grass || tile.TileType == TileID.Dirt)
                    {
                        WorldGen.KillTile(X, Y);
                    }
                }
            }
            */
        }

        public static void ClearAreaAboveStructure(int x, int y)
        {
            for (int i = x - 25; i <= x + 25; i++)
            {
                for (int j = y - 50; j <= y; j++)
                {
                    ShapeData circle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                    WorldUtils.Gen(new Point(i, j), new Shapes.Circle(2), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));

                    WorldUtils.Gen(new Point(i, j), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile(), new Actions.ClearWall()
                    }));
                }
            }
        }

        public static void PlaceBlocksBelowStructure(int x, int y, int width)
        {
            for (int i = x - width; i <= x + width; i++)
            {
                for (int j = y; j <= y + 35; j++)
                {
                    ShapeData circle = new ShapeData();
                    GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                    WorldUtils.Gen(new Point(i, j), new Shapes.Circle(2), Actions.Chain(new GenAction[]
                    {
                        blotchMod.Output(circle)
                    }));

                    WorldUtils.Gen(new Point(i, j), new ModShapes.All(circle), Actions.Chain(new GenAction[]
                    {
                        new Actions.ClearTile(), new Actions.ClearWall(),
                        new Actions.PlaceTile((ushort)ModContent.TileType<CemeteryDirt>())
                    }));
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Rock Wall Runner"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("CemeteryTerrain", PlaceCemeteryArea));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("CemeteryStructures", GenerateCemeteryStructures));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("CemeteryGrass", SpreadCemeteryGrass));
            tasks.Insert(GenIndex1 + 4, new PassLegacy("CemeteryTrees", GrowCemeteryTrees));
        }
    }
}