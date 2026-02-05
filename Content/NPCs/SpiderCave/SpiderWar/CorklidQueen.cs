using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.NPCs.SpiderCave.Projectiles;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class CorklidQueen : ModNPC
	{
		int CurrentFrameX = 0; //0 = pop out of ground animation  1 = walking animation
		int IdleFrameToUse = 0;
		int TeleportTimer = 0;
		int TeleportFrameTimer = 0;
		int TeleportDelay = 0;
		int SoundDelay = 0;
		int SaveDirection;

		float SpeedModifier = 0f;

		bool JustTeleported = false;

		public ushort destinationX = 0;
		public ushort destinationY = 0;

		public enum AnimationState
		{
			GoInGround, JumpOut, Hiding, Walking, StayStill
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/CorklidQueenBestiary",
				Position = new Vector2(35f, 35f),
				PortraitPositionXOverride = 0f,
              	PortraitPositionYOverride = 12f
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
			writer.Write(TeleportTimer);
            writer.Write(TeleportFrameTimer);
			writer.Write(TeleportDelay);
            writer.Write(SoundDelay);

			//ushort
			writer.Write((ushort)destinationX);
            writer.Write((ushort)destinationY);

			//bools
            writer.Write(JustTeleported);

            //floats
            writer.Write(SpeedModifier);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
			TeleportTimer = reader.ReadInt32();
            TeleportFrameTimer = reader.ReadInt32();
			TeleportDelay = reader.ReadInt32();
            SoundDelay = reader.ReadInt32();

			//ushort
			destinationX = reader.ReadUInt16();
            destinationY = reader.ReadUInt16();

			//bools
            JustTeleported = reader.ReadBoolean();

            //floats
            SpeedModifier = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 13000;
            NPC.damage = 50;
			NPC.defense = 15;
			NPC.width = 124;
			NPC.height = 118;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = -1.5f };
			NPC.DeathSound = SoundID.NPCDeath31 with { Pitch = -1f };
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void OnSpawn(IEntitySource source)
		{
			int NewNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y - 1000, ModContent.NPCType<SpotlightFirefly>(), ai3: NPC.whoAmI);
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
			}
		}

		public override bool CheckActive()
		{
			return !SpiderWarWorld.SpiderWarActive;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CorklidQueen"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
			{
				NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
			}

			NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

			//go into the ground, use Xframe 0 and plays animation in reverse
			if (CurrentAnimation == AnimationState.GoInGround)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 7)
				{
					NPC.frame.Y = NPC.frame.Y - frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y <= frameHeight * 0)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			//emerge out of the ground, use Xframe 0 and plays animation normally
			else if (CurrentAnimation == AnimationState.JumpOut)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 3)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 6)
				{
					NPC.frame.Y = 5 * frameHeight;
				}
			}
			//same as above, but it stops at the frame right before its real body comes out of the ground for attacks
			else if (CurrentAnimation == AnimationState.Hiding)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 3)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 3 * frameHeight;
				}
			}
			//normal walking animation, use Xframe 1
			else if (CurrentAnimation == AnimationState.Walking)
			{
				NPC.frameCounter++;
				if (NPC.velocity.X != 0)
				{
					if (NPC.frameCounter > 13 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 7)
					{
						NPC.frame.Y = 1 * frameHeight;
					}
				}
				else
				{
					NPC.frame.Y = 0 * frameHeight;
				}

				//jumping up frame
				if (NPC.velocity.Y < 0)
				{
					NPC.frame.Y = 4 * frameHeight;
				}
				//falling frame
				if (NPC.velocity.Y > 0)
				{
					NPC.frame.Y = 5 * frameHeight;
				}
			}
			//idle frame, can be used with any Xframe and any frame can be used 
			//this can be used for actual idle frames, or for setting the animation to a specific frame before doing another animation
			else if (CurrentAnimation == AnimationState.StayStill)
			{
				NPC.frame.Y = IdleFrameToUse * frameHeight;
			}
		}

		public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
		{
			if (CurrentAnimation == AnimationState.Hiding)
			{
   				modifiers.FinalDamage *= 0.25f;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return CurrentAnimation == AnimationState.Walking;
        }

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			bool AnotherMinibossPresent = SpiderWarWorld.EventActiveNPCCount() > 1;

			switch ((int)NPC.ai[0])
			{
				//walk at player, dig into ground and teleport if line of sight is lost
				case 0:
				{
					NPC.localAI[0]++;
					if (NPC.localAI[0] >= 480)
					{
						NPC.ai[0] = Main.rand.Next(1, 3);
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.netUpdate = true;
					}

					//constantly call stepup collision so it doesnt get stuck on blocks
					Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

					//walk at the player normally
					if (NPC.localAI[2] < 10)
					{
						bool SlowDown = false;

						NPC.localAI[1]++;

						//when it first enters this state, play the jump out of ground animation
						if (NPC.localAI[1] < 25)
						{
							CurrentFrameX = 0;
							CurrentAnimation = AnimationState.JumpOut;

							NPC.velocity.X = 0;
						}
						//play sounds and spawn debris after jump out animation
						else if (NPC.localAI[1] == 25)
						{
							SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);
							Screenshake.ShakeScreenWithIntensity(NPC.Center, 10f, 450f);

							//shoot debris
							int MaxDebris = AnotherMinibossPresent ? 5 : 8;
							for (int numProjs = 0; numProjs < MaxDebris; numProjs++)
							{
								Vector2 PosToShootTo = new Vector2(NPC.Center.X + Main.rand.Next(-30, 31), NPC.Center.Y - 30);

								Vector2 ShootSpeed = PosToShootTo - NPC.Center;
								ShootSpeed.Normalize();
								ShootSpeed *= Main.rand.NextFloat(10f, 15f);

								NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.NextFloat(-45f, 45f), NPC.Bottom.Y), 
								ShootSpeed, ModContent.ProjectileType<CorklidQueenDebris>(), NPC.damage, 4.5f);
							}

							//spawn dusts
							for (int numDusts = 0; numDusts < 35; numDusts++)
							{
								Vector2 PosToShootTo = new Vector2(NPC.Center.X + Main.rand.Next(-30, 31), NPC.Center.Y - 30);

								Vector2 ShootSpeed = PosToShootTo - NPC.Center;
								ShootSpeed.Normalize();
								ShootSpeed *= Main.rand.NextFloat(5f, 10f);

								Dust mudDust = Dust.NewDustPerfect(new Vector2(NPC.Center.X + Main.rand.NextFloat(-45f, 45f), NPC.Bottom.Y), DustID.Mud, ShootSpeed * 0.5f, default, default, 2f);
								mudDust.color = Color.White;
							}

							NPC.netUpdate = true;
						}
						//run towards player and teleport to them if out of light of sight
						else
						{
							bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
							if (!HasLineOfSight)
							{
								NPC.localAI[2]++;
							}

							CurrentFrameX = 1;
							CurrentAnimation = AnimationState.Walking;

							if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
							{
								SlowDown = true;
							}
							if (SlowDown)
							{
								NPC.velocity.X *= 0.9f;
								if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
								{
									NPC.velocity.X = 0f;
								}
							}
							else
							{
								SoundDelay++;
								if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && SoundDelay > 60 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X) * 5)
								{
									SoundEngine.PlaySound(SoundID.DeerclopsStep with { Volume = 1.2f, Pitch = -0.5f }, NPC.Center);
									Screenshake.ShakeScreenWithIntensity(NPC.Center, 4f, 450f);

									SoundDelay = 0;
								}

								if (NPC.direction > 0)
								{
									if (SpeedModifier <= 0f)
									{
										SpeedModifier = 1f;
									}
									if (SpeedModifier < 10f)
									{
										SpeedModifier *= 1.035f;
									}

									NPC.velocity.X = (NPC.velocity.X * 20f + SpeedModifier) / 21f;
								}
								if (NPC.direction < 0)
								{
									if (SpeedModifier >= 0f)
									{
										SpeedModifier = -1f;
									}
									if (SpeedModifier > -10f)
									{
										SpeedModifier *= 1.035f;
									}

									NPC.velocity.X = (NPC.velocity.X * 20f + SpeedModifier) / 21f;
								}
							}

							if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
							{
								if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 150) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 1))
								{
									NPC.velocity.Y = -10f;
									NPC.netUpdate = true;
								}
							}
						}
					}
					//dig in ground and teleport to player if line of sight is lost for too long
					else
					{
						TeleportToPlayer(player, true);
						
						if (JustTeleported)
						{
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							JustTeleported = false;

							NPC.netUpdate = true;
						}
					}

					break;
				}

				//shoot up small rockets that come back down above player
				case 1:
				{
					if (NPC.localAI[0] <= 0)
					{
						if (NPCGlobalHelper.IsCollidingWithFloor(NPC))
						{
							TeleportToPlayer(player, true);
								
							if (JustTeleported)
							{
								SaveDirection = NPC.direction;

								NPC.localAI[0] = 1;
								JustTeleported = false;

								NPC.netUpdate = true;
							}
						}
					}
					else
					{
						CurrentFrameX = 0;
						CurrentAnimation = AnimationState.Hiding;

						NPC.spriteDirection = SaveDirection;
						NPC.velocity.X = 0;

						NPC.localAI[0]++;

						int Frequency = AnotherMinibossPresent ? 10 : 5;
						if (NPC.localAI[0] >= 25 && NPC.localAI[0] <= 120 && NPC.localAI[0] % Frequency == 0)
						{
							SoundEngine.PlaySound(SoundID.Item73, NPC.Center);

							Vector2 ShootFrom = new Vector2(NPC.Center.X + (-20 * NPC.spriteDirection), NPC.Center.Y + NPC.height / 4);

							Vector2 PosToShootTo = new Vector2(NPC.Center.X + (-20 * NPC.spriteDirection), NPC.Center.Y - 30);

							Vector2 ShootSpeed = PosToShootTo - ShootFrom;
							ShootSpeed.Normalize();
							ShootSpeed *= 35f;

							Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(12));

							NPCGlobalHelper.ShootHostileProjectile(NPC, ShootFrom, velocity, ModContent.ProjectileType<CorklidRocket>(), NPC.damage, 4.5f, ai0: 0);

							for (int numDusts = 0; numDusts < 12; numDusts++)
							{
								Dust newDust = Dust.NewDustPerfect(ShootFrom, DustID.InfernoFork, new Vector2(ShootSpeed.X + Main.rand.Next(-7, 8), ShootSpeed.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
								newDust.noGravity = true;
							}
						}

						if (NPC.localAI[0] >= 180 && NPC.localAI[0] <= 275 && NPC.localAI[0] % Frequency == 0)
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, player.Center, Vector2.Zero, ModContent.ProjectileType<CorklidMissileReticle>(), NPC.damage, 1f);
							NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y - 1000), new Vector2(0, 25), ModContent.ProjectileType<CorklidRocket>(), NPC.damage, 4.5f, ai0: 1);
						}

						//teleport to player if they are to far away, with no animation change
						if (NPC.localAI[0] > 120)
						{
							bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
							if (!HasLineOfSight)
							{
								TeleportToPlayer(player, false);
							}
						}

						if (NPC.localAI[0] >= 360)
						{
							NPC.ai[0] = 0;
							NPC.localAI[0] = 0;
							NPC.netUpdate = true;
						}
					}

					break;
				}

				//spawn giant homing nuke bomb
				case 2:
				{
					if (NPC.localAI[0] <= 0)
					{
						if (NPCGlobalHelper.IsCollidingWithFloor(NPC))
						{
							TeleportToPlayer(player, true);
								
							if (JustTeleported)
							{
								SaveDirection = NPC.direction;

								NPC.localAI[0] = 1;
								JustTeleported = false;

								NPC.netUpdate = true;
							}
						}
					}
					else
					{
						CurrentFrameX = 0;
						CurrentAnimation = AnimationState.Hiding;

						NPC.spriteDirection = SaveDirection;
						NPC.velocity.X = 0;

						NPC.localAI[0]++;
						if (NPC.localAI[0] == 20)
						{
							SoundEngine.PlaySound(SoundID.Item73, NPC.Center);

							Vector2 ShootFrom = new Vector2(NPC.Center.X + (-20 * NPC.spriteDirection), NPC.Center.Y + NPC.height / 4);

							Vector2 PosToShootTo = new Vector2(NPC.Center.X + (-20 * NPC.spriteDirection), NPC.Center.Y - 30);

							Vector2 ShootSpeed = PosToShootTo - ShootFrom;
							ShootSpeed.Normalize();
							ShootSpeed *= 12f;

							NPCGlobalHelper.ShootHostileProjectile(NPC, ShootFrom, ShootSpeed, ModContent.ProjectileType<CorklidNuke>(), NPC.damage, 4.5f, ai0: 0);

							for (int numDusts = 0; numDusts < 12; numDusts++)
							{
								Dust newDust = Dust.NewDustPerfect(ShootFrom, DustID.InfernoFork, ShootSpeed * 0.5f, default, default, 2f);
								newDust.noGravity = true;
							}
						}

						//teleport to player if they are to far away, with no animation change
						if (NPC.localAI[0] >= 40)
						{
							bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
							if (!HasLineOfSight)
							{
								TeleportToPlayer(player, false);
							}
						}

						if (NPC.localAI[0] >= 460)
						{
							NPC.ai[0] = 0;
							NPC.localAI[0] = 0;
							NPC.netUpdate = true;
						}
					}

					break;
				}
			}
		}

		public void TeleportToPlayer(Player player, bool UseAnimation)
		{
			if (!NPCGlobalHelper.IsCollidingWithFloor(NPC))
			{
				NPC.velocity.X *= 0.95f;

				CurrentFrameX = 1;

				CurrentAnimation = AnimationState.Walking;
			}
			else
			{
				NPC.velocity.X = 0;

				if (UseAnimation)
				{
					CurrentFrameX = 0;

					TeleportFrameTimer++;
					if (TeleportFrameTimer <= 5)
					{
						IdleFrameToUse = 5;
						CurrentAnimation = AnimationState.StayStill;
					}
					else
					{
						CurrentAnimation = AnimationState.GoInGround;
					}
				}

				TeleportTimer++;
				if (TeleportTimer >= 30)
				{
					if (TeleportTimer >= 30 && destinationX == 0 && destinationY == 0 && Main.netMode != NetmodeID.MultiplayerClient)
					{
						Point point = player.Center.ToTileCoordinates();
						Vector2 chosenTile = Vector2.Zero;
						if (NPCGlobalHelper.TeleportToSpot(NPC, player, ref chosenTile, point.X, point.Y, 55, 15))
						{
							destinationX = (ushort)chosenTile.X;
							destinationY = (ushort)chosenTile.Y;
							NPC.netUpdate = true;
						}
					}

					if (destinationX != 0 && destinationY != 0)
					{
						TeleportDelay++;
						if (TeleportDelay <= 10)
						{
							Dust dust = Dust.NewDustDirect(new Vector2((destinationX * 16f) - 30, (destinationY * 16f) - 20), NPC.width, NPC.height, DustID.Mud, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, Color.White, 2.5f);
							dust.color = Color.White;
							dust.noGravity = true;
						}
						else
						{
							NPC.position.X = destinationX * 16f - (float)(NPC.width / 2) + 8f;
							NPC.position.Y = destinationY * 16f - (float)NPC.height;
							NPC.velocity.X = 0f;
							NPC.velocity.Y = 0f;
							NPC.netOffset *= 0f;
							destinationX = 0;
							destinationY = 0;
							TeleportTimer = 0;
							TeleportFrameTimer = 0;
							TeleportDelay = 0;

							SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.5f }, NPC.Center);
							Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 300f);

							JustTeleported = true;
							NPC.netUpdate = true;
						}
					}
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderWarItemDropCondition(), ModContent.ItemType<SpiderWarFlail>()));
			npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderWarItemDropCondition(), ModContent.ItemType<CorklidQueenTrophyItem>()));

			LeadingConditionRule NoBestiaryCondition = new LeadingConditionRule(new DropConditions.HideBestiaryCondition());

			var parameters = new DropOneByOne.Parameters()
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 4,
				MaximumItemDropsCount = 8,
			};

			NoBestiaryCondition.OnSuccess(new DropOneByOne(ItemID.Heart, parameters));
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
				if (SpiderWarWorld.SpiderWarActive)
				{
					SpiderWarWorld.SpiderWarPoints++;
				}

				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<SpotlightFirefly>() && npc.ai[3] == NPC.whoAmI)
					{
						npc.ai[0]++;
					}
				}

				for (int numGores = 1; numGores <= 8; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CorklidQueenGore" + numGores).Type);
					}
				}
            }
        }
	}
}