using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Generation
{
    public class MiscGeneration : ModSystem
    {
        private void PlaceSpookyChest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.DungeonChests").Value;

            PlaceDungeonChest(progress, ModContent.TileType<SpookyBiomeChest>());
			progress.Set(0.25);
            PlaceDungeonChest(progress, ModContent.TileType<CemeteryBiomeChest>());
			progress.Set(0.5);
			PlaceDungeonChest(progress, ModContent.TileType<SpiderCaveChest>());
			progress.Set(0.75);
			PlaceDungeonChest(progress, ModContent.TileType<SpookyHellChest>());
			progress.Set(1);
		}

        public static void PlaceDungeonChest(GenerationProgress progress, int ChestType)
        {   
            bool placedChest = false;

            bool RightSideDungeon = GenVars.dungeonSide == 1;

            int Start = RightSideDungeon ? Main.maxTilesX - 100 : 100;
            int End = RightSideDungeon ? (Main.maxTilesX - 100) - (Main.maxTilesX / 3) : 100 + (Main.maxTilesX / 3);

            int Increment = RightSideDungeon ? -1 : 1;

            for (int i = Start; RightSideDungeon ? i >= End : i <= End; i += Increment)
			{
				for (int j = (int)Main.worldSurface + 300; j <= Main.maxTilesY - 200; j++)
                {
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
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spooky Biome Dungeon Chests", PlaceSpookyChest));
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