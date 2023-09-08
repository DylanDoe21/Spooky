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

using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Generation
{
    public class SpookyDungeonChests : ModSystem
    {
        private void PlaceSpookyChest(GenerationProgress progress, GameConfiguration configuration)
        {
            bool placedChest = false;

            for (int j = (int)Main.worldSurface + 200; j <= Main.maxTilesY - 200; j++)
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

                    if (Main.tileDungeon[tile.TileType] && Main.wallDungeon[tileUp.WallType] && !tileUp.HasTile && !tileRight.HasTile && !placedChest)
                    {
                        WorldGen.PlaceChest(i, j - 1, (ushort)ModContent.TileType<SpookyBiomeChest>(), true, 1);
                    }

                    if (tileUp.TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        placedChest = true;
                    }
                }
            }
        }

        private void PlaceEyeChest(GenerationProgress progress, GameConfiguration configuration)
        {
            bool placedChest = false;
            
            for (int j = (int)Main.worldSurface + 200; j <= Main.maxTilesY - 200; j++)
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

                    if (Main.tileDungeon[tile.TileType] && Main.wallDungeon[tileUp.WallType] && !tileUp.HasTile && !tileRight.HasTile && !placedChest)
                    {
                        WorldGen.PlaceChest(i, j - 1, (ushort)ModContent.TileType<SpookyHellChest>(), true, 1);
                    }

                    if (tileUp.TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        placedChest = true;
                    }
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("PlaceSpookyChest", PlaceSpookyChest));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("PlaceEyeChest", PlaceEyeChest));
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

                Tile chestTile = Main.tile[chest.x, chest.y];

                //spooky biome chest items
				if (chest != null && (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>() || 
                chestTile.TileType == ModContent.TileType<SpookyHellChest>()))
                {
                    //potions
                    int[] Potions1 = new int[] { ItemID.AmmoReservationPotion, ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

                    //more potions
                    int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.InfernoPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

                    //spooky biome chest main item
                    if (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        chest.item[0].SetDefaults(ModContent.ItemType<ElGourdo>());
                        chest.item[0].stack = 1;
                    }

                    //eye biome chest main item
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