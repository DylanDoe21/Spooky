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
        private void PlaceCemetery(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Cemetery").Value;

            BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

            Catacombs.PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            float worldEdgeOffset = Main.maxTilesX >= 6400 ? 8.65f : 8.55f;

            //place biome based on the opposite side of the dungeon
            if (GenVars.dungeonSide == -1)
			{
                Catacombs.PositionX = Main.maxTilesX - (Main.maxTilesX / (int)worldEdgeOffset) - 120;
			}
			else
			{
                Catacombs.PositionX = (Main.maxTilesX / (int)worldEdgeOffset) - 180;
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
                for (int Y = Catacombs.PositionY - 100; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileUp = Main.tile[X, Y - 1];
                    Tile tileDown = Main.tile[X, Y + 1];
                    Tile tileLeft = Main.tile[X - 1, Y];
                    Tile tileRight = Main.tile[X + 1, Y];

                    //place cemetery dirt blocks on crimstone and ebonstone walls because they are annoying
                    if (!tile.HasTile && (tile.WallType == WallID.EbonstoneUnsafe || tile.WallType == WallID.CrimstoneUnsafe))
                    {
                        WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<CemeteryDirt>());
                    }

                    //convert all tiles into cemetery dirt
                    if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                    {
                        tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                    }

                    //reaplce walls with cemetery grass walls
                    if (tile.WallType > 0)
                    {
                        tile.WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
                    }
                }

                //place block clusters right above the world surface to prevent the cemetery from generating too low
                for (int FillY = (int)Main.worldSurface - 50; FillY <= Main.worldSurface; FillY++)
                {
                    SpookyWorldMethods.PlaceCircle(X, FillY, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //add tile dithering on the edges of the biome
            for (int X = XMiddle - (BiomeWidth / 2) - 20; X <= XMiddle + (BiomeWidth / 2) + 20; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        Tile tile = Main.tile[X, Y];
                        Tile tileUp = Main.tile[X, Y - 1];
                        Tile tileDown = Main.tile[X, Y + 1];
                        Tile tileLeft = Main.tile[X - 1, Y];
                        Tile tileRight = Main.tile[X + 1, Y];

                        //place dirt blocks
                        if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                        {
                            tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                        }

                        //reaplce walls with cemetery grass walls
                        if (tile.WallType > 0)
                        {
                            tile.WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
                        }
                    }
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
            int XEdge = XStart + BiomeWidth;

            int StartPosY = Catacombs.PositionY - 100;

            //graveyards
            GenerateStructure((XStart + XMiddle) / 2 - 95, StartPosY, "Graveyard-1", 12, 8);
            GenerateStructure((XStart + XMiddle) / 2 - 72, StartPosY, "Graveyard-2", 12, 8);
            GenerateStructure((XStart + XMiddle) / 2 - 35, StartPosY, "Graveyard-3", 12, 8);
            GenerateStructure((XStart + XMiddle) / 2 + 35, StartPosY, "Graveyard-4", 12, 8);

            //first ruined house
            GenerateStructure((XStart + XMiddle) / 2, StartPosY, "RuinedHouse-1", 14, 20);

            //catacomb entrance
            GenerateStructure(XMiddle, StartPosY, "CemeteryEntrance", 38, 25);

            //second ruined house
            GenerateStructure((XMiddle + XEdge) / 2, StartPosY, "RuinedHouse-2", 14, 20);

            //graveyards
            GenerateStructure((XMiddle + XEdge) / 2 - 35, StartPosY, "Graveyard-5", 14, 8);
            GenerateStructure((XMiddle + XEdge) / 2 + 35, StartPosY, "Graveyard-6", 12, 8);
            GenerateStructure((XMiddle + XEdge) / 2 + 72, StartPosY, "Graveyard-3", 12, 8);
            GenerateStructure((XMiddle + XEdge) / 2 + 95, StartPosY, "Graveyard-2", 12, 8);
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(startX, startY) && startY <= Main.worldSurface)
				{
					startY++;
				}
                if (!Main.tile[startX, startY].HasTile)
                {
                    //do not allow any structure to be placed on floating islands
                    CheckForFloatingIslands(startX, startY);

					continue;
                }

                Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                Generator.GenerateStructure("Content/Structures/Cemetery/" + StructureFile, origin.ToPoint16(), Mod);

                //when the cemetery catacomb crypt is placed, save the position for the catacomb entrance
                if (StructureFile == "CemeteryEntrance")
                {
                    Catacombs.EntranceY = startY - 37;
                }

                placed = true;
            }
        }

        //check the area around the given position for cloud blocks, to prevent structures from placing on floating islands
        public static bool CheckForFloatingIslands(int X, int Y)
        {
            int canPlace = 0;

            for (int i = X - 20; i < X + 20; i++)
            {
                for (int j = Y - 20; j < Y + 20; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud))
                    {
                        canPlace++;
                        if (canPlace > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Rock Wall Runner"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("CemeteryTerrain", PlaceCemetery));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("CemeteryStructures", GenerateCemeteryStructures));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("CemeteryGrass", SpreadCemeteryGrass));
            tasks.Insert(GenIndex1 + 4, new PassLegacy("CemeteryTrees", GrowCemeteryTrees));
        }
    }
}