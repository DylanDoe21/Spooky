using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Minibiomes.Ocean;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	//TODO: 
	//implement behavior for big dunk to roar and stop chasing the player when the player cant be pathfinded to instead of going to the closest node
	//implement behavior for big dunk getting distracted with bait, should make him pathfind to an existing bait if hes close enough to it
	//big dunk could pathfind to the player if they try and dig through the biome, or at least go to the closest node? or potentially come up with another way to discourage breaking blocks
	public class Dunkleosteus : ModNPC
	{
		private readonly PathFinding pathfinder = new PathFinding(20);

		int syncTimer = 0;
		int BodyFrame = 0;
		int BodyFrameCounter = 0;
		int Aggression = 0;

		bool BiteAnimation = false;
		int BiteAnimationTimer = 0;

		Vector2 PositionGoTo = Vector2.Zero;
	 	List<int> BiomePositionDistances = new List<int>();

		Player TargetedPlayer = Main.LocalPlayer;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> BodyTexture;
		private static Asset<Texture2D> BodyTexture2;

		public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusGrowl1", SoundType.Sound);
		public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusGrowl2", SoundType.Sound);
		public static readonly SoundStyle RoarSound = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusRoar", SoundType.Sound) { PitchVariance = 0.6f };
		public static readonly SoundStyle StingSound = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusSting", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 14;
			NPCID.Sets.TrailCacheLength[NPC.type] = 12;
			NPCID.Sets.TrailingMode[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/DunkleosteusBestiary",
                Position = new Vector2(-110f, 15f),
                PortraitPositionXOverride = -70f,
                PortraitPositionYOverride = 10f
            };

			NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 10000;
			NPC.damage = 100;
			NPC.defense = 9999;
			NPC.width = 90;
			NPC.height = 90;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.waterMovementSpeed = 1f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.behindTiles = true;
			NPC.immortal = true;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = 10000;
			NPC.damage = 100;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Dunkleosteus"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusGlow");
			BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusBody");
			BodyTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusBodyExtra");

			var effects = NPC.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

			//draw body
			Vector2 pos = new Vector2(NPC.velocity.X > 0f ? 12 : -12, 7).RotatedBy(NPC.rotation + MathHelper.PiOver2) + NPC.Center;
			Vector2 drawOrigin = new Vector2(BodyTexture.Width() * 0.5f, (BodyTexture.Height() / 8) * 0.5f);
			spriteBatch.Draw(BodyTexture.Value, pos - screenPos, new Rectangle(0, 122 * BodyFrame, 470, 122), drawColor, NPC.oldRot[NPC.oldPos.Length - 1], drawOrigin, NPC.scale, effects, 0);

			if (NPC.ai[1] <= 0 && Aggression <= 0 && !BiteAnimation)
			{
				spriteBatch.Draw(BodyTexture2.Value, pos - screenPos, new Rectangle(0, 122 * BodyFrame, 470, 122), drawColor, NPC.oldRot[NPC.oldPos.Length - 1], drawOrigin, NPC.scale, effects, 0);
			}

			//draw head
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

			if (!BiteAnimation)
			{
				if (NPC.ai[2] <= 0 && Aggression <= 0)
				{
					if (NPC.frameCounter > 10)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 8)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}
				else
				{
					if (NPC.frame.Y < frameHeight * 9 || NPC.frame.Y >= frameHeight * 14)
					{
						NPC.frame.Y = 8 * frameHeight;
					}

					if (NPC.frameCounter > 8)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 13 && NPC.frame.Y < frameHeight * 14)
					{
						NPC.frame.Y = 10 * frameHeight;
					}
				}
			}
			else
			{
				if (NPC.frame.Y < frameHeight * 5)
				{
					NPC.frame.Y = 8 * frameHeight;
				}

				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 14)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
        }

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[1] < 1 && Aggression <= 0)
			{
				NPC.ai[1] = 1;
			}

			if (Aggression > 0)
			{
				Aggression += 60;
			}
		}
		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[1] < 1 && Aggression <= 0)
			{
				NPC.ai[1] = 1;
			}

			if (Aggression > 0)
			{
				Aggression += 60;
			}
		}

		public override bool CheckActive()
        {
            return !AnyPlayersInBiome();
        }

		public bool AnyPlayersInBiome()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                int playerInBiomeCount = 0;

                if (!player.dead && player.InModBiome(ModContent.GetInstance<ZombieOceanBiome>()))
                {
                    playerInBiomeCount++;
                }

                if (playerInBiomeCount >= 1)
                {
                    return true;
                }
            }

            return false;
        }

		public override void AI()
		{
			if (!TargetedPlayer.active || TargetedPlayer.dead)
			{
				Aggression = 0;
			}

			int BodyFrameRate = Aggression > 0 ? 4 : 8;

			BodyFrameCounter++;
			if (BodyFrameCounter % BodyFrameRate == 0)
			{
				if (BodyFrame >= 7)
				{
					BodyFrame = 0;
				}
				else
				{
					BodyFrame++;
				}
			}

			if (BiteAnimationTimer > 0)
			{
				BiteAnimation = true;
				BiteAnimationTimer--;
			}
			else
			{
				BiteAnimation = false;
			}
	
			float RotateDirection = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.TwoPi;
			float RotateSpeed = 0.04f;

			if (RotateDirection < 0f)
			{
				RotateDirection += (float)Math.PI * 2f;
			}
			if (RotateDirection > (float)Math.PI * 2f)
			{
				RotateDirection -= (float)Math.PI * 2f;
			}

			if (NPC.rotation < RotateDirection)
			{
				if ((double)(RotateDirection - NPC.rotation) > Math.PI)
				{
					NPC.rotation -= RotateSpeed;
				}
				else
				{
					NPC.rotation += RotateSpeed;
				}
			}
			if (NPC.rotation > RotateDirection)
			{
				if ((double)(NPC.rotation - RotateDirection) > Math.PI)
				{
					NPC.rotation += RotateSpeed;
				}
				else
				{
					NPC.rotation -= RotateSpeed;
				}
			}
			if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
			{
				NPC.rotation = RotateDirection;
			}
			if (NPC.rotation < 0f)
			{
				NPC.rotation += (float)Math.PI * 2f;
			}
			if (NPC.rotation > (float)Math.PI * 2f)
			{
				NPC.rotation -= (float)Math.PI * 2f;
			}
			if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
			{
				NPC.rotation = RotateDirection;
			}

			//constantly call stepup collision so it doesnt get stuck on blocks
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			//position infront of dunk where dunks "sight" is
			Vector2 InfrontOfDunk = new Vector2(125, 0).RotatedBy(NPC.rotation + MathHelper.TwoPi) + NPC.Center;

			if (NPC.ai[1] <= 0 && Aggression <= 0)
			{
				foreach (Player player in Main.ActivePlayers)
				{
					if (!player.dead)
					{
						bool PlayerLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
						bool ShouldBecomeAggressive = PlayerLineOfSight && player.Distance(InfrontOfDunk) <= 200;

						if (ShouldBecomeAggressive)
						{
							TargetedPlayer = player;
							NPC.ai[1] = 1;
						}
					}
				}
			}	

			/*
			//create dust ring for debugging
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 200);
                offset.Y += (float)(Math.Cos(angle) * 200);
                Dust dust = Main.dust[Dust.NewDust(InfrontOfDunk + offset - new Vector2(4, 4), 0, 0, DustID.GreenTorch, 0, 0, 100, Color.White, 1f)];
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.scale = 2.5f;
            }
			*/

			if (NPC.ai[1] > 0 && Aggression <= 0)
			{
				NPC.noTileCollide = false;
				NPC.velocity *= 0.92f;

				NPC.ai[1]++;

				if (NPC.ai[1] == 2)
				{
					SoundEngine.PlaySound(StingSound, NPC.Center);

					Dust.NewDustPerfect(new Vector2(NPC.Center.X, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 2f);
					Dust.NewDustPerfect(new Vector2(NPC.Center.X - 50, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 2f);
					Dust.NewDustPerfect(new Vector2(NPC.Center.X + 50, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 2f);
				}

				if (NPC.ai[1] == 30)
				{
					NPC.ai[2] = 1;

					SoundEngine.PlaySound(RoarSound, NPC.Center);

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 12f, 400f);
				}

				if (NPC.ai[1] >= 85)
				{
					Aggression = 600;
				}
			}
			
			//if there are no active players in the biome then despawn
			if (!AnyPlayersInBiome())
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
				NPC.ai[2] = 0;

				NPC.noTileCollide = true;

				NPC.velocity.Y += 0.4f;
				NPC.EncourageDespawn(10);

				return;
			}

			int[] InvalidBlockTypes = new int[]
			{
				ModContent.TileType<OceanSand>(),
				ModContent.TileType<OceanBiomass>(),
				ModContent.TileType<OceanMeat>(),
				ModContent.TileType<LabMetalPipe>(),
				ModContent.TileType<LabMetalPlate>()
			};

			SolidCollisionDestroyTiles(NPC.position, NPC.width, NPC.height, InvalidBlockTypes);

			//passive roaming pathfinding movement
			if (Aggression <= 0)
			{
				//find a random position to go to
				if (NPC.ai[0] == 0)
				{
					int randomPoint = Main.rand.Next(0, Flags.ZombieBiomePositions.Count);
					PositionGoTo = Flags.ZombieBiomePositions[randomPoint] * 16;

					NPC.ai[0] = 1;
				}
				//go to the position
				else
				{
					NPC.noTileCollide = true;

					if (NPC.Distance(PositionGoTo) > 150f)
					{
						bool HasLineOfSight = Collision.CanHitLine(PositionGoTo - new Vector2(10, 10), 20, 20, NPC.position, NPC.width, NPC.height);

						//only use pathfinding if it doesnt have line of sight to the position
						if (!HasLineOfSight)
						{
							PathfindingMovement(PositionGoTo, 2.5f, 20, 7000, false);
						}
						else
						{
							Vector2 desiredVelocity = NPC.DirectionTo(PositionGoTo) * 2.5f;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
					}
					else
					{
						NPC.ai[0] = 0;
					}
				}
			}
			//hostile chasing pathfinding
			else
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;

				float Speed = TargetedPlayer.Distance(NPC.Center) >= 300f ? 4.25f : 3.5f;

				//quickly loose aggression if the player leaves the biome
				if (!TargetedPlayer.InModBiome<ZombieOceanBiome>())
				{
					Aggression -= 20;
				}
				else
				{
					if (TargetedPlayer.Distance(NPC.Center) <= 100f)
					{
						BiteAnimationTimer = 36;
					}

					//only use pathfinding if it doesnt have line of sight to the player
					bool PlayerLineOfSight = Collision.CanHitLine(TargetedPlayer.position, TargetedPlayer.width, TargetedPlayer.height, NPC.position, NPC.width, NPC.height);
					if (!PlayerLineOfSight)
					{
						NPC.noTileCollide = true;

						PathfindingMovement(TargetedPlayer.Center, Speed, 50, 7000, true);

						//decrease aggression timer
						Aggression--;
					}
					else
					{
						NPC.noTileCollide = false;

						Vector2 desiredVelocity = NPC.DirectionTo(TargetedPlayer.Center) * Speed;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
				}

				if (Aggression <= 5)
				{
					Aggression = 0;
					NPC.ai[2] = 0;
				}
			}
		}

		//custom copied version of vanilla SolidCollision but with a list of specific tiles
		//used for the dunkelosteus so that it only checks for specific tiles when pathfinding since it is meant to destroy any tiles that arent a part of the zombie ocean biome
		public void SolidCollisionDestroyTiles(Vector2 Position, int Width, int Height, int[] InvalidTiles)
		{
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector = default(Vector2);
			for (int i = num; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile == null || !tile.HasTile || InvalidTiles.Contains(tile.TileType) || !Main.tileSolid[tile.TileType])
					{
						continue;
					}

					NPC.ai[3]++;
					if (NPC.ai[3] > 0)
					{
						NPC.velocity *= 0.95f;
					}
					if (NPC.ai[3] == 10)
					{
						BiteAnimationTimer = 36;
					}

					if (NPC.ai[3] >= 10 && BiteAnimationTimer == 5)
					{
						for (int x = num; x < value2; x++)
						{
							for (int y = value3; y < value4; y++)
							{
								Tile tile2 = Main.tile[x, y];
								bool flag2 = tile2 != null && tile2.HasTile && !InvalidTiles.Contains(tile2.TileType) && Main.tileSolid[tile2.TileType];
								if (flag2)
								{
									WorldGen.KillTile(x, y);
								}
							}
						}

						NPC.ai[3] = 0;
					}
				}
			}
		}

		private static bool SolidCollideArea(Vector2 Center, int Width, int Height)
		{
			int[] BlockTypes = new int[]
			{
				ModContent.TileType<OceanSand>(),
				ModContent.TileType<OceanBiomass>(),
				ModContent.TileType<OceanMeat>(),
				ModContent.TileType<LabMetalPipe>(),
				ModContent.TileType<LabMetalPlate>()
			};

			return PathFinding.SolidCollisionWithSpecificTiles(Center - new Vector2(Width / 2, Height / 2), Width, Height, BlockTypes);
		}

		private void PathfindingMovement(Vector2 position, float Speed, int DistanceCheck, int Iterations, bool FollowingPlayer)
		{
			// Once every few seconds, sync the npc - bandaid on pathfinder in mp
			if (++syncTimer > 60)
			{
				NPC.netUpdate = true;
				syncTimer = 0;
			}

			bool FindClosestNode = false;

			//go to positions around the player, and if they arent valid try offsetting the position to allow dunk to find the location it needs to go to
			//this is done by just doing an additional solid collision check around every compass direction and every diagonal position from the position it needs to go to
			Vector2 RealPosition = (position + new Vector2(0, DistanceCheck)); 
			if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
			{
				RealPosition = (position + new Vector2(0, -DistanceCheck));
				if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
				{
					RealPosition = (position + new Vector2(DistanceCheck, 0));
					if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
					{
						RealPosition = (position + new Vector2(-DistanceCheck, 0));
						if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
						{
							RealPosition = (position + new Vector2(DistanceCheck, DistanceCheck));
							if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
							{
								RealPosition = (position + new Vector2(DistanceCheck, -DistanceCheck));
								if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
								{
									RealPosition = (position + new Vector2(-DistanceCheck, DistanceCheck));
									if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
									{
										RealPosition = (position + new Vector2(-DistanceCheck, -DistanceCheck));
										if (SolidCollideArea(RealPosition, NPC.width, NPC.height))
										{
											if (FollowingPlayer)
											{
												FindClosestNode = true;
												BiomePositionDistances.Clear();
											}
										}
									}
								}
							}
						}
					}
				}
			}

			//if for whatever reason the player is not reachable after the above position checks, then attempt to pathfind to the closest "node" position in the biome
			if (FindClosestNode)
			{
				//get the distance between the player and every position in the zombie biome and add them to the position distances list
				foreach (Vector2 pos in Flags.ZombieBiomePositions)
				{
					int Dist = (int)position.Distance(pos * 16);
					BiomePositionDistances.Add(Dist);
				}

				//find the minimum distance values index and set the position to pathfind to
				int minimumValueIndex = BiomePositionDistances.IndexOf(BiomePositionDistances.Min());
				RealPosition = Flags.ZombieBiomePositions[minimumValueIndex] * 16;
			}

			//find path based on the offset position decided above
			Point16 pathStart = NPC.Center.ToTileCoordinates16();
			Point16 pathEnd = RealPosition.ToTileCoordinates16();
			pathfinder.CheckDrawPath(pathStart, pathEnd, Iterations, new Vector2(NPC.width / 16f, NPC.height / 16f), new Vector2(-NPC.width / 2, -NPC.width / 2));

			bool FoundValidPath = pathfinder.HasPath && pathfinder.Path.Count > 0;

			if (FoundValidPath)
			{
				int index = pathfinder.Path.IndexOf(pathfinder.Path.MinBy(x => x.Position.ToVector2().DistanceSQ(NPC.Center / 16f)));
				List<PathFinding.FoundPoint> checkPoints = pathfinder.Path[^(Math.Min(pathfinder.Path.Count, 6))..];
				Vector2 direction = -AveragePathDirection(checkPoints, Speed);

				NPC.velocity = Vector2.Lerp(NPC.velocity, direction, 0.08f);

				/*
				//debug to show the calculated path with dusts
				foreach (PathFinding.FoundPoint point in pathfinder.Path)
				{
					var Velocity = PathFinding.ToVector2(point.Direction);
					int Type = checkPoints.Contains(point) ? point == checkPoints.Last() ? DustID.Poisoned : DustID.GreenFairy : DustID.YellowStarDust;
					var NewDust = Dust.NewDustPerfect(point.Position.ToWorldCoordinates(), Type, Velocity * 2);
					NewDust.noGravity = true;
				}
				*/
			}
		}

		private static Vector2 AveragePathDirection(List<PathFinding.FoundPoint> foundPoints, float Speed)
		{
			Vector2 dir = Vector2.Zero;

			foreach (PathFinding.FoundPoint point in foundPoints)
			{
				dir += PathFinding.ToVector2(point.Direction) * Speed;
			}

			return dir / foundPoints.Count;
		}
	}

	public class DunkleosteusScene : ModSceneEffect
	{
		private int DunkNPC = -1;

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DunkleosteusChase");

		public override SceneEffectPriority Priority => SceneEffectPriority.BossLow;

		public override bool IsSceneEffectActive(Player player) => FindHostileDunkleosteus() && !Main.gameMenu;

		private bool FindHostileDunkleosteus()
		{
			if (DunkNPC >= 0 && Main.npc[DunkNPC].active && Main.npc[DunkNPC].type == ModContent.NPCType<Dunkleosteus>() && Main.npc[DunkNPC].ai[2] > 0)
			{
				return true;
			}

			DunkNPC = NPC.FindFirstNPC(ModContent.NPCType<Dunkleosteus>());

			return DunkNPC != -1 && Main.npc[DunkNPC].ai[2] > 0;
		}
	}
}