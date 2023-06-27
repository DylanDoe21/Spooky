using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Cemetery.Ambient;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class Cemetery : ModSystem
    {
        public static int BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

        //place a giant dirt area for the graveyard to generate on
        private void PlaceCemeteryArea(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Cemetery").Value;

            Catacombs.PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            float worldEdgeOffset = Main.maxTilesX >= 6400 ? 8.75f : 8.75f;

            //place biome based on the opposite side of the dungeon
            if (GenVars.dungeonSide == -1)
			{
                Catacombs.PositionX = Main.maxTilesX - (Main.maxTilesX / (int)worldEdgeOffset);
			}
			else
			{
                Catacombs.PositionX = (Main.maxTilesX / (int)worldEdgeOffset);
            }

            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            bool foundSurface = false;
            int attempts = 0;

            //place biome exactly on the surface by finding a valid surface
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

            //place the terrain itself and replace blocks with cemetery blocks
            for (int X = XMiddle - (BiomeWidth / 2); X <= XMiddle + (BiomeWidth / 2); X++)
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
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                    }

                    //place dirt blocks where walls exist to prevent unwanted craters or caves
                    if (tile.WallType > 0 && !tile.HasTile)
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                    }

                    //fill in any single empty tiles
                    if (!tile.HasTile && tileUp.HasTile && tileDown.HasTile && tileLeft.HasTile && tileRight.HasTile)
                    {
                        tile.ClearEverything();
                        WorldGen.PlaceTile(X, Y, ModContent.TileType<CemeteryDirt>());
                    }
                }

                //fill in right above the world surface to prevent weird holes that just get stopped by the catacombs
                for (int FillY = (int)Main.worldSurface - 35; FillY <= Main.worldSurface; FillY++)
                {
                    SpookyWorldMethods.PlaceCircle(X, FillY, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //place clumps of stone in the biome
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
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            //spread grass on all cemetery dirt tiles
            for (int X = XMiddle - (BiomeWidth / 2) - 100; X <= XMiddle + (BiomeWidth / 2) + 100; X++)
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
                    if (tile.TileType == ModContent.TileType<CemeteryDirt>() && ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
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
                        if (WorldGen.genRand.NextBool(12))
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
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            for (int X = XMiddle - (BiomeWidth / 2) - 100; X <= XMiddle + (BiomeWidth / 2) + 100; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    //grow trees on cemetery grass
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
            int XMiddle = XStart + (BiomeWidth / 2);

            //first hut
            bool placedHut1 = false;
            int hut1Attempts = 0;
            while (!placedHut1 && hut1Attempts++ < 100000 && Main.maxTilesX >= 6400)
            {
                int HutX = XMiddle - 175;
                int HutY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(HutX, HutY) && HutY <= Main.worldSurface)
				{
					HutY++;
				}
                if (!Main.tile[HutX, HutY].HasTile || Main.tile[HutX, HutY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                ClearAreaAboveStructure(HutX, HutY - 20);
                PlaceBlocksBelowStructure(HutX, HutY + 3, 25);

                Vector2 origin = new Vector2(HutX - 14, HutY - 15);
                Generator.GenerateStructure("Content/Structures/CemeteryHut1", origin.ToPoint16(), Mod);

                placedHut1 = true;
            }

            //first burial pit
            bool placedHole1 = false;
            int hole1Attempts = 0;
            while (!placedHole1 && hole1Attempts++ < 100000)
            {
                int HoleX = XMiddle - 100;
                int HoleY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(HoleX, HoleY) && HoleY <= Main.worldSurface)
				{
					HoleY++;
				}
                if (!Main.tile[HoleX, HoleY].HasTile || Main.tile[HoleX, HoleY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                ClearAreaAboveStructure(HoleX, HoleY - 27);
                PlaceBlocksBelowStructure(HoleX, HoleY - 2, 30);

                Vector2 origin = new Vector2(HoleX - 22, HoleY - 22);
                Generator.GenerateStructure("Content/Structures/CemeteryHole1", origin.ToPoint16(), Mod);

                placedHole1 = true;
            }

            //crypt
            bool placedCrypt = false;
            int cryptAttempts = 0;
            while (!placedCrypt && cryptAttempts++ < 100000)
            {
                int CryptX = XMiddle - 3;
                int CryptY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(CryptX, CryptY) && CryptY <= Main.worldSurface)
				{
					CryptY++;
				}
                if (!Main.tile[CryptX, CryptY].HasTile || Main.tile[CryptX, CryptY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                PlaceBlocksBelowStructure(CryptX, CryptY, 40);

                Vector2 origin = new Vector2(CryptX - 28, CryptY - 37);
                Generator.GenerateStructure("Content/Structures/CemeteryCrypt", origin.ToPoint16(), Mod);

                //set the catacomb entrance position so it places the tunnel down to the catacombs properly
                Catacombs.EntranceY = CryptY - 37;

                placedCrypt = true;
            }

            //fishing lake
            bool placedlake = false;
            int lakeAttempts = 0;
            while (!placedlake && lakeAttempts++ < 100000)
            {
                int LakeX = XMiddle + 75;
                int LakeY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(LakeX, LakeY) && LakeY <= Main.worldSurface)
				{
					LakeY++;
				}
                if (!Main.tile[LakeX, LakeY].HasTile || Main.tile[LakeX, LakeY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                ClearAreaAboveStructure(LakeX, LakeY - 23);
                PlaceBlocksBelowStructure(LakeX, LakeY, 22);

                Vector2 origin = new Vector2(LakeX - 19, LakeY - 16);
                Generator.GenerateStructure("Content/Structures/CemeteryLake", origin.ToPoint16(), Mod);

                placedlake = true;
            }

            //second burial pit
            bool placedHole2 = false;
            int hole2Attempts = 0;
            while (!placedHole2 && hole2Attempts++ < 100000 && Main.maxTilesX >= 6400)
            {
                int HoleX = XMiddle + 135;
                int HoleY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(HoleX, HoleY) && HoleY <= Main.worldSurface)
                {
                    HoleY++;
				}
                if (!Main.tile[HoleX, HoleY].HasTile || Main.tile[HoleX, HoleY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                ClearAreaAboveStructure(HoleX, HoleY - 27);
                PlaceBlocksBelowStructure(HoleX, HoleY - 2, 30);

                Vector2 origin = new Vector2(HoleX - 22, HoleY - 22);
                Generator.GenerateStructure("Content/Structures/CemeteryHole2", origin.ToPoint16(), Mod);

                placedHole2 = true;
            }

            //second hut
            bool placedHut2 = false;
            int hut2Attempts = 0;
            while (!placedHut2 && hut2Attempts++ < 100000 && Main.maxTilesX >= 6400)
            {
                int HutX = XMiddle + 185;
                int HutY = Catacombs.PositionY - 55;

                while (!WorldGen.SolidTile(HutX, HutY) && HutY <= Main.worldSurface)
				{
					HutY++;
				}
                if (!Main.tile[HutX, HutY].HasTile || Main.tile[HutX, HutY].WallType == WallID.EbonstoneUnsafe)
                {
					continue;
                }

                ClearAreaAboveStructure(HutX, HutY - 16);
                PlaceBlocksBelowStructure(HutX, HutY + 3, 10);

                Vector2 origin = new Vector2(HutX - 16, HutY - 11);
                Generator.GenerateStructure("Content/Structures/CemeteryHut2", origin.ToPoint16(), Mod);

                placedHut2 = true;
            }
        }

        public static void ClearAreaAboveStructure(int x, int y)
        {
            //clear tiles above structures to prevent clumps of floating blocks
            for (int i = x - 25; i <= x + 25; i++)
            {
                for (int j = y - 50; j <= y + 5; j++)
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
            //place blocks below each structure incase of weird terrain
            for (int i = x - width + 5; i <= x + width - 5; i += 2)
            {
                for (int j = y; j <= (int)Main.worldSurface - 20; j += 2)
                {
                    if (Main.tile[i, j].TileType != ModContent.TileType<CemeteryStone>())
                    {
                        ShapeData circle = new ShapeData();
                        GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
                        WorldUtils.Gen(new Point(i, j), new Shapes.Circle(Main.rand.Next(1, 3)), Actions.Chain(new GenAction[]
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