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
        private void PlaceDungeonChests(GenerationProgress progress, GameConfiguration configuration)
        {
            bool placedChest1 = false;
            bool placedChest2 = false;

            //spooky forest chest
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

                    if (Main.tileDungeon[tile.TileType] && Main.wallDungeon[tileUp.WallType] && !tileUp.HasTile && !tileRight.HasTile && !placedChest1)
                    {
                        WorldGen.PlaceChest(i, j - 1, (ushort)ModContent.TileType<SpookyBiomeChest>(), true, 1);
                    }

                    if (tileUp.TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        placedChest1 = true;
                    }
                }
            }

            //eye valley chest
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

                    if (Main.tileDungeon[tile.TileType] && Main.wallDungeon[tileUp.WallType] && !tileUp.HasTile && !tileRight.HasTile && !placedChest2)
                    {
                        WorldGen.PlaceChest(i, j - 1, (ushort)ModContent.TileType<SpookyHellChest>(), true, 1);
                    }

                    if (tileUp.TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        placedChest2 = true;
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

            tasks.Insert(GenIndex1 + 1, new PassLegacy("PlaceDungeonChests", PlaceDungeonChests));
        }

        //place items in chests
        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex]; 

                //spooky biome chest items
				if (chest != null && (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyBiomeChest>() || 
                Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyHellChest>()))
                {
                    int[] Potions = new int[] { ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };

                    //spooky biome chest main item
                    if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        chest.item[0].SetDefaults(ModContent.ItemType<ElGourdo>());
                        chest.item[0].stack = 1;
                    }

                    //eye biome chest main item
                    if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        chest.item[0].SetDefaults(ModContent.ItemType<BrainJar>());
                        chest.item[0].stack = 1;
                    }

                    //candles
                    chest.item[1].SetDefaults(ModContent.ItemType<CandleItem>());
                    chest.item[1].stack = WorldGen.genRand.Next(5, 12);
                    //potions
                    chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions));
                    chest.item[2].stack = WorldGen.genRand.Next(5, 8);
                    //healing potions
                    chest.item[3].SetDefaults(ItemID.GreaterHealingPotion);
                    chest.item[3].stack = WorldGen.genRand.Next(12, 20);
                    //mana potions
                    chest.item[4].SetDefaults(ItemID.GreaterManaPotion);
                    chest.item[4].stack = WorldGen.genRand.Next(12, 20);
                    //gold coins
                    chest.item[5].SetDefaults(ItemID.GoldCoin);
                    chest.item[5].stack = WorldGen.genRand.Next(10, 15);
                }
            }
        }
    }
}