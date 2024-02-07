using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.RotGourd;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Catacomb;
using Spooky.Content.NPCs.Catacomb.Layer1;
using Spooky.Content.NPCs.Catacomb.Layer2;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.SpiderCave;
using Spooky.Content.NPCs.SpookyBiome;
using Spooky.Content.NPCs.SpookyHell;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Core
{
    //separate globalNPC for all of spooky mod's biome spawn pools so I can keep them more organized
    public class SpookyBiomeSpawns : GlobalNPC
    {
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			//remove spawns if any spooky mod boss is alive (basically just a QoL change)
			if (NPC.AnyNPCs(ModContent.NPCType<RotGourd>()) || NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()) ||
            NPC.AnyNPCs(ModContent.NPCType<Moco>()) || NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()) || NPC.AnyNPCs(ModContent.NPCType<BigBone>()) ||
            NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
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

			//disable spawns during a hallucination encounter
            if (player.HasBuff(ModContent.BuffType<HallucinationDebuff1>()) || player.HasBuff(ModContent.BuffType<HallucinationDebuff2>()) ||
			player.HasBuff(ModContent.BuffType<HallucinationDebuff3>()) || player.HasBuff(ModContent.BuffType<HallucinationDebuff4>()))
			{
				spawnRate = 0;
				maxSpawns = 0;
			}

			//increase the spawn rate massively if you are in the catacombs before unlocking them, so that catacomb guardians spawn immediately
			if ((player.InModBiome(ModContent.GetInstance<CatacombBiome>()) && !Flags.CatacombKey1) ||
			(player.InModBiome(ModContent.GetInstance<CatacombBiome2>()) && !Flags.CatacombKey2))
			{
				spawnRate /= 2;
			}

			//drastically increase spawns during the raveyard so all the funny skeletons spawn
			if (player.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
				spawnRate = (int)(spawnRate / 5);
				maxSpawns = (int)(maxSpawns * 5);
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

				//day time spawns
				if (Main.dayTime)
				{
					//critters
					pool.Add(ModContent.NPCType<FlySmall>(), 2);
					pool.Add(ModContent.NPCType<FlyBig>(), 2);

					//dont spawn enemies in a town, but also allow enemy spawns in a town with the shadow candle
					if (!spawnInfo.PlayerInTown || (spawnInfo.PlayerInTown && spawnInfo.Player.ZoneShadowCandle))
					{
						pool.Add(ModContent.NPCType<PuttySlime1>(), 6);
						pool.Add(ModContent.NPCType<PuttySlime2>(), 6);
						pool.Add(ModContent.NPCType<PuttySlime3>(), 6);
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
						pool.Add(ModContent.NPCType<ZomboidPumpkin>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye1>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye2>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye3>(), 1);
						pool.Add(ModContent.NPCType<MonsterEye4>(), 1);
						pool.Add(ModContent.NPCType<FluffBatSmall1>(), 1);
                    	pool.Add(ModContent.NPCType<FluffBatSmall2>(), 1);
						
						//spawn windchime zomboids during windy days
						if (Main.WindyEnoughForKiteDrops)
						{
							pool.Add(ModContent.NPCType<ZomboidWind>(), 3);
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
							pool.Add(ModContent.NPCType<ZomboidPumpkinFire>(), 1);
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
					pool.Add(ModContent.NPCType<FluffBatSmall1>(), 4);
					pool.Add(ModContent.NPCType<FluffBatSmall2>(), 4);
                    pool.Add(ModContent.NPCType<FluffBatBig1>(), 2);
                    pool.Add(ModContent.NPCType<FluffBatBig2>(), 2);
                    pool.Add(ModContent.NPCType<ZomboidFungus>(), 5);

                    //do not spawn zomboid warlocks if one already exists
					if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidWarlock>()))
					{
						pool.Add(ModContent.NPCType<ZomboidWarlock>(), 1);
					}

                    //mushroom moss mini-biome spawns
                    if (spawnInfo.SpawnTileType == ModContent.TileType<MushroomMoss>())
                    {
                        pool.Add(ModContent.NPCType<Bungus>(), 3);
                        pool.Add(ModContent.NPCType<Chungus>(), 2);
                    }
                }
			}

			//cemetery spawns
			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && NoEventsHappening)
			{
				pool.Clear();

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
					pool.Add(ModContent.NPCType<ZomboidGremlin>(), 4);
					pool.Add(ModContent.NPCType<BloatGhostSmall>(), 3);
					pool.Add(ModContent.NPCType<SadGhostSmall>(), 2);
					pool.Add(ModContent.NPCType<SadGhostBig>(), 1);

					if (!NPC.AnyNPCs(ModContent.NPCType<ZomboidCasket>()))
					{
						pool.Add(ModContent.NPCType<ZomboidCasket>(), 2);
					}

					if (!Main.dayTime)
					{
						pool.Add(ModContent.NPCType<Possessor>(), 3);

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

				int[] CatacombLayer1Tiles = { ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombFlooring>(), ModContent.TileType<CemeteryGrass>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer1Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					if (Flags.CatacombKey1)
					{
						//critters
						pool.Add(NPCID.Maggot, 2);
						pool.Add(ModContent.NPCType<FlySmall>(), 2);
						pool.Add(ModContent.NPCType<FlyBig>(), 2);

						//enemies
						pool.Add(ModContent.NPCType<Skeletoid1>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid2>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid3>(), 5);
						pool.Add(ModContent.NPCType<Skeletoid4>(), 5);
						pool.Add(ModContent.NPCType<SkeletoidBig>(), 3);
						pool.Add(ModContent.NPCType<RollingSkull1>(), 5);
						pool.Add(ModContent.NPCType<RollingSkull2>(), 5);
						pool.Add(ModContent.NPCType<RollingSkull3>(), 5);
						pool.Add(ModContent.NPCType<RollingSkull4>(), 1);
						pool.Add(ModContent.NPCType<GiantPutty>(), 5);
						pool.Add(ModContent.NPCType<BoneStackerBase>(), 4);
						pool.Add(ModContent.NPCType<ZomboidNecromancer>(), 3);
						pool.Add(ModContent.NPCType<ZomboidPyromancer>(), 3);
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

                int[] CatacombLayer2Tiles = { ModContent.TileType<CatacombBrick2>(), ModContent.TileType<GildedBrick>(), ModContent.TileType<CemeteryGrass>() };

				//do not allow catacomb enemies to spawn on non catacomb tiles
				if (CatacombLayer2Tiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
					if (Flags.CatacombKey2)
					{
						//critters
						pool.Add(NPCID.Maggot, 2);
						pool.Add(ModContent.NPCType<FlySmall>(), 2);
						pool.Add(ModContent.NPCType<FlyBig>(), 2);

						//enemies
						pool.Add(ModContent.NPCType<Daisy1>(), 3);
						pool.Add(ModContent.NPCType<Daisy2>(), 3);
						pool.Add(ModContent.NPCType<Flourence>(), 2);
						pool.Add(ModContent.NPCType<Marigold>(), 3);
						pool.Add(ModContent.NPCType<MarigoldSpit>(), 2);
						pool.Add(ModContent.NPCType<Smelly>(), 2);
						pool.Add(ModContent.NPCType<Toothy>(), 2);

						//do not spawn sunny if one already exists
						if (!NPC.AnyNPCs(ModContent.NPCType<Sunny>()))
						{
							pool.Add(ModContent.NPCType<Sunny>(), 1);
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

			//eye valley spawns
            if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{ 
                pool.Clear();

				pool.Add(ModContent.NPCType<EyeBat>(), 5);
				pool.Add(ModContent.NPCType<MrHandy>(), 5);
				pool.Add(ModContent.NPCType<ManHole>(), 4);
				pool.Add(ModContent.NPCType<ManHoleBig>(), 4);
				pool.Add(ModContent.NPCType<Tortumor>(), 3);

				//do not spawn giant tortumors if one already exists
				if (!NPC.AnyNPCs(ModContent.NPCType<TortumorGiant>()))
                {
                    pool.Add(ModContent.NPCType<TortumorGiant>(), Main.hardMode ? 1 : 0.5f);
                }
            }

			if (spawnInfo.Player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
			{
				pool.Clear();

				int[] SpiderGrottoTiles = { ModContent.TileType<DampGrass>(), ModContent.TileType<DampSoil>(), ModContent.TileType<SpookyStone>() };

				//do not allow spider grotto enemies to spawn on non spider grotto tiles
				if (SpiderGrottoTiles.Contains(Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY].TileType))
				{
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

						if (Main.hardMode)
						{
							pool.Add(ModContent.NPCType<OrbWeaverGiant>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk1>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk2>(), 1);
							pool.Add(ModContent.NPCType<TarantulaHawk3>(), 1);
							pool.Add(ModContent.NPCType<TrapdoorSpiderIdle1>(), 2);
							pool.Add(ModContent.NPCType<TrapdoorSpiderIdle2>(), 1);
							pool.Add(ModContent.NPCType<WhipSpider>(), 1);
						}
					}
				}
			}

			//dumb zomboid can spawn anywhere super rarely
			if (!NPC.AnyNPCs(ModContent.NPCType<DumbZomboid>()))
			{
				pool.Add(ModContent.NPCType<DumbZomboid>(), 0.0005f);
			}
        }
    }
}