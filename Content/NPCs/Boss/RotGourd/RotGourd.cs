using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.RotGourd.Projectiles;

namespace Spooky.Content.NPCs.Boss.RotGourd
{
	public class RotGourd : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rot Gourd");
			Main.npcFrameCount[NPC.type] = 2;

			var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/RotGourd/RotGourdBC",
                Position = new Vector2(20f, 20f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 10f
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
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 3000; //Main.masterMode ? 5300 / 3 : Main.expertMode ? 4200 / 2 : 3000;
            NPC.damage = 30; //Main.masterMode ? 65 / 3 : Main.expertMode ? 45 / 2 : 30;
            NPC.defense = 10;
            NPC.width = 78;
            NPC.height = 136;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/PumpkinBoss");
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiome>().Type };
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.6f);
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Rotty and his little companion jerry make up this giant, moldy gourd. Rotty will aggressively jump while jerry will act as a turret, spitting nasty mold spores and bugs."),
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

			spriteBatch.Draw(moldTex1, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			spriteBatch.Draw(moldTex2, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			spriteBatch.Draw(moldTex3, NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY) - Main.screenPosition, 
			NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = frameHeight * 1;
            }
			if (NPC.velocity.Y <= 0)
			{
				NPC.frame.Y = frameHeight * 0;
			}
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 50 / 3 : Main.expertMode ? 35 / 2 : 18;

			NPC.spriteDirection = NPC.direction;
			NPC.rotation = NPC.velocity.X * 0.01f;

			//spawn swarm of flies when spawned
            if (NPC.ai[2] <= 0)
            {
                for (int numFlies = 0; numFlies < 20; numFlies++)
                {
                    Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, vector.X, vector.Y, 
                        ModContent.ProjectileType<RotFly>(), 0, 0.0f, Main.myPlayer, 0.0f, (float)NPC.whoAmI);
                    }
                }

                NPC.netUpdate = true;

                NPC.ai[2] = 1;
            }

			switch ((int)NPC.ai[0])
			{
				//Jump 3 times towards the player
				case 0:
				{
					NPC.localAI[0]++;
								
					if (NPC.localAI[1] < 3)
					{
						Vector2 JumpTo = new(player.Center.X, player.Center.Y - 400);
						Vector2 velocity = JumpTo - NPC.Center;

						//actual jumping
						if (NPC.localAI[0] == 60)
						{
							SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

							float speed = MathHelper.Clamp(velocity.Length() / 36, 6, 18);
							velocity.Normalize();
							velocity.Y -= 0.18f;
							velocity.X *= 1.1f;
							NPC.velocity = velocity * speed * 1.1f;
						}

						//set no tile collide to true so he can jump through blocks
						if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 115)
						{
							NPC.noTileCollide = true;
						}

						//charge down
						if (NPC.localAI[0] == 115)
						{
							NPC.noGravity = true;

							NPC.velocity.X *= 0;
							NPC.velocity.Y = 18;
						}

						//set tile collide to true after jumping so you cant avoid them
						if (NPC.localAI[0] > 115)
						{	
							if (NPC.position.Y >= player.Center.Y - 150)
							{
								NPC.noTileCollide = false;
							}
						}

						//slam the ground
						if (NPC.localAI[0] > 115 && NPC.localAI[2] == 0 && NPC.velocity.Y <= 0.1f)
						{
							NPC.noGravity = false;

							NPC.velocity.X *= 0;

							SpookyPlayer.ScreenShakeAmount = 5;

							SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

							//push player in the air if they are on the ground
							if (player.velocity.Y == 0)
							{
								player.velocity.Y -= 8f;
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
							
							//"complete" the jump attack
							NPC.localAI[2] = 1; 
						}

						//only loop attack if the jump has been completed
						if (NPC.localAI[0] >= 185 && NPC.localAI[2] > 0)
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
						NPC.ai[0]++;
						NPC.netUpdate = true;
					}

					break;
				}

				case 1:
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

						Vector2 ChargeDirection = NPC.velocity;
						ChargeDirection.Normalize();

						ChargeDirection.X *= 0;
						NPC.velocity.Y = 32;
					}

					//set tile collide to true after jumping
					if (NPC.localAI[0] > 80)
					{
						if (NPC.position.Y >= player.Center.Y - 150)
						{
							NPC.noTileCollide = false;
						}
					}

					//slam the ground
					if (NPC.localAI[0] > 80 && NPC.localAI[1] == 0 && NPC.velocity.Y <= 0.1f)
					{
						NPC.noGravity = false;

						SpookyPlayer.ScreenShakeAmount = 10;

						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

						//push player in the air if they are on the ground
						if (player.velocity.Y == 0)
						{
							player.velocity.Y -= 12f;
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
						
						//"complete" the jump attack
						NPC.localAI[1] = 1; 
					}

					if (NPC.localAI[0] > 80 && NPC.localAI[1] > 0)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}
			}
		}

        public override void BossLoot(ref string name, ref int potionType)
        {
            base.BossLoot(ref name, ref potionType);
        }
    }
}