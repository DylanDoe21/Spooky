using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.SpookFishron.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.SpookFishron
{
	[AutoloadBossHead]
	public class SpookFishron : ModNPC
	{
		Vector2 SavePosition;
		Vector2 SavePlayerPosition;
		Vector2 SavePlayerPosition2;

		public int SaveDirection;

		public float SaveRotation;
		public float SpinMultiplier = 0f;

		public bool Transition = false;
		public bool Phase2 = false;
		public bool Phase3 = false;
		public bool Charging = false;
		public bool OpenMouth = false;
		public bool DontFacePlayer = false;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;

		public static readonly SoundStyle StunnedSound = new("Spooky/Content/Sounds/SpookFishron/FishronStunned", SoundType.Sound);
		
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 10;
			NPCID.Sets.TrailCacheLength[NPC.type] = 10;
			NPCID.Sets.TrailingMode[NPC.type] = 0;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Rotation = MathHelper.PiOver2,
				Position = new Vector2(60f, 0f),
				PortraitPositionXOverride = 10f,
				PortraitPositionYOverride = 0f
			};

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Venom] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(SavePosition);
			writer.WriteVector2(SavePlayerPosition);
			writer.WriteVector2(SavePlayerPosition2);

			//ints
			writer.Write(SaveDirection);

			//bools
			writer.Write(Transition);
			writer.Write(Phase2);
			writer.Write(Phase3);
			writer.Write(Charging);
			writer.Write(OpenMouth);
			writer.Write(DontFacePlayer);

			//floats
			writer.Write(SaveRotation);
			writer.Write(SpinMultiplier);
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			SavePosition = reader.ReadVector2();
			SavePlayerPosition = reader.ReadVector2();
			SavePlayerPosition2 = reader.ReadVector2();

			//ints
			SaveDirection = reader.ReadInt32();

			//bools
			Transition = reader.ReadBoolean();
			Phase2 = reader.ReadBoolean();
			Phase3 = reader.ReadBoolean();
			Charging = reader.ReadBoolean();
			OpenMouth = reader.ReadBoolean();
			DontFacePlayer = reader.ReadBoolean();

			//floats
			SaveRotation = reader.ReadSingle();
			SpinMultiplier = reader.ReadSingle();
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 65000;
			NPC.damage = 72;
			NPC.defense = 65;
			NPC.width = 150;
			NPC.height = 150;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 25, 0, 0);
			NPC.lavaImmune = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.boss = true;
			NPC.netAlways = true;
			NPC.HitSound = SoundID.NPCHit14;
			NPC.DeathSound = SoundID.NPCDeath20;
			NPC.aiStyle = -1;
			Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookFishron");
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SpookFishron"),
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.PumpkinMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookFishron_Background", Color.White)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookFishron/SpookFishronGlow");

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw aura
            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.OrangeRed, Color.Orange, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5f), 0).RotatedBy(MathHelper.ToRadians(i));

				Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.05f, effects, 0);
            }

			Color NpcColor = Phase2 ? NPC.GetAlpha(Color.Purple) : NPC.GetAlpha(Color.White);

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NpcColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			//draw aura
			if (Phase2)
			{
				Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			}

			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			//regular flying animation
			if (Charging && !OpenMouth)
			{
				NPC.frame.Y = frameHeight * 2;
			}
			else if (OpenMouth && !Charging)
			{
				if (NPC.frame.Y < frameHeight * 9)
				{
					NPC.frame.Y = frameHeight * 8;
				}

				NPC.frameCounter++;
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = frameHeight * 9;
				}
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 8)
				{
					NPC.frame.Y = frameHeight * 0;
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			if (!DontFacePlayer)
			{
				//EoC rotation
				Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
				float RotateX = player.Center.X - vector.X;
				float RotateY = player.Center.Y - vector.Y;
				NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
			}

			NPC.spriteDirection = NPC.direction;

			//set to phase transition if it drops below half health
			if (NPC.life < (NPC.lifeMax / 2) && !Phase2 && !Transition)
			{
				NPC.rotation = 0;
				DontFacePlayer = false;
				Charging = false;
				OpenMouth = false;
				NPC.localAI[0] = 0;
				NPC.localAI[1] = 0;
				NPC.localAI[2] = 0;
				NPC.localAI[3] = 0;
				NPC.ai[1] = 0;
				NPC.ai[0] = -1;
				Transition = true;
			}
			//expert mode phase transition
			if (Main.expertMode && NPC.life < (NPC.lifeMax / 10) && !Phase3 && !Transition)
			{
				NPC.rotation = 0;
				DontFacePlayer = false;
				Charging = false;
				OpenMouth = false;
				NPC.localAI[0] = 0;
				NPC.localAI[1] = 0;
				NPC.localAI[2] = 0;
				NPC.localAI[3] = 0;
				NPC.ai[1] = 0;
				NPC.ai[0] = -2;
				Transition = true;
			}

			//despawn if the player dies or its day time
			if (player.dead || Main.dayTime || NPC.Distance(player.Center) > 5600f)
			{
				NPC.ai[0] = -3;
			}

			//TODO: make fishron either immortal or enrage if you leave the ocean
			//also implement frost moon "EoL no hit" AI except it drops no unique rewards

			//while charging spawn dust the same way duke fishron does
			if (Charging)
			{
				int MaxDusts = 7;
				for (int j = 0; j < MaxDusts; j++)
				{
					Vector2 vector4 = (Vector2.Normalize(NPC.velocity) * new Vector2((float)(NPC.width + 50) / 2f, NPC.height) * 0.75f).RotatedBy((double)(j - (MaxDusts / 2 - 1)) * Math.PI / (double)(float)MaxDusts) + NPC.Center;
					Vector2 vector5 = ((float)(Main.rand.NextDouble() * 3.14) - (float)Math.PI / 2f).ToRotationVector2() * Main.rand.Next(3, 8);
					int num29 = Dust.NewDust(vector4 + vector5, 0, 0, 55, vector5.X * 2f, vector5.Y * 2f, 100, Color.Orange, 1f);
					Main.dust[num29].noGravity = true;
					Main.dust[num29].noLight = true;
					Main.dust[num29].velocity /= 4f;
					Main.dust[num29].velocity -= NPC.velocity;
				}
			}

			//attacks
			switch ((int)NPC.ai[0])
			{
				//despawning
				case -3:
				{
					NPC.velocity.Y -= 0.4f;
					NPC.EncourageDespawn(10);

					break;
				}

				//expert phase 3 transition
				case -2:
				{
					NPC.localAI[0]++;

					NPC.velocity *= 0.92f;

					if (NPC.localAI[0] == 1)
					{
						NPC.immortal = true;
						NPC.dontTakeDamage = true;
					}

					if (NPC.localAI[0] == 120)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

						OpenMouth = true;
						Phase3 = true;

						SpookyPlayer.ScreenShakeAmount = 18;

						for (float i = 5f; i <= 15f; i += 5f)
						{
							float maxAmount = 30;
							int currentAmount = 0;
							while (currentAmount <= maxAmount)
							{
								Vector2 velocity = new Vector2(i, i);
								Vector2 Bounds = new Vector2(3f, 3f);
								float intensity = i;

								Vector2 vector12 = Vector2.UnitX * 0f;
								vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
								vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
								int num104 = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
								Main.dust[num104].noGravity = true;
								Main.dust[num104].position = NPC.Center + vector12;
								Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
								currentAmount++;
							}
						}
					}

					if (NPC.localAI[0] >= 120 && NPC.alpha < 255)
					{
						NPC.alpha += 5;
					}

					if (NPC.localAI[0] == 200)
					{
						OpenMouth = false;
					}

					if (NPC.localAI[0] >= 250)
					{
						NPC.immortal = false;
						NPC.dontTakeDamage = false;

						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 7;

						Transition = false;

						NPC.netUpdate = true;
					}

					break;
				}

				//phase 2 transition
				case -1:
				{
					NPC.localAI[0]++;

					NPC.velocity *= 0.92f;

					if (NPC.localAI[0] == 1)
					{
						NPC.immortal = true;
						NPC.dontTakeDamage = true;
					}

					if (NPC.localAI[0] == 120)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

						OpenMouth = true;
						Phase2 = true;

						SpookyPlayer.ScreenShakeAmount = 18;

						for (float i = 5f; i <= 10f; i += 5f)
						{
							float maxAmount = 30;
							int currentAmount = 0;
							while (currentAmount <= maxAmount)
							{
								Vector2 velocity = new Vector2(i, i);
								Vector2 Bounds = new Vector2(3f, 3f);
								float intensity = i;

								Vector2 vector12 = Vector2.UnitX * 0f;
								vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
								vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
								int num104 = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
								Main.dust[num104].noGravity = true;
								Main.dust[num104].position = NPC.Center + vector12;
								Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
								currentAmount++;
							}
						}
					}

					if (NPC.localAI[0] == 200)
					{
						OpenMouth = false;
					}

					if (NPC.localAI[0] >= 250)
					{
						NPC.immortal = false;
						NPC.dontTakeDamage = false;

						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 1;

						Transition = false;

						NPC.netUpdate = true;
					}

					break;
				}

				//spawn intro, fly up and roar
				case 0:
				{
					NPC.localAI[0]++;

					//fly up quickly
					if (NPC.localAI[0] == 2)
					{
						NPC.velocity.Y = -35;
					}

					NPC.velocity *= 0.92f;

					if (NPC.localAI[0] < 35)
					{
						NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
						NPC.rotation += 0f * (float)NPC.spriteDirection;
					}

					if (NPC.localAI[0] == 60)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

						SpookyPlayer.ScreenShakeAmount = 18;
						OpenMouth = true;
					}

					if (NPC.localAI[0] >= 140)
					{
						OpenMouth = false;

						NPC.localAI[0] = 0;
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}
					
					break;
				}

				//fly to a location somewhere and charge, then attempt to spin towards the player, repeat 3 times
				case 1:
				{
					NPC.localAI[0]++;

					if (NPC.ai[1] < 2)
					{
						if (NPC.localAI[0] == 2)
						{
							NPC.localAI[1] = player.Center.X > NPC.Center.X ? Main.rand.Next(-550, -450) : Main.rand.Next(450, 550);
							NPC.localAI[2] = Main.rand.Next(-350, 150);
						}

						if (NPC.localAI[0] > 2 && NPC.localAI[0] < 150)
						{
							Vector2 GoTo = player.Center;
							GoTo.X += NPC.localAI[1];
							GoTo.Y += NPC.localAI[2];

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
						}

						if (NPC.localAI[0] > 60 && NPC.localAI[0] < 140)
						{
							OpenMouth = true;

							int Frequency = Phase2 ? 10 : 15;

							if (NPC.localAI[0] % Frequency == 0)
							{
								SoundEngine.PlaySound(SoundID.Item87, NPC.Center);

								Vector2 ShootSpeed = player.Center - NPC.Center;
								ShootSpeed.Normalize();
								ShootSpeed *= 15;

								Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
								Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y + 20);

								if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
								{
									position += Offset;
								}

								float SpreadX = Main.rand.NextFloat(-1.5f, 1.5f);
								float SpreadY = Main.rand.NextFloat(-1.5f, 1.5f);

								NPCGlobalHelper.ShootHostileProjectile(NPC, position, new Vector2(ShootSpeed.X + SpreadX, ShootSpeed.Y + SpreadY), ModContent.ProjectileType<SpookyBubble>(), NPC.damage, 4.5f);
							}
						}

						if (NPC.localAI[0] == 145)
						{
							SavePlayerPosition = player.Center;

							OpenMouth = false;
						}

						if (NPC.localAI[0] == 150)
						{
							SoundEngine.PlaySound(SoundID.Zombie9, NPC.Center);

							Charging = true;

							SaveDirection = NPC.spriteDirection;

							Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
							ChargeDirection.Normalize();
							ChargeDirection *= Phase2 ? 40f : 30f;
							NPC.velocity = ChargeDirection;
						}

						if (NPC.localAI[0] > 150)
						{
							NPC.direction = SaveDirection;
							NPC.spriteDirection = SaveDirection;

							NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
							NPC.rotation += 0f * (float)NPC.direction;
						}

						if (NPC.localAI[0] == 160)
						{
							SavePlayerPosition2 = player.Center;
						}

						//attempt to spin around the player
						if (NPC.localAI[0] > 165)
						{
							double angle = NPC.DirectionTo(SavePlayerPosition2).ToRotation() - NPC.velocity.ToRotation();
							while (angle > Math.PI)
							{
								angle -= 2.0 * Math.PI;
							}
							while (angle < -Math.PI)
							{
								angle += 2.0 * Math.PI;
							}

							float Angle = Math.Sign(angle);
							NPC.velocity = Vector2.Normalize(NPC.velocity) * 22; //(Phase2 ? 30 : 22);

							NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4.5f) * Angle);
						}

						if (NPC.localAI[0] >= 210)
						{
							Charging = false;

							NPC.localAI[0] = 0;
							NPC.ai[1]++;
							NPC.netUpdate = true;
						}
					}
					else
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.ai[1] = 0;
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}

					break;
				}

				//flame thrower attack, phase 1 shoot towards saved location, phase 2 shoot towards the player
				case 2:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] == 2)
					{
						NPC.localAI[1] = player.Center.X > NPC.Center.X ? Main.rand.Next(-500, -400) : Main.rand.Next(400, 500);
					}

					if (NPC.localAI[0] > 2 && NPC.localAI[0] < 60)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += NPC.localAI[1];
						GoTo.Y -= 200;

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}
					else
					{
						NPC.velocity *= 0.95f;
					}

					//save npc center
					if (NPC.localAI[0] == 85)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -1.2f }, NPC.Center);

						SavePosition = NPC.Center;

						NPC.netUpdate = true;
					}

					//shake before firing projectiles
					if (NPC.localAI[0] > 85 && NPC.localAI[0] < 240)
					{
						NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
						NPC.Center += Main.rand.NextVector2Square(-7, 7);
					}

					if (NPC.localAI[0] == 110)
					{
						SavePlayerPosition = player.Center;
						SaveDirection = NPC.spriteDirection;
					}

					//rotate towards the saved player position in phase 1
					if (!Phase2 && NPC.localAI[0] >= 120 && NPC.localAI[0] <= 300)
					{
						NPC.direction = SaveDirection;
						NPC.spriteDirection = SaveDirection;

						Vector2 NewNector = new Vector2(NPC.Center.X, NPC.Center.Y);
						float NewRotateX = SavePlayerPosition.X - NewNector.X;
						float NewRotateY = SavePlayerPosition.Y - NewNector.Y;
						NPC.rotation = (float)Math.Atan2((double)NewRotateY, (double)NewRotateX) + 4.71f;
					}

					if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 240)
					{
						OpenMouth = true;

						Vector2 ShootSpeed = Phase2 ? player.oldPosition - NPC.Center : SavePlayerPosition - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 15;

						Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
						Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y + 20);

						if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
						{
							position += Offset;
						}

						if (NPC.localAI[0] % 10 == 0)
						{
							SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
						}

						if (NPC.localAI[0] % 2 == 0)
						{
							for (int numProjs = 0; numProjs <= 2; numProjs++)
							{
								float SpreadX = Phase2 ? 0 : Main.rand.Next(-1, 2);
								float SpreadY = Phase2 ? 0 : Main.rand.Next(-1, 2);

								NPCGlobalHelper.ShootHostileProjectile(NPC, position, new Vector2(ShootSpeed.X + SpreadX, ShootSpeed.Y + SpreadY), ModContent.ProjectileType<SpookyFlames>(), NPC.damage, 4.5f);
							}
						}
					}

					if (NPC.localAI[0] > 240)
					{
						OpenMouth = false;
					}   

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}

					break;
				}

				//charge again
				case 3:
				{
					goto case 1;
				}

				//create spooky sharkron tornado
				case 4:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] == 2)
					{
						NPC.localAI[1] = player.Center.X > NPC.Center.X ? Main.rand.Next(-450, -400) : Main.rand.Next(400, 450);
						NPC.localAI[2] = Main.rand.Next(-400, -200);
					}

					if (NPC.localAI[0] > 2)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += NPC.localAI[1];
						GoTo.Y += NPC.localAI[2];

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 135)
					{
						NPC.velocity *= 0.95f;
					}

					if (NPC.localAI[0] == 60)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

						SpookyPlayer.ScreenShakeAmount = 18;
						OpenMouth = true;
					}

					if (NPC.localAI[0] == 120)
					{
						OpenMouth = false;

						Vector2 ShootSpeed = player.Center - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 5f;

						Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 62f;
						Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y + 20);

						if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
						{
							position += Offset;
						}

						//tornado in phase 1
						if (!Phase2)
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<SpookyTornadoSpawner>(), NPC.damage, 4.5f);
						}
						//tornado bolt in phase 2 becomes fully homing
						else
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<SpookyTornadoSpawner>(), NPC.damage, 4.5f, 1);
						}
					}

					if (NPC.localAI[0] >= 320)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.localAI[3] = 0;
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}

					break;
				}

				//charge again
				case 5:
				{
					goto case 1;
				}

				//stake launcher in phase 1, horsemans blade in phase 2
				case 6:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] == 2)
					{
						NPC.localAI[1] = player.Center.X > NPC.Center.X ? Main.rand.Next(-550, -450) : Main.rand.Next(450, 550);
					}

					if (NPC.localAI[0] > 2 && NPC.localAI[0] < 60)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += NPC.localAI[1];

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] == 60)
					{
						SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

						SpookyPlayer.ScreenShakeAmount = 12;
						OpenMouth = true;
					}

					if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 175)
					{
						NPC.velocity *= 0.95f;
					}

					//stake launcher
					if (!Phase2)
					{
						//spawn stake launcher
						if (NPC.localAI[0] == 120)
						{
							OpenMouth = false;

							Vector2 pos = new Vector2(1000, 0).RotatedBy(NPC.rotation + MathHelper.PiOver2);
							NPCGlobalHelper.ShootHostileProjectile(NPC, pos + player.Center, Vector2.Zero, ModContent.ProjectileType<StakeLauncherSpin>(), NPC.damage, 4.5f, 0, NPC.whoAmI);
						}

						//repeat stake launcher attack 4 times
						if (NPC.ai[1] < 6)
						{
							//charge and use the stake launcher, 3 times
							if (NPC.ai[1] <= 4)
							{
								Vector2 VelocityPredictCenter = player.Center + player.velocity * 20f;

								//go to position around the player
								if (NPC.localAI[0] == 240)
								{
									NPC.localAI[1] = player.Center.X > NPC.Center.X ? Main.rand.Next(-550, -450) : Main.rand.Next(450, 550);
									NPC.localAI[2] = Main.rand.Next(-350, 0);
								}
								if (NPC.localAI[0] > 240 && NPC.localAI[0] < 300)
								{
									//rotate towards predicted position
									Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
									float RotateX = VelocityPredictCenter.X - vector.X;
									float RotateY = VelocityPredictCenter.Y - vector.Y;
									NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

									Vector2 GoTo = player.Center;
									GoTo.X += NPC.localAI[1];
									GoTo.Y += NPC.localAI[2];

									float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
									NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
								}

								if (NPC.localAI[0] == 300)
								{
									SavePlayerPosition = VelocityPredictCenter;
									SaveDirection = NPC.spriteDirection;
								}

								if (NPC.localAI[0] >= 300)
								{
									NPC.velocity = Vector2.Zero;

									NPC.direction = SaveDirection;
									NPC.spriteDirection = SaveDirection;

									//rotate towards the saved predicted position until the attack loops
									Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
									float RotateX = SavePlayerPosition.X - vector.X;
									float RotateY = SavePlayerPosition.Y - vector.Y;
									NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
								}

								if (NPC.localAI[0] > 320)
								{
									NPC.localAI[0] = 238;
									NPC.localAI[1] = 0;
									NPC.ai[1]++;
									NPC.netUpdate = true;
								}
							}
							//spin and launch the stake launcher on the 6th attack
							else
							{
								if (NPC.localAI[0] == 240)
								{
									NPC.localAI[1] = player.Center.X > NPC.Center.X ? -660 : 660;
								}

								if (NPC.localAI[0] > 240 && NPC.localAI[0] < 450)
								{
									Vector2 GoTo = player.Center;
									GoTo.X += NPC.localAI[1];

									float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
									NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
								}

								if (NPC.localAI[0] == 300)
								{
									DontFacePlayer = true;
								}
								
								//spin speed acceleration
								if (NPC.localAI[0] > 300 && NPC.localAI[0] < 450)
								{
									SpinMultiplier += 0.005f;

									SaveDirection = NPC.spriteDirection;

									NPC.rotation += (1f * SpinMultiplier) * (float)SaveDirection;
								}

								if (NPC.localAI[0] >= 450)
								{
									NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f * SaveDirection;

									NPC.direction = SaveDirection;
									NPC.spriteDirection = SaveDirection;

									NPC.velocity *= 0.98f;

									if (NPC.localAI[0] == 470)
									{
										SoundEngine.PlaySound(StunnedSound, NPC.Center);
									}

									//stunned behavior, goofy tweety bird sound and star dusts
									if (NPC.localAI[0] % 5 == 0)
									{						
										int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 1.2f);
										Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
										Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
										Main.dust[newDust].velocity.Y = Main.rand.Next(-10, -4);
										Main.dust[newDust].noGravity = true;
									}
								}

								if (NPC.localAI[0] > 650)
								{
									Charging = false;
									DontFacePlayer = false;

									SpinMultiplier = 0f;

									NPC.localAI[0] = 0;
									NPC.localAI[1] = 0;
									NPC.ai[1]++;
									NPC.netUpdate = true;
								}
							}
						}
						else
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.ai[1] = 0;
							NPC.ai[0] = 1;
							NPC.netUpdate = true;
						}
					}
					//horsemans blade attack
					else
					{
						//spawn sword
						if (NPC.localAI[0] == 120)
						{
							OpenMouth = false;

							Vector2 pos = new Vector2(1000, 0).RotatedBy(NPC.rotation + MathHelper.PiOver2);
							NPCGlobalHelper.ShootHostileProjectile(NPC, pos + player.Center, Vector2.Zero, ModContent.ProjectileType<SpookySwordSpin>(), NPC.damage, 4.5f, 0, NPC.whoAmI);
						}

						//repeat sword attack 4 times
						if (NPC.ai[1] < 4)
						{
							//charge and use the sword to shoot out pumpkin heads, 3 times
							if (NPC.ai[1] <= 2)
							{
								//manually set rotation to just be left and right
								if (NPC.localAI[0] >= 238)
								{
									NPC.rotation = NPC.spriteDirection == -1 ? MathHelper.PiOver2 : -MathHelper.PiOver2;
								}

								if (NPC.localAI[0] == 240)
								{
									NPC.localAI[1] = player.Center.X > NPC.Center.X ? -600 : 600;
								}

								if (NPC.localAI[0] > 240 && NPC.localAI[0] < 300)
								{
									Vector2 GoTo = player.Center;
									GoTo.X += NPC.localAI[1];

									float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
									NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
								}

								if (NPC.localAI[0] == 300)
								{
									SavePlayerPosition = player.Center;
								}

								if (NPC.localAI[0] > 300 && NPC.localAI[0] < 310)
								{
									NPC.velocity *= 0.95f;
								}

								if (NPC.localAI[0] == 310)
								{
									SoundEngine.PlaySound(SoundID.Zombie9, NPC.Center);

									Charging = true;

									SaveDirection = NPC.spriteDirection;

									Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
									ChargeDirection.Normalize();
									ChargeDirection.X *= 55f;
									ChargeDirection.Y *= 0f;
									NPC.velocity = ChargeDirection;
								}

								if (NPC.localAI[0] > 310)
								{
									NPC.direction = SaveDirection;
									NPC.spriteDirection = SaveDirection;

									NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
									NPC.rotation += 0f * (float)NPC.direction;
								}

								if (NPC.localAI[0] > 325)
								{
									Charging = false;

									NPC.velocity *= 0.85f;
								}

								if (NPC.localAI[0] > 375)
								{
									NPC.localAI[0] = 238;
									NPC.localAI[1] = 0;
									NPC.ai[1]++;
									NPC.netUpdate = true;
								}
							}
							//spin and launch the sword on the 4th attack
							else
							{
								//manually set rotation to just be left and right
								if (NPC.localAI[0] >= 238 && NPC.localAI[0] < 300)
								{
									NPC.rotation = NPC.spriteDirection == -1 ? MathHelper.PiOver2 : -MathHelper.PiOver2;
								}

								if (NPC.localAI[0] == 240)
								{
									NPC.localAI[1] = player.Center.X > NPC.Center.X ? -660 : 660;
								}

								if (NPC.localAI[0] > 240 && NPC.localAI[0] < 450)
								{
									Vector2 GoTo = player.Center;
									GoTo.X += NPC.localAI[1];

									float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
									NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
								}

								if (NPC.localAI[0] == 300)
								{
									DontFacePlayer = true;
								}
								
								//spin speed acceleration
								if (NPC.localAI[0] > 300 && NPC.localAI[0] < 450)
								{
									SpinMultiplier += 0.005f;

									SaveDirection = NPC.spriteDirection;

									NPC.rotation += (1f * SpinMultiplier) * (float)SaveDirection;
								}

								if (NPC.localAI[0] >= 450)
								{
									NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.01f * SaveDirection;

									NPC.direction = SaveDirection;
									NPC.spriteDirection = SaveDirection;

									NPC.velocity *= 0.98f;

									if (NPC.localAI[0] == 470)
									{
										SoundEngine.PlaySound(StunnedSound, NPC.Center);
									}

									//stunned behavior, goofy tweety bird sound and star dusts
									if (NPC.localAI[0] % 5 == 0)
									{						
										int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 1.2f);
										Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
										Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
										Main.dust[newDust].velocity.Y = Main.rand.Next(-10, -4);
										Main.dust[newDust].noGravity = true;
									}
								}

								if (NPC.localAI[0] > 650)
								{
									Charging = false;
									DontFacePlayer = false;

									SpinMultiplier = 0f;

									NPC.localAI[0] = 0;
									NPC.localAI[1] = 0;
									NPC.ai[1]++;
									NPC.netUpdate = true;
								}
							}
						}
						else
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.ai[1] = 0;
							NPC.ai[0] = 1;
							NPC.netUpdate = true;
						}
					}

					break;
				}

				//expert mode phase, screen goes black (handled in sky) and goes invisible with only glowmask showing
				//teleport around the player and charge, becoming visible and fading out in a similar manner to duke fishron in expert mode
				//after 5 charges, spawn a spooky sharkron tornado vortex and then repeat constantly
				case 7:
				{
					NPC.localAI[0]++;

					//teleport to a position and charge
					if (NPC.ai[1] < 4)
					{
						if (NPC.localAI[0] == 2)
						{
							NPC.localAI[1] = Main.rand.NextBool() ? -450 : 450;
							NPC.localAI[2] = Main.rand.Next(0, 2);
						}

						if (NPC.localAI[0] == 30)
						{
							Charging = false;

							float GoToX = NPC.localAI[2] == 0 ? NPC.localAI[1] : 0;
							float GoToY = NPC.localAI[2] == 0 ? 0 : NPC.localAI[1];

							NPC.position = player.Center + new Vector2(GoToX, GoToY) - (NPC.frame.Size() / 2);
							NPC.velocity = Vector2.Zero;

							float maxAmount = 30;
							int currentAmount = 0;
							while (currentAmount <= maxAmount)
							{
								Vector2 velocity = new Vector2(5f, 5f);
								Vector2 Bounds = new Vector2(3f, 3f);
								float intensity = 5f;

								Vector2 vector12 = Vector2.UnitX * 0f;
								vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
								vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
								int num104 = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
								Main.dust[num104].noGravity = true;
								Main.dust[num104].position = NPC.Center + vector12;
								Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
								currentAmount++;
							}
						}

						if (NPC.localAI[0] > 30 && NPC.localAI[0] < 75)
						{
							Vector2 GoTo = player.Center;
							GoTo.X += NPC.localAI[2] == 0 ? NPC.localAI[1] : 0;
							GoTo.Y += NPC.localAI[2] == 0 ? 0 : NPC.localAI[1];

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
						}

						if (NPC.localAI[0] == 65)
						{
							SavePlayerPosition = player.Center;
						}

						//fade in
						if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 75)
						{
							if (NPC.alpha > 0)
							{
								NPC.alpha -= 50;
							}
						}

						if (NPC.localAI[0] == 75)
						{
							SoundEngine.PlaySound(SoundID.Zombie9, NPC.Center);

							Charging = true;

							SaveDirection = NPC.spriteDirection;

							Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
							ChargeDirection.Normalize();
							ChargeDirection *= 40f;
							NPC.velocity = ChargeDirection;
						}

						if (NPC.localAI[0] == 85)
						{
							SavePlayerPosition2 = player.Center;
						}

						if (Charging)
						{
							NPC.direction = SaveDirection;
							NPC.spriteDirection = SaveDirection;

							NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;
							NPC.rotation += 0f * (float)NPC.direction;
						}

						//fade out
						if (NPC.localAI[0] > 90)
						{
							if (NPC.alpha < 255)
							{
								NPC.alpha += 20;
							}
						}

						if (NPC.localAI[0] > 90)
						{
							double angle = NPC.DirectionTo(SavePlayerPosition2).ToRotation() - NPC.velocity.ToRotation();
							while (angle > Math.PI)
							{
								angle -= 2.0 * Math.PI;
							}
							while (angle < -Math.PI)
							{
								angle += 2.0 * Math.PI;
							}

							float Angle = Math.Sign(angle);
							NPC.velocity = Vector2.Normalize(NPC.velocity) * 38;

							NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4.5f) * Angle);
						}

						if (NPC.localAI[0] >= 115)
						{
							NPC.localAI[0] = 0;
							NPC.ai[1]++;
							NPC.netUpdate = true;
						}
					}
					//fire tornado
					else
					{
						NPC.localAI[0]++;

						if (NPC.localAI[0] == 2)
						{
							Charging = false;

							NPC.localAI[1] = Main.rand.NextBool() ? -500 : 500;
						}

						if (NPC.localAI[0] == 30)
						{
							NPC.position = player.Center + new Vector2(NPC.localAI[1], NPC.localAI[2]) - (NPC.frame.Size() / 2);
							NPC.velocity = Vector2.Zero;

							for (float i = 5f; i <= 10f; i += 5f)
							{
								float maxAmount = 30;
								int currentAmount = 0;
								while (currentAmount <= maxAmount)
								{
									Vector2 velocity = new Vector2(i, i);
									Vector2 Bounds = new Vector2(3f, 3f);
									float intensity = i;

									Vector2 vector12 = Vector2.UnitX * 0f;
									vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
									vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
									int num104 = Dust.NewDust(NPC.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
									Main.dust[num104].noGravity = true;
									Main.dust[num104].position = NPC.Center + vector12;
									Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
									currentAmount++;
								}
							}
						}

						//fade in
						if (NPC.localAI[0] > 30 && NPC.localAI[0] <= 180)
						{
							if (NPC.alpha > 0)
							{
								NPC.alpha -= 50;
							}
						}

						if (NPC.localAI[0] > 30)
						{
							Vector2 GoTo = player.Center;
							GoTo.X += NPC.localAI[1];
							GoTo.Y += NPC.localAI[2];

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
						}

						if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 120)
						{
							NPC.velocity *= 0.95f;
						}

						if (NPC.localAI[0] == 60)
						{
							SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = -2.2f }, NPC.Center);

							SpookyPlayer.ScreenShakeAmount = 18;
							OpenMouth = true;
						}

						if (NPC.localAI[0] == 120)
						{
							OpenMouth = false;

							Vector2 ShootSpeed = player.Center - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= 5f;

							Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 62f;
							Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y + 20);

							if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
							{
								position += Offset;
							}

							NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<SpookyTornadoSpawner>(), NPC.damage, 4.5f, 1);
						}

						if (NPC.localAI[0] > 380)
						{
							//fade out
							if (NPC.alpha < 255)
							{
								NPC.alpha += 20;
							}
						}

						if (NPC.localAI[0] >= 420)
						{
							NPC.localAI[0] = 0;
							NPC.ai[1] = 0;
							NPC.netUpdate = true;
						}
					}

					break;
				}
			}
		}

		//Loot and stuff
		public override void ModifyNPCLoot(NPCLoot npcLoot) 
		{
			LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

			//treasure bag
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagSpookFishron>()));

			//master relic and pet
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SpookFishronRelicItem>()));
			npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SinisterShell>(), 4));

			//weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<SpookFishronFlail>(), 
                ModContent.ItemType<SpookFishronBow>(), 
                ModContent.ItemType<SpookFishronTome>(), 
                ModContent.ItemType<SpookFishronGun>(),
                ModContent.ItemType<SpookFishronStaff>()
            };

			notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

			//drop boss mask
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpookFishronMask>(), 7));

			//trophy always drops directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpookFishronTrophyItem>(), 10));

			npcLoot.Add(notExpertRule);
		}

		public override void OnKill()
		{
			if (!Flags.downedSpookFishron)
			{
				Flags.GuaranteedRaveyard = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			NPC.SetEventFlagCleared(ref Flags.downedSpookFishron, -1);
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.GreaterHealingPotion;
		}
	}
}