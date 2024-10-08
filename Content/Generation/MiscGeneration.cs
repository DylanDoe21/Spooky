using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.IO;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class MiscGeneration : ModSystem
    {
        private void PlaceSpookyChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.DungeonChests").Value;

            PlaceDungeonChest(ModContent.TileType<SpookyBiomeChest>());
            PlaceDungeonChest(ModContent.TileType<CemeteryBiomeChest>());
            PlaceDungeonChest(ModContent.TileType<SpiderCaveChest>());
            PlaceDungeonChest(ModContent.TileType<SpookyHellChest>());
        }

        private void PlaceOresInMossyStone(GenerationProgress progress, GameConfiguration configuration)
        {
            //place clumps of vanilla ores
            
            //determine which ores to place based on the opposite of what ores generate
            ushort OppositeTier1Ore = WorldGen.SavedOreTiers.Copper == TileID.Copper ? TileID.Tin : TileID.Copper;
            ushort OppositeTier2Ore = WorldGen.SavedOreTiers.Iron == TileID.Iron ? TileID.Lead : TileID.Iron;
            ushort OppositeTier3Ore = WorldGen.SavedOreTiers.Silver == TileID.Silver ? TileID.Tungsten : TileID.Silver;
            ushort OppositeTier4Ore = WorldGen.SavedOreTiers.Gold == TileID.Gold ? TileID.Platinum : TileID.Gold;

            for (int copper = 0; copper < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); copper++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(8, 12), WorldGen.genRand.Next(8, 12), OppositeTier1Ore, false, 0f, 0f, false, true);
                }
            }

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 4E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(6, 10), WorldGen.genRand.Next(6, 10), OppositeTier2Ore, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 3E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), OppositeTier3Ore, false, 0f, 0f, false, true);
                }
            }

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 2E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 6), OppositeTier4Ore, false, 0f, 0f, false, true);
                }
            }
        }

        public static void PlaceDungeonChest(int ChestType)
        {   
            bool placedChest = false;
            
            for (int j = (int)Main.worldSurface + 300; j <= Main.maxTilesY - 200; j++)
            {
                int i = 100;
                if (GenVars.dungeonSide == 1)
                {
                    i = Main.maxTilesX - 100;
                }

                bool shouldContinue = true;

                while (shouldContinue)
                {
                    if (GenVars.dungeonSide == 1)
                    {
                        i--;
                        if (i < Main.maxTilesX / 2)
                        {
                            shouldContinue = false;
                        }
                    }
                    else
                    {
                        i++;
                        if (i > Main.maxTilesX / 2)
                        {
                            shouldContinue = false;
                        }
                    }

                    Tile tile = Main.tile[i, j];
                    Tile tileUp = Main.tile[i, j - 1];
                    Tile tileRight = Main.tile[i + 1, j - 1];

                    if (Main.tileDungeon[tile.TileType] && Main.wallDungeon[tileUp.WallType] && !tileUp.HasTile && !tileRight.HasTile && !placedChest && CanPlaceDungeonChest(i, j))
                    {
                        WorldGen.PlaceChest(i, j - 1, (ushort)ChestType, true, 1);
                    }

                    if (tileUp.TileType == ChestType)
                    {
                        placedChest = true;
                    }
                }
            }
        }

        public static bool CanPlaceDungeonChest(int X, int Y)
        {
            //check a 150 by 150 square to make sure the spooky mod dungeon chests are spaced out enough
            for (int i = X - 75; i < X + 75; i++)
            {
                for (int j = Y - 75; j < Y + 75; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<SpookyBiomeChest>() || Main.tile[i, j].TileType == ModContent.TileType<SpookyHellChest>() ||
                    Main.tile[i, j].TileType == ModContent.TileType<CemeteryBiomeChest>() || Main.tile[i, j].TileType == ModContent.TileType<SpiderCaveChest>()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Spreading Grass"));
            if (GenIndex1 == -1) 
            {
                return;
            }

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spooky Biome Ores", PlaceOresInMossyStone));

            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex2 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spooky Biome Dungeon Chests", PlaceSpookyChest));
        }

        //place items in chests
        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
				Chest chest = Main.chest[chestIndex];

                if (chest == null) 
                {
					continue;
				}

				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

					//spooky biome chest items
					if (chest != null && (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>() || chestTile.TileType == ModContent.TileType<SpookyHellChest>() ||
					chestTile.TileType == ModContent.TileType<CemeteryBiomeChest>() || chestTile.TileType == ModContent.TileType<SpiderCaveChest>()))
					{
						//potions
						int[] Potions1 = new int[] { ItemID.AmmoReservationPotion, ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

						//more potions
						int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.InfernoPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

						//cemetery biome chest main item
						if (chestTile.TileType == ModContent.TileType<CemeteryBiomeChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<DiscoSkull>());
							chest.item[0].stack = 1;
						}

						//spider cave biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpiderCaveChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<VenomHarpoon>());
							chest.item[0].stack = 1;
						}

						//spooky forest biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<ElGourdo>());
							chest.item[0].stack = 1;
						}

						//eye valley biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpookyHellChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<BrainJar>());
							chest.item[0].stack = 1;
						}

						//potions
						chest.item[1].SetDefaults(WorldGen.genRand.Next(Potions1));
						chest.item[1].stack = WorldGen.genRand.Next(1, 3);
						//even more potions
						chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions2));
						chest.item[2].stack = WorldGen.genRand.Next(1, 3);
						//recovery potions
						chest.item[3].SetDefaults(ItemID.GreaterHealingPotion);
						chest.item[3].stack = WorldGen.genRand.Next(5, 11);
						//gold coins
						chest.item[4].SetDefaults(ItemID.GoldCoin);
						chest.item[4].stack = WorldGen.genRand.Next(5, 16);
					}
				}
            }
        }
    }
}