using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Catacomb.Layer1;
using Spooky.Content.NPCs.Catacomb.Layer2;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.Quest;
using Spooky.Content.NPCs.SpiderCave;
using Spooky.Content.NPCs.SpookyBiome;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.NoseTemple;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Core
{
    //separate globalNPC for all of spooky mod's biome spawn pools so I can keep them more organized
    public class SpookyBiomeSpawns : GlobalNPC
    {
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			//increase spawns in the spider grotto because they are really low for some reason
			if (player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
            {
				spawnRate /= 3;
			}

			//increase spawns in the catacombs because they are too low by default
			if (player.InModBiome(ModContent.GetInstance<CatacombBiome>()))
            {
				spawnRate /= 2;
			}
			//increase spawns in the catacombs because they are too low by default
			if (player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
            {
				spawnRate /= 3;
			}

			//increase the spawn rate massively if you are in the catacombs before unlocking them, so that catacomb guardians spawn immediately
			if ((player.InModBiome(ModContent.GetInstance<CatacombBiome>()) && !Flags.CatacombKey1) || (player.InModBiome(ModContent.GetInstance<CatacombBiome2>()) && !Flags.CatacombKey2))
			{
				spawnRate /= 5;
			}

			//drastically increase spawns during the raveyard so all the funny skeletons spawn
			if (player.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
				spawnRate /= 5;
				maxSpawns *= 5;
			}

			//remove spawns if any spooky mod boss is alive (basically just a QoL change)
			if (NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) || NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()) ||
            NPC.AnyNPCs(ModContent.NPCType<Moco>()) || NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()) || NPC.AnyNPCs(ModContent.NPCType<BigBone>()) ||
            NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()) ||
			NPC.AnyNPCs(ModContent.NPCType<BanditBook>()) || NPC.AnyNPCs(ModContent.NPCType<EyeWizard>()) || NPC.AnyNPCs(ModContent.NPCType<FrankenGoblin>()) || NPC.AnyNPCs(ModContent.NPCType<StitchSpider>()))
            {
				spawnRate = 0;
				maxSpawns = 0;
			}

			//remove spawns during the pandora's box event
			if (player.InModBiome(ModContent.GetInstance<PandoraBoxBiome>()))
			{
				spawnRate = 0;
				maxSpawns = 0;
			}

			//remove spawns during the egg event
			if (player.InModBiome(ModContent.GetInstance<SpookyHellEventBiome>()))
            {
				spawnRate = 0;
				maxSpawns = 0;
			}

			//remove spawns while in the nose dungeon
			if (player.InModBiome(ModContent.GetInstance<NoseTempleBiome>()))
            {
				spawnRate = 0;
				maxSpawns = 0;
			}

			//disable spawns during a hallucination encounter
            if (player.HasBuff(ModContent.BuffType<HallucinationDebuff1>()) )
			{
				spawnRate = 0;
				maxSpawns = 0;
			}
		}

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
		{
			//bool to check if no events are happening
			bool NoEventsHappening = !spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
			!(spawnInfo.Player.ZoneTowerSolar || spawnInfo.Player.ZoneTowerVortex || spawnInfo.Player.ZoneTowerNebula || spawnInfo.Player.ZoneTowerStardust);

            //spooky forest surface spawns
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyBiome>()) && NoEventsHappening)
			{
				pool.Clear();

				//quest miniboss
				if (spawnInfo.Player.HasItem(ModContent.ItemType<SummonItem1>()) && !NPC.AnyNPCs(ModContent.NPCType<FrankenGoblin>()))
				{
					pool.Add(ModContent.NPCType<FrankenGoblin>(), 3);
				}

				//day time spawns
				if (Main.dayTime)
				{
					//critters
					pool.Add(ModContent.NPCType<FlySmall>(), 2);
					pool.Add(ModContent.NPCType<FlyBig>(), 2);

					//dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
						pool.Add(ModContent.NPCType<PuttySlime1>(), 3);
						pool.Add(ModContent.NPCType<PuttySlime2>(), 3);
						pool.Add(ModContent.NPCType<PuttySlime3>(), 3);
						pool.Add(ModContent.NPCType<HoppingCandyBasket>(), 0.2f);

						//hardmode enemies
                        if (Main.hardMode)
                        {
							pool.Add(ModContent.NPCType<PuttyPumpkin>(), 2);
                            pool.Add(ModContent.NPCType<ScarecrowShotgunner>(), 1);
                        }
                    }
				}
				//night time spawns
				else
				{
					//critters
					pool.Add(ModContent.NPCType<TinyGhost1>(), 1);
					pool.Add(ModContent.NPCType<TinyGhost2>(), 1);
					pool.Add(ModContent.NPCType<TinyGhostBoof>(), 0.5f);
					pool.Add(ModContent.NPCType<TinyGhostRare>(), 0.1f);

                    //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
						pool.Add(ModContent.NPCType<ZomboidThorn>(), 4);
						pool.Add(ModContent.NPCType<ZomboidPumpkin>(), 2);
						pool.Add(ModContent.NPCType<MonsterEye1>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye2>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye3>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye4>(), 1);
						pool.Add(ModContent.NPCType<FluffBatSmall1>(), 1);
                    	pool.Add(ModContent.NPCType<FluffBatSmall2>(), 1);
						pool.Add(ModContent.NPCType<HoppingCandyBasket>(), 0.2f);
						
						//windy day enemies
						if (Main.WindyEnoughForKiteDrops)
						{
							pool.Add(ModContent.NPCType<ZomboidWind>(), 3);
							pool.Add(ModContent.NPCType<ZomboidClarinet>(), 3);
						}

						//raining enemies
						if (Main.raining)
						{
							pool.Add(ModContent.NPCType<ZomboidRain>(), 3);
							pool.Add(ModContent.NPCType<ZomboidArmored>(), 0.8f);
						}

						//bloodmoon enemies
						if (Main.bloodMoon)
						{
							pool.Add(ModContent.NPCType<ZomboidTomato>(), 2);
							pool.Add(ModContent.NPCType<ZomboidTomatoMold>(), 2);
						}

						//do not spawn zomboid warlocks if one already exists
						if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidWarlock>()))
						{
							pool.Add(ModContent.NPCType<ZomboidWarlock>(), 1);
						}

						//hardmode enemies
						if (Main.hardMode)
						{
							pool.Add(ModContent.NPCType<Witch>(), 1);
							pool.Add(ModContent.NPCType<ZomboidPumpkinFire>(), 2);
						}
					}
				}
			}

			//spooky forest underground spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyBiomeUg>()))
			{
				pool.Clear();

				//critters
				pool.Add(ModContent.NPCType<LittleSpider>(), 3);
				pool.Add(ModContent.NPCType<TinyMushroom>(), 2);

                if (spawnInfo.SpawnTileType == ModContent.TileType<MushroomMoss>())
                {
                    pool.Add(ModContent.NPCType<ShroomHopper>(), 3);
                }

                //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
				if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
				{
					pool.Add(ModContent.NPCType<FluffBatSmall1>(), 2);
					pool.Add(ModContent.NPCType<FluffBatSmall2>(), 2);
                    pool.Add(ModContent.NPCType<FluffBatBig1>(), 1);
                    pool.Add(ModContent.NPCType<FluffBatBig2>(), 1);
                    pool.Add(ModContent.NPCType<ZomboidFungus>(), 4);

                    //do not spawn zomboid warlocks if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidWarlock>()))
					{
						pool.Add(ModContent.NPCType<ZomboidWarlock>(), 1);
					}

                    //mushroom moss mini-biome spawns
                    if (spawnInfo.SpawnTileType == ModContent.TileType<MushroomMoss>())
                    {
                        pool.Add(ModContent.NPCType<Bungus>(), 2);
                        pool.Add(ModContent.NPCType<Chungus>(), 2);
                    }

					//hardmode enemies
					if (Main.hardMode)
					{
						pool.Add(ModContent.NPCType<CandleMonster>(), 2);
					}
                }
			}

			//cemetery spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && !spawnInfo.Player.InModBiome(ModContent.GetInstance<RaveyardBiome>()) && NoEventsHappening)
			{
				pool.Clear();

				//quest miniboss
				if (spawnInfo.Player.HasItem(ModContent.ItemType<SummonItem2>()) && !NPC.AnyNPCs(ModContent.NPCType<BanditBook>()))
				{
					pool.Add(ModContent.NPCType<BanditBook>(), 3);
				}

				//critters
				//dont spawn rats during blood moons because of the feral rats
				if (!Main.bloodMoon)
				{
					pool.Add(ModContent.NPCType<TinyRat1>(), 1);
					pool.Add(ModContent.NPCType<TinyRat2>(), 1);
				}

                //dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
				if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
				{
					pool.Add(ModContent.NPCType<BloatGhostSmall>(), 4);
					pool.Add(ModContent.NPCType<SadGhostSmall>(), 3);
					pool.Add(ModContent.NPCType<SadGhostBig>(), 2);
					pool.Add(ModContent.NPCType<HungryGhost>(), 2);

					if (Main.dayTime)
					{
						pool.Add(ModContent.NPCType<SmileGhost>(), 2);
					}
					
					if (!Main.dayTime)
					{
						pool.Add(ModContent.NPCType<Possessor>(), 3);
						pool.Add(ModContent.NPCType<ZomboidGremlin>(), 4);
						
						if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidCasket>()))
						{
							pool.Add(ModContent.NPCType<ZomboidCasket>(), 2);
						}

						if (Main.bloodMoon)
						{	
							pool.Add(ModContent.NPCType<FeralRat1>(), 2);
							pool.Add(ModContent.NPCType<FeralRat2>(), 2);
						}
					}
                }
			}

			//raveyard skeleton spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<RaveyardBiome>()) && NoEventsHappening)
			{
				pool.Clear();

				if (!spawnInfo.Water)
				{
					pool.Add(ModContent.NPCType<PartySkeleton1>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton2>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton3>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton4>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton5>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton6>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton7>(), 5);
					pool.Add(ModContent.NPCType<PartySkeleton8>(), 5);
					pool.Add(ModContent.NPCType<SkeletonBouncer>(), 4);
					pool.Add(ModContent.NPCType<SpookyDance>(), 3);

					//do not spawn suspicious skeletons if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<SuspiciousSkeleton>()))
					{
						pool.Add(ModContent.NPCType<SuspiciousSkeleton>(), 5);
					}

					//do not spawn musicman if one exists, and if there isnt enough flat ground
					if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].HasTile &&
					Main.tile[spawnInfo.SpawnTileX + 1, spawnInfo.SpawnTileY].HasTile &&
					Main.tile[spawnInfo.SpawnTileX + 2, spawnInfo.SpawnTileY].HasTile &&
					Main.tile[spawnInfo.SpawnTileX + 3, spawnInfo.SpawnTileY].HasTile &&
					Main.tile[spawnInfo.SpawnTileX + 4, spawnInfo.SpawnTileY].HasTile)
					{
						if (!NPC.AnyNPCs(ModContent.NPCType<Musicman>()))
						{
							pool.Add(ModContent.NPCType<Musicman>(), 6);
						}
					}
				}
			}

			//catacomb first layer spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CatacombBiome>()))
			{
                pool.Clear();

				int[] CatacombLayer1Tiles = { ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombFlooring>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer1Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					if (Flags.CatacombKey1)
					{
						pool.Add(ModContent.NPCType<Skeletoid1>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid2>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid3>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid4>(), 5);
						pool.Add(ModContent.NPCType<SkeletoidBig>(), 3);
						pool.Add(ModContent.NPCType<RollingSkull1>(), 4);
						pool.Add(ModContent.NPCType<RollingSkull2>(), 4);
						pool.Add(ModContent.NPCType<RollingSkull3>(), 4);
						pool.Add(ModContent.NPCType<RollingSkull4>(), 1);
						pool.Add(ModContent.NPCType<GiantPutty>(), 5);
						pool.Add(ModContent.NPCType<BoneStackerBase>(), 4);
						pool.Add(ModContent.NPCType<ZomboidNecromancer>(), 3);
						pool.Add(ModContent.NPCType<ZomboidPyromancer>(), 3);
						//pool.Add(ModContent.NPCType<ZomboidGlyphomancer>(), 3);
						pool.Add(ModContent.NPCType<ZomboidSuspiciomancer>(), 2);
					}
					else
					{
						if (!NPC.AnyNPCs(ModContent.NPCType<CatacombGuardian>()))
						{
							pool.Add(ModContent.NPCType<CatacombGuardian>(), 2);
						}
					}
				}
			}

			//catacomb second layer spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
			{
                pool.Clear();

                int[] CatacombLayer2Tiles = { ModContent.TileType<CatacombBrick2>(), ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<GildedBrick>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer2Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					if (Flags.CatacombKey2)
					{
						pool.Add(ModContent.NPCType<CatacombCrusherSpawner>(), 12);
						pool.Add(ModContent.NPCType<CelebrationSkeletoid1>(), 5);
						pool.Add(ModContent.NPCType<CelebrationSkeletoid2>(), 5);
						pool.Add(ModContent.NPCType<CelebrationSkeletoid3>(), 5);
						pool.Add(ModContent.NPCType<CelebrationSkeletoid4>(), 5);
						pool.Add(ModContent.NPCType<CelebrationSkeletoid5>(), 5);
						pool.Add(ModContent.NPCType<FlushBush1>(), 2);
						pool.Add(ModContent.NPCType<FlushBush2>(), 2);
						pool.Add(ModContent.NPCType<FlushBush3>(), 2);
						pool.Add(ModContent.NPCType<FlushBush4>(), 2);
						pool.Add(ModContent.NPCType<JumpingSeed1>(), 4);
						pool.Add(ModContent.NPCType<JumpingSeed2>(), 4);
						pool.Add(ModContent.NPCType<JumpingSeed3>(), 4);
						pool.Add(ModContent.NPCType<LilySlime1Big>(), 3);
						pool.Add(ModContent.NPCType<LilySlime1Small>(), 3);
						pool.Add(ModContent.NPCType<LilySlime2Big>(), 3);
						pool.Add(ModContent.NPCType<LilySlime2Small>(), 3);
						pool.Add(ModContent.NPCType<OrchidStem>(), 12);
						pool.Add(ModContent.NPCType<PitcherPlant1>(), 3);
						pool.Add(ModContent.NPCType<PitcherPlant2>(), 3);
						pool.Add(ModContent.NPCType<PitcherPlant3>(), 3);
						pool.Add(ModContent.NPCType<PitcherPlant4>(), 3);
						pool.Add(ModContent.NPCType<PlantTrap1>(), 2);
						pool.Add(ModContent.NPCType<PlantTrap2>(), 2);
						pool.Add(ModContent.NPCType<PlantTrap3>(), 2);
						pool.Add(ModContent.NPCType<PlantTrap4>(), 2);
						pool.Add(ModContent.NPCType<PlantTrap5>(), 2);
						pool.Add(ModContent.NPCType<PlantTrap6>(), 2);
						pool.Add(ModContent.NPCType<PollinatorBeeDamage>(), 2);
						pool.Add(ModContent.NPCType<PollinatorBeeHealing>(), 2);
						pool.Add(ModContent.NPCType<SkeletoidBandit>(), 2);
						pool.Add(ModContent.NPCType<Sunflower1>(), 3);
						pool.Add(ModContent.NPCType<Sunflower2>(), 3);
						pool.Add(ModContent.NPCType<Sunflower3>(), 3);

						//do not spawn smelly if one already exists
						if (!NPC.AnyNPCs(ModContent.NPCType<Smelly>()))
						{
							pool.Add(ModContent.NPCType<Smelly>(), 1);
						}

						//do not spawn dahlia if one already exists
						if (!NPC.AnyNPCs(ModContent.NPCType<Dahlia>()))
						{
							pool.Add(ModContent.NPCType<Dahlia>(), 0.8f);
						}
					}
					else
					{
						if (!NPC.AnyNPCs(ModContent.NPCType<CatacombGuardian>()))
						{
							pool.Add(ModContent.NPCType<CatacombGuardian>(), 2);
						}
					}
				}
			}

			//spider grotto spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
			{
				pool.Clear();

				int[] InvalidTiles = { ModContent.TileType<SpookyStoneBricks>() };

				//do not allow spider grotto enemies to spawn inside of the structures in the biome
				if (!InvalidTiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					//quest miniboss
					if (spawnInfo.Player.HasItem(ModContent.ItemType<SummonItem3>()) && !NPC.AnyNPCs(ModContent.NPCType<StitchSpider>()))
					{
						pool.Add(ModContent.NPCType<StitchSpider>(), 3);
					}

					//critters
					pool.Add(ModContent.NPCType<Ant1>(), 1);
					pool.Add(ModContent.NPCType<Ant2>(), 1);
					pool.Add(ModContent.NPCType<Cockroach>(), 1);
					pool.Add(ModContent.NPCType<Inchworm1>(), 1);
					pool.Add(ModContent.NPCType<Inchworm2>(), 1);
					pool.Add(ModContent.NPCType<Inchworm3>(), 1);
					pool.Add(ModContent.NPCType<Mosquito1>(), 1);
					pool.Add(ModContent.NPCType<Mosquito2>(), 1);
					pool.Add(ModContent.NPCType<Mosquito3>(), 1);
					pool.Add(ModContent.NPCType<Moth1>(), 1);
					pool.Add(ModContent.NPCType<Moth2>(), 1);

					//dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
						pool.Add(ModContent.NPCType<DaddyLongLegs>(), 3);
						pool.Add(ModContent.NPCType<JumpingSpider1>(), 3);
						pool.Add(ModContent.NPCType<JumpingSpider2>(), 3);
						pool.Add(ModContent.NPCType<BallSpiderWeb>(), 3);
						pool.Add(ModContent.NPCType<LeafSpiderSleeping>(), 2);
						pool.Add(ModContent.NPCType<OrbWeaver1>(), 2);
						pool.Add(ModContent.NPCType<OrbWeaver2>(), 2);
						pool.Add(ModContent.NPCType<OrbWeaver3>(), 2);
						pool.Add(ModContent.NPCType<TinySpiderEgg>(), 3);

						if (Main.hardMode)
						{
							pool.Add(ModContent.NPCType<OrbWeaverGiant>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk1>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk2>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk3>(), 1);
							pool.Add(ModContent.NPCType<TrapdoorSpiderIdle1>(), 2);
							pool.Add(ModContent.NPCType<TrapdoorSpiderIdle2>(), 1);
							pool.Add(ModContent.NPCType<WhipSpider>(), 2);
						}
					}
				}
			}

			//eye valley spawns
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{ 
                pool.Clear();

				int[] NoseTempleTiles = { ModContent.TileType<NoseTempleBrickGray>(), ModContent.TileType<NoseTempleBrickGreen>(), ModContent.TileType<NoseTempleBrickPurple>(),
				ModContent.TileType<NoseTempleFancyBrickGray>(), ModContent.TileType<NoseTempleFancyBrickGreen>(), ModContent.TileType<NoseTempleFancyBrickPurple>(),
				ModContent.TileType<NoseTemplePlatformGray>(), ModContent.TileType<NoseTemplePlatformGreen>(), ModContent.TileType<NoseTemplePlatformPurple>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (!NoseTempleTiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					//quest miniboss
					if (spawnInfo.Player.HasItem(ModContent.ItemType<SummonItem4>()) && !NPC.AnyNPCs(ModContent.NPCType<EyeWizard>()))
					{
						pool.Add(ModContent.NPCType<EyeWizard>(), 3);
					}

					pool.Add(ModContent.NPCType<EyeBat>(), 3);
					pool.Add(ModContent.NPCType<EyeBatFleshy>(), 0.8f);
					pool.Add(ModContent.NPCType<MrHandy>(), 3);
					pool.Add(ModContent.NPCType<MrHandyFleshy>(), 0.8f);
					pool.Add(ModContent.NPCType<ManHole>(), 3);
					pool.Add(ModContent.NPCType<ManHoleFleshy>(), 0.8f);
					pool.Add(ModContent.NPCType<Tortumor>(), 3);
					pool.Add(ModContent.NPCType<TortumorFleshy>(), 0.8f);

					//do not spawn mocling swarms if one exists
					if (!NPC.AnyNPCs(ModContent.NPCType<MoclingSwarm>()))
					{
						pool.Add(ModContent.NPCType<MoclingSwarm>(), 1);
					}

					//do not spawn giant tortumors if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<TortumorGiant>()))
					{
						pool.Add(ModContent.NPCType<TortumorGiant>(), 0.7f);
					}

					//do not spawn giant fleshy tortumors if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<TortumorGiantFleshy>()))
					{
						pool.Add(ModContent.NPCType<TortumorGiantFleshy>(), 0.55f);
					}
				}
            }

			//dumb zomboid can spawn anywhere super rarely
			if (!NPC.AnyNPCs(ModContent.NPCType<DumbZomboid>()) && !spawnInfo.Water)
			{
				//pool.Add(ModContent.NPCType<DumbZomboid>(), 0.01f);
			}
        }
    }
}