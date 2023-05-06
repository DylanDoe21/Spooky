using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.NPCs.Boss.RotGourd.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.RotGourd
{
	[AutoloadBossHead]
	public class RotGourd : ModNPC
	{
		Vector2 SavePlayerPosition;

		public bool FirstFlySpawned = false;
		public bool SecondFlySpawned = false;
		public bool ThirdFlySpawned = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/RotGourd/RotGourdBC",
                Position = new Vector2(20f, 20f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(FirstFlySpawned);
            writer.Write(SecondFlySpawned);
            writer.Write(ThirdFlySpawned);

			//local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//bools
            FirstFlySpawned = reader.ReadBoolean();
            SecondFlySpawned = reader.ReadBoolean();
            ThirdFlySpawned = reader.ReadBoolean();

			//local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 3000;
            NPC.damage = 18;
            NPC.defense = 5;
            NPC.width = 78;
            NPC.height = 134;
            NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/RotGourd");
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.RotGourd"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
			Texture2D moldTex1 = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/RotGourd/RotGourdMold1").Value;
			Texture2D moldTex2 = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/RotGourd/RotGourdMold2").Value;
			Texture2D moldTex3 = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/RotGourd/RotGourdMold3").Value;

			float stretch = NPC.velocity.Y * 0.008f;

			stretch = Math.Abs(stretch);

			//limit how much he can stretch
			if (stretch > 0.3f)
			{
				stretch = 0.3f;
			}

			//limit how much he can squish
			if (stretch < -0.15f)
			{
				stretch = -0.15f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			
			if (NPC.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}
			if (NPC.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(tex, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			if (NPC.life > (NPC.lifeMax / 1.25f))
			{
				spriteBatch.Draw(moldTex1, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
				NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);
			}

			if (NPC.life > (NPC.lifeMax / 2))
			{
				spriteBatch.Draw(moldTex2, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
				NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);
			}

			if (NPC.life > (NPC.lifeMax / 3))
			{
				spriteBatch.Draw(moldTex3, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
				NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);
			}

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
			if (NPC.velocity.Y <= 0)
			{
				NPC.frame.Y = frameHeight * 0;
			}
            if (NPC.velocity.Y > 0 || (NPC.ai[0] == 1 && NPC.localAI[0] >= 60 && NPC.localAI[0] <= 120))
            {
                NPC.frame.Y = frameHeight * 1;
            }
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 35 / 3 : Main.expertMode ? 25 / 2 : 20;

			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * 0.02f;

			if (NPC.life < (NPC.lifeMax / 1.25f) && !FirstFlySpawned)
			{
				NPC.ai[1] = 0;
				FirstFlySpawned = true;
				NPC.netUpdate = true;
			}

			if (NPC.life < (NPC.lifeMax / 2) && !SecondFlySpawned)
			{
				NPC.ai[1] = 0;
				SecondFlySpawned = true;
				NPC.netUpdate = true;
			}

			if (NPC.life < (NPC.lifeMax / 3) && !ThirdFlySpawned)
			{
				NPC.localAI[0] = 0;
				NPC.localAI[1] = 0;
				NPC.localAI[2] = 0;
				NPC.ai[0] = 6;
				NPC.ai[1] = 0;
				NPC.noGravity = false;
				NPC.noTileCollide = false;
				ThirdFlySpawned = true;
				NPC.netUpdate = true;
			}

			//spawn swarm of flies when spawned
            if (NPC.ai[1] <= 0)
            {
				int maxFlies = ThirdFlySpawned ? 20 : 12;

                for (int numFlies = 0; numFlies < maxFlies; numFlies++)
                {
                    Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57f) * new Vector2(5f, 3f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, vector.X, vector.Y, 
                        ModContent.ProjectileType<RotFly>(), Damage, 0f, NPC.target, 0f, (float)NPC.whoAmI);
					}
				}

                NPC.netUpdate = true;

                NPC.ai[1] = 1;
            }

			//despawn if all players are dead
            if (player.dead)
            {
                NPC.ai[2]++;

				//play sound
				if (NPC.ai[2] == 60)
				{
					NPC.noTileCollide = false;
					SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);
				}

				//jump up super high, then despawn
                if (NPC.ai[2] >= 60)
                {
                    NPC.velocity.Y = -40;
					NPC.EncourageDespawn(10);
                }
            }
			//use normal attacks
			else
			{
				//attacks
				switch ((int)NPC.ai[0])
				{
					//Jump 3 times towards the player
					case 0:
					{
						NPC.localAI[0]++;
									
						if (NPC.localAI[1] < 3)
						{
							//jumping velocity
							Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 500);

							Vector2 velocity = JumpTo - NPC.Center;

							//actual jumping
							if (NPC.localAI[0] == 60)
							{
								SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

								float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 20);
								velocity.Normalize();
								velocity.Y -= 0.18f;
								velocity.X *= 1.2f;
								NPC.velocity = velocity * speed * 1.1f;
							}

							//set no tile collide to true so he can jump through blocks
							if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 115)
							{
								NPC.noTileCollide = true;
							}

							//slam down instantly if above the player
							if (NPC.localAI[0] > 85 && NPC.localAI[0] < 110 && NPC.position.X <= player.Center.X + 10 && NPC.Center.X >= player.Center.X - 10)
							{
								NPC.localAI[0] = 115;
							}

							//charge down
							if (NPC.localAI[0] == 115)
							{
								NPC.noGravity = true;

								NPC.velocity.X *= 0;
								NPC.velocity.Y = Main.expertMode ? 16 : 13;
							}

							//set tile collide to true after jumping so you cant avoid them
							if (NPC.localAI[0] >= 115)
							{	
								if (NPC.position.Y >= player.Center.Y - 200)
								{
									NPC.noTileCollide = false;
								}
							}

							//slam the ground
							if (NPC.localAI[0] >= 115 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
							{
								NPC.noGravity = false;

								NPC.velocity.X *= 0;

								SpookyPlayer.ScreenShakeAmount = 5;

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

								//push all nearby players in the air if they are on the ground
								for (int i = 0; i < Main.maxPlayers; i++)
								{
									if (Main.player[i].active && Main.player[i].velocity.Y == 0 && NPC.Distance(Main.player[i].Center) <= 500f)
									{
										Main.player[i].velocity.Y -= 8f;
									}
								}

								//make cool dust effect when slamming the ground
								for (int i = 0; i < 45; i++)
								{                                                                                  
									int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
									Main.dust[slamDust].noGravity = true;
									Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
									Main.dust[slamDust].position.Y += 104;
									Main.dust[slamDust].scale = 3f;
									
									if (Main.dust[slamDust].position != NPC.Center)
									{
										Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
									}
								}
								
								//complete the slam attack
								NPC.localAI[2] = 1; 
							}

							//only loop attack if the jump has been completed
							if (NPC.localAI[0] >= 140 && NPC.localAI[2] > 0)
							{
								NPC.localAI[0] = 0;
								NPC.localAI[2] = 0;
								NPC.localAI[1]++;
								NPC.netUpdate = true;
							}
						}
						else
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							
							//set to mold spore attack when below 75% hp, otherwise go to big slam
							if (NPC.life <= (NPC.lifeMax / 1.25f))
							{
								NPC.ai[0]++;
							}
							else
							{
								NPC.ai[0] = 2;
							}

							NPC.netUpdate = true;
						}

						break;
					}

					//jerry spits lingering mold spores, only use when below 75% hp
					case 1:
					{
						NPC.localAI[0]++;

						if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 120)
						{
							Vector2 ShootSpeed = player.Center - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed.X *= Main.rand.NextFloat(2f, 4f);
							ShootSpeed.Y *= Main.rand.NextFloat(2f, 4f);

							if (Main.rand.NextBool(4))
							{
								SoundEngine.PlaySound(SoundID.NPCDeath9, NPC.Center);

								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y - 10, ShootSpeed.X + Main.rand.Next(-3, 3), 
								ShootSpeed.Y + Main.rand.Next(-3, 3), ModContent.ProjectileType<MoldSpore>(), Damage, 1, NPC.target, 0, 0);
							}
						}

						if (NPC.localAI[0] >= 240)
						{
							NPC.localAI[0] = 0;
							NPC.ai[0]++;
							NPC.netUpdate = true;
						}

						break;
					}

					//jump up really high, then slam back down above the player
					case 2:
					{
						NPC.localAI[0]++;

						if (NPC.localAI[0] == 20)
						{
							NPC.noTileCollide = true;

							SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);
						}
									
						if (NPC.localAI[0] >= 20 && NPC.localAI[0] <= 50)
						{
							NPC.velocity.Y = -40;
						}

						if (NPC.localAI[0] == 70)
						{
							NPC.position.X = player.Center.X - 40;
							NPC.position.Y = player.Center.Y - 1200;

							for (int k = 0; k < Main.maxProjectiles; k++)
							{
								if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
								{
									Main.projectile[k].Center = NPC.Center;
								}
							}

							NPC.velocity *= 0;
						}

						if (NPC.localAI[0] >= 75 && NPC.localAI[0] <= 80)
						{
							NPC.noGravity = true;

							NPC.velocity.X *= 0;
							NPC.velocity.Y = 32;
						}

						//set tile collide to true after jumping
						if (NPC.localAI[0] >= 75)
						{
							if (NPC.position.Y >= player.Center.Y - 150)
							{
								NPC.noTileCollide = false;
							}
						}

						//slam the ground
						if (NPC.localAI[0] >= 75 && NPC.localAI[1] == 0 && NPC.velocity.Y <= 0.1f)
						{
							NPC.noGravity = false;

							SpookyPlayer.ScreenShakeAmount = 10;

							SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

							//push all nearby players in the air if they are on the ground
							for (int i = 0; i < Main.maxPlayers; i++)
							{
								if (Main.player[i].active && Main.player[i].velocity.Y == 0 && NPC.Distance(Main.player[i].Center) <= 500f)
								{
									Main.player[i].velocity.Y -= 12f;
								}
							}

							//make cool dust effect when slamming the ground
							for (int i = 0; i < 65; i++)
							{                                                                                  
								int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
								Main.dust[slamDust].noGravity = true;
								Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
								Main.dust[slamDust].position.Y += 104;
								Main.dust[slamDust].scale = 3f;
								
								if (Main.dust[slamDust].position != NPC.Center)
								{
									Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
								}
							}
							
							//complete the slam attack
							NPC.localAI[1] = 1; 
						}

						if (NPC.localAI[0] > 80 && NPC.localAI[1] > 0)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.ai[0]++;
							NPC.netUpdate = true;
						}

						break;
					}

					//jump across the player, then release flies mid air and then fall and slow down
                    case 3:
                    {
                        NPC.localAI[0]++;

                        Vector2 JumpTo = new Vector2(player.Center.X + (NPC.Center.X > player.Center.X ? -400 : 400), player.Center.Y - 1000);
						Vector2 velocity = JumpTo - NPC.Center;

                        //actual jumping
                        if (NPC.localAI[0] == 80)
                        {
							SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                            float speed = MathHelper.Clamp(velocity.Length() / 36, 6, 18);
                            velocity.Normalize();
                            velocity.Y -= 0.25f;
                            velocity.X *= 1.2f;
                            NPC.velocity = velocity * speed * 1.1f;
                        }

						//change fly ai to charging
						for (int k = 0; k < Main.maxProjectiles; k++)
						{
							if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
							{
								if ((NPC.localAI[0] >= 90 && NPC.localAI[0] < 160 && Main.rand.NextBool(50)) || NPC.localAI[0] == 160)
								{
									Main.projectile[k].ai[0] = 1;
								}
							}
						}

						//set no tile collide to true so he can jump through blocks
                        if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 160)
                        {
                            NPC.noTileCollide = true;
                        }

                        if (NPC.localAI[0] > 160)
                        {
                            NPC.noTileCollide = false;
                            NPC.velocity.X *= 0.95f;

							if (NPC.velocity.Y == 0)
							{
								NPC.velocity.X *= 0.5f;
							}
                        }

                        if (NPC.localAI[0] >= 250)
                        {
                            for (int k = 0; k < Main.maxProjectiles; k++)
							{
								if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
								{
									Main.projectile[k].localAI[0] = 0;
									Main.projectile[k].ai[0] = 0;
								}
							}

							NPC.localAI[0] = 0;

							//set to mega slam attack when below 50% hp,other wise reset attack pattern
							if (NPC.life <= ((NPC.lifeMax / 2)))
							{
								NPC.ai[0]++;
							}
							else
							{
								NPC.ai[0] = 0;
							}

							NPC.netUpdate = true;
                        }

                        break;
                    }

					//just use normal jump attack after flies
					case 4:
					{
						goto case 0;
					}

					//jump in place, sending multiple spreads of dirt debris everywhere
					case 5:
					{
						NPC.localAI[0]++;

						if (NPC.localAI[1] < 3)
						{
							if (NPC.localAI[0] == 2)
							{
								SavePlayerPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 250);
							}

							if (NPC.localAI[0] == 60)
							{
								NPC.noTileCollide = true;

								NPC.velocity.Y = -40;
							}

							if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 90)
							{
								NPC.noGravity = true;

								NPC.velocity.X *= 0;
								NPC.velocity.Y = 32;
							}

							if (NPC.localAI[0] >= 90)
							{
								if (NPC.position.Y >= SavePlayerPosition.Y)
								{
									NPC.noTileCollide = false;
								}
							}

							//slam the ground
							if (NPC.localAI[0] >= 90 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
							{
								NPC.noGravity = false;

								SpookyPlayer.ScreenShakeAmount = 10;

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

								int NumProjectiles = Main.rand.Next(10, 15);
								for (int i = 0; i < NumProjectiles; i++)
								{
									float Spread = Main.rand.Next(-2500, 2500) * 0.01f;

									if (Main.netMode != NetmodeID.MultiplayerClient)
									{
										Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20, 0 + Spread, Main.rand.Next(-18, -13), 
										ModContent.ProjectileType<DirtDebris>(), Damage, 1, NPC.target, 0, 0);
									}
								}

								//make cool dust effect when slamming the ground
								for (int i = 0; i < 65; i++)
								{                                                                                  
									int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
									Main.dust[slamDust].noGravity = true;
									Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
									Main.dust[slamDust].position.Y += 104;
									Main.dust[slamDust].scale = 3f;
									
									if (Main.dust[slamDust].position != NPC.Center)
									{
										Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
									}
								}
								
								//complete the slam attack
								NPC.localAI[2] = 1; 
							}

							if (NPC.localAI[2] >= 1)
							{
								NPC.localAI[0] = 50;
								NPC.localAI[2] = 0;
								NPC.localAI[1]++;
							}
						}
						else
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.ai[0] = 0;
						}

						break;
					}

					//get flies to carry him, drop him on you
					case 6:
					{
						NPC.localAI[0]++;

						if (NPC.localAI[1] < 3)
						{
							if (NPC.localAI[0] >= 65 && NPC.localAI[0] < 140)
							{
								NPC.noTileCollide = true;

								NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);	
								Vector2 GoTo = player.Center;
								NPC.spriteDirection = NPC.direction;
								GoTo.X += 0f;
								GoTo.Y -= 500;

								float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 8, 16);
								NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
							}

							if (NPC.localAI[0] == 140)
							{
								NPC.velocity.X *= 0;
							}

							//attempt to crush the player
							if (NPC.localAI[0] == 150)
							{
								SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);
								
								NPC.noGravity = true;

								for (int k = 0; k < Main.maxProjectiles; k++)
								{
									if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
									{
										Main.projectile[k].ai[0] = 2;
									}
								}

								NPC.velocity.X *= 0;
								NPC.velocity.Y = 30;
							}

							//set tile collide to true after jumping so you cant avoid them
							if (NPC.localAI[0] >= 150)
							{	
								if (NPC.position.Y >= player.Center.Y - 150)
								{
									NPC.noTileCollide = false;
								}
							}

							//slam the ground
							if (NPC.localAI[0] >= 150 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
							{
								NPC.noGravity = false;

								NPC.velocity.X *= 0;

								SpookyPlayer.ScreenShakeAmount = 5;

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

								//push all nearby players in the air if they are on the ground
								for (int i = 0; i < Main.maxPlayers; i++)
								{
									if (Main.player[i].active && Main.player[i].velocity.Y == 0 && NPC.Distance(Main.player[i].Center) <= 500f)
									{
										Main.player[i].velocity.Y -= 8f;
									}
								}

								//make cool dust effect when slamming the ground
								for (int i = 0; i < 45; i++)
								{                                                                                  
									int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
									Main.dust[slamDust].noGravity = true;
									Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
									Main.dust[slamDust].position.Y += 104;
									Main.dust[slamDust].scale = 3f;
									
									if (Main.dust[slamDust].position != NPC.Center)
									{
										Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
									}
								}
								
								//complete the slam attack
								NPC.localAI[2] = 1;
							}

							if (NPC.localAI[0] >= 150 && NPC.localAI[2] == 1)
							{
								for (int k = 0; k < Main.maxProjectiles; k++)
								{
									if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
									{
										Main.projectile[k].ai[0] = 0;
									}
								}

								NPC.localAI[0] = 0;
								NPC.localAI[2] = 0;
								NPC.localAI[1]++;
								NPC.netUpdate = true;
							}
						}
						else
						{
							for (int k = 0; k < Main.maxProjectiles; k++)
							{
								if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
								{
									Main.projectile[k].localAI[0] = 0;
									Main.projectile[k].ai[0] = 0;
								}
							}

							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.ai[0]++;
							NPC.netUpdate = true;
						}

						break;
					}

					//make flies go above player and then charge
					case 7:
					{
						NPC.localAI[0]++;
						
						//change fly ai to charging
						for (int k = 0; k < Main.maxProjectiles; k++)
						{
							if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
							{
								if (NPC.localAI[0] == 2)
								{
									Main.projectile[k].localAI[1] = 1;
								}

								if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 145 && Main.rand.NextBool(35)|| NPC.localAI[0] == 145)
								{
									Main.projectile[k].ai[0] = 3;
								}
							}
						}

						if (NPC.localAI[1] < 2)
						{
							Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 400);

							Vector2 velocity = JumpTo - NPC.Center;

							//actual jumping
							if (NPC.localAI[0] == 30)
							{
								SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

								float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 20);
								velocity.Normalize();
								velocity.Y -= 0.18f;
								velocity.X *= 1.2f;
								NPC.velocity = velocity * speed * 1.1f;
							}

							//set no tile collide to true so he can jump through blocks
							if (NPC.localAI[0] >= 30 && NPC.localAI[0] < 85)
							{
								NPC.noTileCollide = true;
							}

							//charge down
							if (NPC.localAI[0] == 85)
							{
								NPC.noGravity = true;

								NPC.velocity.X *= 0;
								NPC.velocity.Y = 18;
							}

							//set tile collide to true after jumping so you cant avoid them
							if (NPC.localAI[0] >= 85)
							{	
								if (NPC.position.Y >= player.Center.Y - 200)
								{
									NPC.noTileCollide = false;
								}
							}

							//slam the ground
							if (NPC.localAI[0] >= 85 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
							{
								NPC.noGravity = false;

								NPC.velocity.X *= 0;

								SpookyPlayer.ScreenShakeAmount = 5;

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

								//push all nearby players in the air if they are on the ground
								for (int i = 0; i < Main.maxPlayers; i++)
								{
									if (Main.player[i].active && Main.player[i].velocity.Y == 0 && NPC.Distance(Main.player[i].Center) <= 500f)
									{
										Main.player[i].velocity.Y -= 8f;
									}
								}

								//make cool dust effect when slamming the ground
								for (int i = 0; i < 45; i++)
								{                                                                                  
									int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
									Main.dust[slamDust].noGravity = true;
									Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
									Main.dust[slamDust].position.Y += 104;
									Main.dust[slamDust].scale = 3f;
									
									if (Main.dust[slamDust].position != NPC.Center)
									{
										Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
									}
								}
								
								//complete the slam attack
								NPC.localAI[2] = 1; 
							}

							//only loop attack if the jump has been completed
							if (NPC.localAI[0] >= 140 && NPC.localAI[2] > 0)
							{
								NPC.localAI[0] = 3;
								NPC.localAI[2] = 0;
								NPC.localAI[1]++;
								NPC.netUpdate = true;
							}
						}
						else
						{
							//reset everything
							if (NPC.localAI[0] >= 20)
							{
								for (int k = 0; k < Main.maxProjectiles; k++)
								{
									if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<RotFly>()) 
									{
										Main.projectile[k].localAI[0] = 0;
										Main.projectile[k].localAI[1] = 0;
										Main.projectile[k].ai[0] = 0;
									}
								}

								NPC.localAI[0] = 0;
								NPC.localAI[1] = 0;
								NPC.localAI[2] = 0;
								NPC.ai[0] = 6;
							}
						}

						break;
					}
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RotGourdGore" + numGores).Type);
                }

				for (int numDusts = 0; numDusts < 45; numDusts++)
				{
					int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.DesertWater2, 0f, 0f, 100, default, 2f);
					Main.dust[DustGore].color = Color.MediumPurple;
					Main.dust[DustGore].scale = 0.8f;
					Main.dust[DustGore].velocity *= 1.2f;

					if (Main.rand.NextBool(2))
					{
						Main.dust[DustGore].scale = 0.5f;
						Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
					}
				}
            }
        }

		//Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

			//treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagRotGourd>()));
            
			//master relic and pet
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<RotGourdRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RottenGourd>(), 4));
            
			//spooky key
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpookyChestKey>()));

			//material
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RottenChunk>(), 1, 12, 18));

			//trophy and mask always drop directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RotGourdTrophyItem>(), 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RotGourdMask>(), 7));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedRotGourd, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}