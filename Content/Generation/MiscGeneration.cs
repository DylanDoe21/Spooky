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
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class MiscGeneration : ModSystem
    {
        private void PlaceSpookyChest(GenerationProgress progress, GameConfiguration configuration)
        {
            PlaceDungeonChest(ModContent.TileType<SpookyBiomeChest>());
            PlaceDungeonChest(ModContent.TileType<SpookyHellChest>());
            PlaceDungeonChest(ModContent.TileType<CemeteryBiomeChest>());
            PlaceDungeonChest(ModContent.TileType<SpiderCaveChest>());
        }

        public static void PlaceDungeonChest(int ChestType)
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
            //check a 300 by 300 square to make sure the spooky mod dungeon chests are spaced out enough
            for (int i = X - 150; i < X + 150; i++)
            {
                for (int j = Y - 150; j < Y + 150; j++)
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

        private void PlaceSecretPetShrine(GenerationProgress progress, GameConfiguration configuration)
        {
            Vector2 structureOrigin = new Vector2(Main.maxTilesX / 2 + WorldGen.genRand.Next(-200, 200), Main.maxTilesY / 2 + 20);
            Generator.GenerateStructure("Content/Structures/SecretPetShrine", structureOrigin.ToPoint16(), Mod);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spooky Biome Dungeon Chests", PlaceSpookyChest));

            if (MenuSaveSystem.hasDefeatedRotGourd && MenuSaveSystem.hasDefeatedSpookySpirit && MenuSaveSystem.hasDefeatedMoco &&
            MenuSaveSystem.hasDefeatedDaffodil && MenuSaveSystem.hasDefeatedOrroboro && MenuSaveSystem.hasDefeatedBigBone)
            {
                tasks.Insert(GenIndex1 + 3, new PassLegacy("Secret Pet Shrine", PlaceSecretPetShrine));
            }
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