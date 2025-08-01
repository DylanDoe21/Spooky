using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Minibiomes.Ocean;
using Spooky.Content.Projectiles.Minibiomes.Ocean;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class Dunkleosteus : ModNPC
	{
		private readonly PathFinding pathfinder = new PathFinding(15);

		int SyncTimer = 0;
		int BodyFrame = 0;
		int BodyFrameCounter = 0;
		int Aggression = 0;
		
		int BiteAnimationTimer = 0;
		bool BiteAnimation = false;
		bool AteBomb = false;

		int RoarAnimationTimer = 0;
		bool RoarAnimation = false;

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
		public static readonly SoundStyle GulpSound = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusGulp", SoundType.Sound);
		public static readonly SoundStyle ExplodeSound = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusExplode", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 14;
			NPCID.Sets.TrailCacheLength[NPC.type] = 20;
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

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(PositionGoTo);

			//ints
			writer.Write(SyncTimer);
			writer.Write(BodyFrame);
			writer.Write(BodyFrameCounter);
			writer.Write(BiteAnimationTimer);
			writer.Write(RoarAnimationTimer);
			writer.Write(Aggression);

			//bools
			writer.Write(BiteAnimation);
			writer.Write(RoarAnimation);
			writer.Write(AteBomb);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			PositionGoTo = reader.ReadVector2();

			//ints
			SyncTimer = reader.ReadInt32();
			BodyFrame = reader.ReadInt32();
			BodyFrameCounter = reader.ReadInt32();
			BiteAnimationTimer = reader.ReadInt32();
			RoarAnimationTimer = reader.ReadInt32();
			Aggression = reader.ReadInt32();

			//bools
			BiteAnimation = reader.ReadBoolean();
			RoarAnimation = reader.ReadBoolean();
			AteBomb = reader.ReadBoolean();
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 10000;
			NPC.damage = 150;
			NPC.defense = 9999;
			NPC.width = 90;
			NPC.height = 90;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.waterMovementSpeed = 1f;
			NPC.SuperArmor = true;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.behindTiles = true;
			NPC.immortal = true;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = RoarSound with { Pitch = -1.2f };
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.damage = (int)(NPC.damage * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Dunkleosteus"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusGlow");
			BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusBody");
			BodyTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Ocean/DunkleosteusBodyExtra");

			var effects = NPC.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

			if (NPC.ai[1] > 0 && Aggression <= 0)
			{
				effects = NPC.Center.X < TargetedPlayer.Center.X ? SpriteEffects.None : SpriteEffects.FlipVertically;
			}

			//draw body
			Vector2 pos = new Vector2(effects == SpriteEffects.None ? 12 : -12, 7).RotatedBy(NPC.rotation + MathHelper.PiOver2) + NPC.Center;
			Vector2 drawOrigin = new Vector2(BodyTexture.Width() * 0.5f, (BodyTexture.Height() / 8) * 0.5f);
			spriteBatch.Draw(BodyTexture.Value, pos - screenPos, new Rectangle(0, 122 * BodyFrame, 470, 122), drawColor, NPC.oldRot[NPC.oldPos.Length / 2], drawOrigin, NPC.scale, effects, 0);

			//draw extra body texture when its mouth isnt open so it doesnt look odd when rotating while passively swimming
			if (NPC.ai[1] <= 0 && Aggression <= 0 && !BiteAnimation && !RoarAnimation)
			{
				spriteBatch.Draw(BodyTexture2.Value, pos - screenPos, new Rectangle(0, 122 * BodyFrame, 470, 122), drawColor, NPC.oldRot[NPC.oldPos.Length - 1], drawOrigin, NPC.scale, effects, 0);
			}

			//draw head and glowmask
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

			if (!BiteAnimation && !RoarAnimation)
			{
				if ((NPC.ai[2] <= 0 && Aggression <= 0) || AteBomb)
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
			else if (BiteAnimation)
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
			else if (RoarAnimation)
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

				if (RoarAnimationTimer < 25)
				{
					if (NPC.frame.Y >= frameHeight * 14)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}
				else
				{
					if (NPC.frame.Y >= frameHeight * 13 && NPC.frame.Y < frameHeight * 14)
					{
						NPC.frame.Y = 10 * frameHeight;
					}
				}
			}
        }

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[1] < 1 && Aggression <= 0)
			{
				NPC.ai[1] = 1;
			}
		}
		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			if (NPC.ai[1] < 1 && Aggression <= 0)
			{
				NPC.ai[1] = 1;
			}
		}

		public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
		{
			//big dunk damage ignores all player defense
			modifiers.ScalingArmorPenetration += 1f;
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

			int BodyFrameRate = AteBomb ? 15 : (Aggression > 0 ? 4 : 8);

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

			if (RoarAnimationTimer > 0)
			{
				RoarAnimation = true;
				RoarAnimationTimer--;
			}
			else
			{
				RoarAnimation = false;
			}

			//rotation stuff
            float RotateDirection = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.TwoPi;
			float RotateSpeed = 0.04f;

			if (NPC.ai[1] > 0 && Aggression <= 0)
			{
				Vector2 RotateTowards = TargetedPlayer.Center - NPC.Center;
                RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + MathHelper.TwoPi;
			}
			else
			{
				RotateDirection = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.TwoPi;
			}

			NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);

			if (Aggression > 2)
			{
				foreach (var proj in Main.ActiveProjectiles)
				{
					if (proj.type == ModContent.ProjectileType<MineProj>() && proj.Distance(NPC.Center) <= 200)
					{
						Vector2 desiredVelocity = proj.DirectionTo(NPC.Center) * 5;
						proj.velocity = Vector2.Lerp(proj.velocity, desiredVelocity, 1f / 20);

						if (proj.Hitbox.Intersects(new Rectangle((int)NPC.Center.X - 20, (int)NPC.Center.Y - 20, 40, 40)))
						{
							proj.scale -= 0.25f;

							if (proj.scale <= 0)
							{
								AteBomb = true;
								proj.active = false;
							}
						}
					}
				}
			}

			if (AteBomb)
			{
				Vector2 desiredVelocity = NPC.DirectionTo(TargetedPlayer.Center) * 1;
				NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
				NPC.velocity *= 0.98f;

				NPC.localAI[0]++;

				if (NPC.localAI[0] == 2)
				{
					SoundEngine.PlaySound(GulpSound, NPC.Center);

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 2f, 500f);
				}

				if (NPC.localAI[0] >= 180)
				{
					NPC.SuperArmor = false;
					NPC.immortal = false;

					SoundEngine.PlaySound(ExplodeSound, NPC.Center);

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 20f, 700f);

					//spawn gores
					for (int numGores = 1; numGores <= 10; numGores++)
					{
						if (Main.netMode != NetmodeID.Server)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), ModContent.Find<ModGore>("Spooky/DunkleosteusGore" + numGores).Type);
						}
					}

					//flame dusts
					for (int numDust = 0; numDust < 50; numDust++)
					{
						int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, 5f);
						Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-20f, 20f);
						Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-10f, 10f);
						Main.dust[dustGore].noGravity = true;
					}

					//explosion smoke
					for (int numExplosion = 0; numExplosion < 25; numExplosion++)
					{
						int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));
						Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
						Main.dust[DustGore].noGravity = true;
					}

					//death message
					string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.DunkleosteusDefeat");

					if (Main.netMode != NetmodeID.Server)
					{
						Main.NewText(text, 171, 64, 255);
					}
					else
					{
						ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
					}

					Flags.downedDunkleosteus = true;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}

					TargetedPlayer.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false, null, true);
				}

				return;
			}

			//if there are no active players in the biome then despawn
			if (!AnyPlayersInBiome())
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;
				NPC.ai[2] = 0;

				NPC.velocity.Y += 0.4f;
				NPC.EncourageDespawn(10);

				return;
			}

			//constantly call stepup collision so it doesnt get stuck on blocks
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			//position infront of dunk where dunks "sight" is
			Vector2 InfrontOfDunk = new Vector2(110, 0).RotatedBy(NPC.rotation + MathHelper.TwoPi) + NPC.Center;
			Vector2 BigDunkBody = -new Vector2(60, 0).RotatedBy(NPC.rotation + MathHelper.TwoPi) + NPC.Center;

			if (NPC.ai[1] <= 0 && Aggression <= 0)
			{
				foreach (Player player in Main.ActivePlayers)
				{
					if (!player.dead)
					{
						bool PlayerLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
						bool ShouldBecomeAggressive = PlayerLineOfSight && (player.Distance(InfrontOfDunk) <= 175 || player.Distance(BigDunkBody) <= 125);

						if (ShouldBecomeAggressive)
						{
							TargetedPlayer = player;
							NPC.ai[1] = 1;
							NPC.netUpdate = true;
						}
					}
				}
			}

			/*
			//create dust rings for debugging
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 125);
                offset.Y += (float)(Math.Cos(angle) * 125);
                Dust dust = Main.dust[Dust.NewDust(BigDunkBody + offset - new Vector2(4, 4), 0, 0, DustID.RedTorch, 0, 0, 100, Color.White, 1f)];
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.scale = 2.5f;
            }
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 175);
                offset.Y += (float)(Math.Cos(angle) * 175);
                Dust dust = Main.dust[Dust.NewDust(InfrontOfDunk + offset - new Vector2(4, 4), 0, 0, DustID.GreenTorch, 0, 0, 100, Color.White, 1f)];
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.scale = 2.5f;
            }
			*/

			if (NPC.ai[1] > 0 && Aggression <= 0)
			{
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
					NPC.netUpdate = true;
				}
			}

			SolidCollisionDestroyTiles(NPC.position, NPC.width, NPC.height, PathFinding.BlockTypes);

			//passive roaming pathfinding movement
			if (Aggression <= 0)
			{
				if (NPC.ai[1] > 0)
				{
					Vector2 desiredVelocity = NPC.DirectionTo(TargetedPlayer.Center) * 1;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
				}
				else
				{
					if (Main.rand.NextBool(1500) && RoarAnimationTimer <= 0)
					{
						SoundStyle[] Sounds = new SoundStyle[] { GrowlSound1, GrowlSound2 };

						SoundEngine.PlaySound(Main.rand.Next(Sounds), NPC.Center);

						RoarAnimationTimer = 75;
					}

					//find a random position to go to
					if (NPC.ai[0] == 0)
					{
						int randomPoint = Main.rand.Next(0, Flags.ZombieBiomePositions.Count);

						while (NPC.Distance(Flags.ZombieBiomePositions[randomPoint] * 16) > 3500f)
						{
							randomPoint = Main.rand.Next(0, Flags.ZombieBiomePositions.Count);
						}

						PositionGoTo = Flags.ZombieBiomePositions[randomPoint] * 16;

						NPC.ai[0] = 1;

						NPC.netUpdate = true;
					}
					//go to the position
					else
					{
						if (NPC.Distance(PositionGoTo) > 150f)
						{
							PathfindingMovement(PositionGoTo, 2.2f, 20, 7000, false);
							NPC.noTileCollide = true;
						}
						else
						{
							NPC.ai[0] = 0;
						}
					}
				}
			}
			//hostile chasing pathfinding
			else
			{
				NPC.ai[0] = 0;
				NPC.ai[1] = 0;

				float Speed = TargetedPlayer.Distance(NPC.Center) >= 300f ? 3f : 2.5f;

				if (Main.expertMode && !Main.masterMode)
				{
					Speed = TargetedPlayer.Distance(NPC.Center) >= 300f ? 3.5f : 3f;
				}
				else if (Main.masterMode)
				{
					Speed = TargetedPlayer.Distance(NPC.Center) >= 300f ? 3.8f : 3.5f;
				}

				//quickly loose aggression if the player leaves the biome
				if (!TargetedPlayer.InModBiome<ZombieOceanBiome>())
				{
					Aggression -= 20;
				}
				else
				{
					if (TargetedPlayer.Distance(NPC.Center) <= 150f)
					{
						BiteAnimationTimer = 36;
					}

					//only use pathfinding if it doesnt have line of sight to the player
					bool PlayerLineOfSight = Collision.CanHitLine(TargetedPlayer.Center - new Vector2(1, 1), 2, 2, NPC.position, NPC.width, NPC.height);
					if (!PlayerLineOfSight)
					{
						PathfindingMovement(TargetedPlayer.Center, Speed, 70, 7000, true);
						NPC.noTileCollide = true;

						//decrease aggression
						Aggression--;
					}
					else
					{
						Vector2 desiredVelocity = NPC.DirectionTo(TargetedPlayer.Center) * Speed;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						NPC.noTileCollide = false;
					}
				}

				if (Aggression == 1)
				{
					SoundStyle[] Sounds = new SoundStyle[] { GrowlSound1, GrowlSound2 };

					SoundEngine.PlaySound(Main.rand.Next(Sounds), NPC.Center);

					RoarAnimationTimer = 75;

					Aggression = 0;
					NPC.ai[2] = 0;

					NPC.netUpdate = true;
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
									if (Main.netMode == NetmodeID.MultiplayerClient)
									{
										NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
									}
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
			return TileGlobal.SolidCollisionWithSpecificTiles(Center - new Vector2(Width / 2, Height / 2), Width, Height, PathFinding.BlockTypes);
		}

		private void PathfindingMovement(Vector2 position, float Speed, int DistanceCheck, int Iterations, bool FollowingPlayer)
		{
			bool FindClosestNode = false;

			//go to positions around the player, and if they arent valid try offsetting the position to allow dunk to find the location it needs to go to
			//this is done by just doing an additional solid collision check around every compass direction and every diagonal position from the position it needs to go to
			Vector2 RealPosition = Vector2.Zero;

			//do a loop to check a box around the player for a valid spot to pathfind to
			for (int X = -DistanceCheck; X < DistanceCheck; X++)
			{
				if (RealPosition != Vector2.Zero)
				{
					break;
				}

				for (int Y = -DistanceCheck; Y < DistanceCheck; Y++)
				{
					if (SolidCollideArea(position + new Vector2(X, Y), NPC.width + 20, NPC.height + 20))
					{
						continue;
					}
					else
					{
						RealPosition = position + new Vector2(X, Y);
						break;
					}
				}
			}

			//if a position is not able to be found after the loop, then find the closest node
			if (RealPosition == Vector2.Zero)
			{
				if (FollowingPlayer)
				{
					FindClosestNode = true;
					NPC.netUpdate = true;
				}
				else
				{
					NPC.ai[0] = 0;
				}
			}

			//if for whatever reason the player is not reachable after the above position checks, then attempt to pathfind to the closest "node" position in the biome
			if (FindClosestNode)
			{
				BiomePositionDistances.Clear();
				
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

			if (!FoundValidPath || (FindClosestNode && NPC.Distance(RealPosition) < 100f))
			{
				if (Aggression > 2)
				{
					Aggression = 2;
				}
			}
			else
			{
				int index = pathfinder.Path.IndexOf(pathfinder.Path.MinBy(x => x.Position.ToVector2().DistanceSQ(NPC.Center / 16f)));
				List<PathFinding.FoundPoint> checkPoints = pathfinder.Path[^(Math.Min(pathfinder.Path.Count, 6))..];
				Vector2 direction = -GetPathDirection(checkPoints, Speed);

				NPC.velocity = Vector2.Lerp(NPC.velocity, direction, 0.08f);
			}
		}

		private static Vector2 GetPathDirection(List<PathFinding.FoundPoint> foundPoints, float Speed)
		{
			Vector2 dir = Vector2.Zero;

			foreach (PathFinding.FoundPoint point in foundPoints)
			{
				dir += PathFinding.ToVector2(point.Direction) * Speed;
			}

			return dir / foundPoints.Count;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			var parameters = new DropOneByOne.Parameters() 
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 75,
				MaximumItemDropsCount = 100,
			};

			npcLoot.Add(new DropOneByOne(ModContent.ItemType<DunkleosteusHide>(), parameters));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SeaSeed>(), 1, 1, 2));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DunkleosteusRelicItem>()));
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