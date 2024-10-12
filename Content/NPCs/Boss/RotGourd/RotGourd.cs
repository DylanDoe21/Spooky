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
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.NPCs.Boss.RotGourd.Projectiles;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.RotGourd
{
	[AutoloadBossHead]
	public class RotGourd : ModNPC
	{
		public bool HasSpawnedFlies = false;
		public bool FirstFlySpawned = false;
		public bool SecondFlySpawned = false;
		public bool ThirdFlySpawned = false;

		Vector2 SavePlayerPosition;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
			NPCID.Sets.MPAllowedEnemies[NPC.type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/RotGourdBestiary",
                Position = new Vector2(20f, 20f),
                PortraitPositionXOverride = -10f,
                PortraitPositionYOverride = 12f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
			writer.WriteVector2(SavePlayerPosition);

            //bools
			writer.Write(HasSpawnedFlies);
            writer.Write(FirstFlySpawned);
            writer.Write(SecondFlySpawned);
            writer.Write(ThirdFlySpawned);

			//floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			SavePlayerPosition = reader.ReadVector2();

			//bools
			HasSpawnedFlies = reader.ReadBoolean();
            FirstFlySpawned = reader.ReadBoolean();
            SecondFlySpawned = reader.ReadBoolean();
            ThirdFlySpawned = reader.ReadBoolean();

			//floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 2200;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.width = 72;
            NPC.height = 130;
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

			if (NPC.life > (NPC.lifeMax / 4))
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
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            int Damage = Main.masterMode ? 35 / 3 : Main.expertMode ? 25 / 2 : 20;

			NPC.rotation = NPC.velocity.X * 0.02f;

			//despawn if the player dies
            if (player.dead)
            {
                NPC.ai[0] = -2;
            }

			if (NPC.life < (NPC.lifeMax / 1.25f) && !FirstFlySpawned)
			{
				HasSpawnedFlies = false;
				FirstFlySpawned = true;
				NPC.netUpdate = true;
			}

			if (NPC.life < (NPC.lifeMax / 2) && !SecondFlySpawned)
			{
				HasSpawnedFlies = false;
				SecondFlySpawned = true;
				NPC.netUpdate = true;
			}

			if (NPC.life < (NPC.lifeMax / 4) && !ThirdFlySpawned)
			{
				NPC.localAI[0] = 0;
				NPC.localAI[1] = 0;
				NPC.localAI[2] = 0;
				NPC.ai[0] = 6;
				HasSpawnedFlies = false;
				NPC.noGravity = false;
				NPC.noTileCollide = false;
				ThirdFlySpawned = true;
				NPC.netUpdate = true;
			}

			//spawn swarm of flies when spawned
            if (!HasSpawnedFlies)
            {
				int maxFlies = ThirdFlySpawned ? 20 : 12;

                for (int numFlies = 0; numFlies < maxFlies; numFlies++)
                {
                    Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57f) * new Vector2(5f, 3f);

					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, vector.X, vector.Y, ModContent.ProjectileType<RotFly>(), Damage, 0f, NPC.target, 0f, (float)NPC.whoAmI);
				}

				NPC.netUpdate = true;
                HasSpawnedFlies = true;
            }

			//despawn if all players are dead
            if (player.dead)
            {
                NPC.ai[0] = -2;
            }

			//attacks
			switch ((int)NPC.ai[0])
			{	
				//despawning
				case -2:
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

					break;
				}

				//slam down spawn intro
				case -1:
				{
					NPC.localAI[0]++;

					NPC.velocity.X *= 0;

					//charge down
					if (NPC.localAI[0] == 20)
					{
						NPC.noGravity = true;

						NPC.velocity.X *= 0;
						NPC.velocity.Y = 35;
					}

					//set tile collide to true once it gets to the players level to prevent cheesing
					if (NPC.localAI[0] >= 20)
					{
						if (NPC.position.Y >= player.Center.Y - 200)
						{
							NPC.noTileCollide = false;
						}
					}

					//slam the ground
					if (NPC.localAI[0] >= 20 && NPC.localAI[1] == 0 && NPC.velocity.Y <= 0.1f)
					{
						NPC.noGravity = false;

						NPC.velocity.X *= 0;

						SpookyPlayer.ScreenShakeAmount = 7;

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
						NPC.localAI[1] = 1; 
					}

					//only loop attack if the jump has been completed
					if (NPC.localAI[0] >= 20 && NPC.localAI[1] > 0)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

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

						//set tile collide to false so he can jump through blocks
						if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 115)
						{
							NPC.noTileCollide = true;
						}

						//slam down instantly if above the player
						if (NPC.localAI[0] > 70 && NPC.localAI[0] < 110 && NPC.position.X <= player.Center.X + 3 && NPC.Center.X >= player.Center.X - 3)
						{
							NPC.localAI[0] = 115;
						}

						if (NPC.localAI[0] == 115)
						{
							NPC.velocity.X *= 0;
						}

						//slam down
						if (NPC.localAI[0] == 125)
						{
							NPC.noGravity = true;

							NPC.velocity.X *= 0;
							NPC.velocity.Y = 16;
						}

						//set tile collide to true once it gets to the players level to prevent cheesing
						if (NPC.localAI[0] >= 125)
						{	
							if (NPC.position.Y >= player.Center.Y - 200)
							{
								NPC.noTileCollide = false;
							}
						}

						//slam the ground
						if (NPC.localAI[0] >= 125 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
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
							for (int numDusts = 0; numDusts < 45; numDusts++)
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

					//set tile collide to true once it gets to the players level to prevent cheesing
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
						for (int numDusts = 0; numDusts < 65; numDusts++)
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

					//set tile collide to false so he can jump through blocks
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

							NPC.netUpdate = true;
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
							for (int numProjs = 0; numProjs < NumProjectiles; numProjs++)
							{
								float Spread = Main.rand.Next(-2500, 2500) * 0.01f;

								Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 20, Spread, 
								Main.rand.Next(-18, -13), ModContent.ProjectileType<DirtDebris>(), Damage, 2, NPC.target, 0, 0);
							}

							//make cool dust effect when slamming the ground
							for (int numDusts = 0; numDusts < 65; numDusts++)
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

							NPC.netUpdate = true;
						}

						if (NPC.localAI[2] >= 1)
						{
							NPC.localAI[0] = 50;
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
						NPC.ai[0] = 0;

						NPC.netUpdate = true;
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

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 10, 20);
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

						//set tile collide to true once it gets to the players level to prevent cheesing
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
							for (int numDusts = 0; numDusts < 45; numDusts++)
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

						//set tile collide to false so he can jump through blocks
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

						//set tile collide to true once it gets to the players level to prevent cheesing
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
							for (int numDusts = 0; numDusts < 45; numDusts++)
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

							NPC.netUpdate = true;
						}
					}

					break;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
                    {
                    	Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RotGourdGore" + numGores).Type);
					}
                }

				for (int numDusts = 0; numDusts < 45; numDusts++)
				{
					int DustGore = Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.DesertWater2, 0f, 0f, 100, default, 1.5f);
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

			//material
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RottenChunk>(), 1, 12, 20));

			//pumpkin carving kit
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<PumpkinCarvingKit>()));

			//spider grotto compass
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<GrottoCompass>()));

			//drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RotGourdMask>(), 7));

			//pumpkin gut bloom seed, drop directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FallSeed>()));

            //trophy always drops directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RotGourdTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
			if (!Flags.downedRotGourd)
			{
				Flags.GuaranteedRaveyard = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

            NPC.SetEventFlagCleared(ref Flags.downedRotGourd, -1);

			if (!MenuSaveSystem.hasDefeatedRotGourd)
			{
				MenuSaveSystem.hasDefeatedRotGourd = true;
			}
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}