using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
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
using Spooky.Content.Dusts;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Quest
{
    public class FrankenGoblin : ModNPC  
    {
		int CurrentFrameX = 0; //0 = idle/sleeping  1 = walking/jumping
		int SaveDirection;

        bool SpawnedHands = false;

		private static Asset<Texture2D> NPCTexture;

		public static readonly SoundStyle StretchSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound) { Pitch = -1f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
			NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/FrankenGoblinBestiary",
                Position = new Vector2(18f, 65f),
				PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 40f
            };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			//ints
			writer.Write(SaveDirection);

            //bools
            writer.Write(SpawnedHands);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//ints
            SaveDirection = reader.ReadInt32();

            //bools
            SpawnedHands = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5000;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 142;
			NPC.height = 158;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit52 with { Pitch = 1.5f, Volume = 2f };
			NPC.DeathSound = SoundID.NPCDeath5 with { Pitch = 1.2f };
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.85f * bossAdjustment);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FrankenGoblin"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

			//sleeping animation
			if (NPC.localAI[3] == 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 15)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 4)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			else
			{
				//standing still
				if (CurrentFrameX == 0)
				{
					if (NPC.frame.Y < frameHeight * 5)
					{
						NPC.frame.Y = 4 * frameHeight;
					}

					NPC.frameCounter++;
					if (NPC.frameCounter > 8)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 9)
					{
						NPC.frame.Y = 4 * frameHeight;
					}
				}
				//running animation
				else
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 7)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}

				//jumping frame
				if (NPC.velocity.Y < 0)
				{
					NPC.frame.Y = 7 * frameHeight;
				}

				//falling frame
				if (NPC.velocity.Y > 0)
				{
					NPC.frame.Y = 8 * frameHeight;
				}
			}
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return NPC.localAI[3] != 0;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			if (NPC.localAI[3] == 0)
			{
				NPC.spriteDirection = 1;

				NPC.aiStyle = 0;
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
			}

			if (!SpawnedHands)
			{
				int FrontHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-45, -20), (int)NPC.Center.Y, ModContent.NPCType<FrankenGoblinHandFront>(), ai0: NPC.whoAmI);
				int BackHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(20, 35), (int)NPC.Center.Y, ModContent.NPCType<FrankenGoblinHandBack>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: FrontHand);
					NetMessage.SendData(MessageID.SyncNPC, number: BackHand);
				}

				SpawnedHands = true;
				NPC.netUpdate = true;
			}

			switch ((int)NPC.localAI[3])
			{
				//spawn sleepy dusts
				case 0:
				{
					CurrentFrameX = 0;

					if (!Main.gamePaused)
					{
						if (Main.rand.NextBool(75))
						{
							Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-25, 25)), 5, 5, ModContent.DustType<GoblinSleepyDust>());
						}
					}

					break;
				}
				//move at the player, swipe hands if they get too close
				case 1:
				{
					NPC.localAI[0]++;

					CurrentFrameX = 1;

					NPC.aiStyle = 3;
					AIType = NPCID.GoblinScout;

					if (NPC.localAI[0] > 300)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[3]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//stand still, spin hands around and make them follow the player
				case 2:
				{
					if (NPC.velocity.Y == 0 || NPC.localAI[0] >= 80)
					{
						NPC.localAI[0]++;
					}

					if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 480)
					{
						CurrentFrameX = 0;

						NPC.aiStyle = 0;
					}
					else
					{
						CurrentFrameX = 1;

						NPC.aiStyle = 3;
						AIType = NPCID.GoblinScout;
					}

					if (NPC.localAI[0] == 80 && NPC.velocity.Y == 0)
					{
						SoundEngine.PlaySound(StretchSound, NPC.Center);
					}

					if (NPC.localAI[0] >= 600)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[3]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//jump towards the player, land, and then make a bunch of debris fly up
				case 3:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] > 40 && NPC.localAI[0] < 60)
					{
						CurrentFrameX = 0;

						NPC.velocity.X *= 0.5f;

						NPC.velocity.Y += 2f;

						NPC.aiStyle = 0;
					}
								
					if (NPC.localAI[2] < 3)
					{
						//spawn eyes all over the ground
						if (NPC.localAI[0] == 40)
						{
							for (int i = -4; i <= 4; i++)
							{
								Vector2 center = new Vector2(player.Center.X, player.Center.Y - 100);

								center.X += Main.rand.Next(200, 300) * i; //distance between each eye

								int numtries = 0;
								int x = (int)(center.X / 16);
								int y = (int)(center.Y / 16);

								while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
								{
									y++;
									center.Y = y * 16;
								}
								while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
								{
									numtries++;
									y--;
									center.Y = y * 16;
								}

								if (numtries >= 10)
								{
									break;
								}

								int Eye = Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X - 3, center.Y, 0, 0, ModContent.ProjectileType<GoblinEyeDebrisGrounded>(), NPC.damage / 4, 0, Main.myPlayer);
								Main.projectile[Eye].frame = Main.rand.Next(0, 4);
								Main.projectile[Eye].ai[1] = NPC.whoAmI;
							}
						}

						//jumping velocity
						Vector2 JumpTo = new Vector2(player.Center.X, player.Center.Y - 500);

						Vector2 velocity = JumpTo - NPC.Center;

						//actual jumping
						if (NPC.localAI[0] == 60)
						{
							CurrentFrameX = 1;

							SaveDirection = NPC.spriteDirection;

							NPC.aiStyle = -1;

							SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

							float speed = MathHelper.Clamp(velocity.Length() / 36, 10, 25);
							velocity.Normalize();
							velocity.Y -= 0.22f;
							velocity.X *= 1.05f;
							NPC.velocity = velocity * speed * 1.1f;

							NPC.netUpdate = true;
						}

						if (NPC.localAI[0] > 60)
						{
							NPC.spriteDirection = SaveDirection;
						}

						//set tile collide to false so he can jump through blocks
						if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 115)
						{
							NPC.noTileCollide = true;
						}

						//fall down a bit before slamming
						if (NPC.localAI[0] > 60 && NPC.localAI[0] < 115)
						{
							NPC.velocity.Y += 0.2f;
						}

						//lower velocity before and while slaming down
						if (NPC.localAI[0] > 90)
						{
							NPC.velocity.X *= 0.985f;
						}

						//slam down
						if (NPC.localAI[0] == 115)
						{
							NPC.noGravity = true;

							NPC.velocity.Y = 16;
						}

						//set tile collide to true once it gets to the players level to prevent cheesing
						if (NPC.localAI[0] >= 115)
						{	
							if (NPC.position.Y >= player.Center.Y - 200)
							{
								NPC.noTileCollide = false;
							}
						}

						//slam the ground
						if (NPC.localAI[0] >= 120 && NPC.localAI[1] == 0 && NPC.velocity.Y == 0)
						{
							CurrentFrameX = 0;

							NPC.noGravity = false;

							NPC.velocity.X *= 0;

							SpookyPlayer.ScreenShakeAmount = 5;

							SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound with { Pitch = 1.2f }, NPC.Center);

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
							NPC.localAI[1] = NPC.localAI[0];
						}

						//only loop attack if the jump has been completed
						if (NPC.localAI[0] >= NPC.localAI[1] + 20 && NPC.localAI[1] > 0)
						{
							NPC.localAI[0] = 30;
							NPC.localAI[1] = 0;
							NPC.localAI[2]++;

							NPC.netUpdate = true;
						}
					}
					else
					{
						if (NPC.localAI[0] >= 60)
						{
							CurrentFrameX = 1;

							NPC.aiStyle = 3;
							AIType = NPCID.GoblinScout;

							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.localAI[3] = NPC.life < (NPC.lifeMax / 1.75f) ? 4 : 1;

							NPC.netUpdate = true;
						}
					}

					break;
				}

				//stand still, pick up eyeball minions and then throw them
				case 4:
				{
					if (NPC.velocity.Y == 0 || NPC.localAI[0] >= 80)
					{
						NPC.localAI[0]++;
					}

					if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 480)
					{
						CurrentFrameX = 0;

						NPC.aiStyle = 0;
					}
					else
					{
						CurrentFrameX = 1;

						NPC.aiStyle = 3;
						AIType = NPCID.GoblinScout;
					}

					if (NPC.localAI[0] == 80 && NPC.velocity.Y == 0)
					{
						SoundEngine.PlaySound(StretchSound, NPC.Center);
					}

					if (NPC.localAI[0] >= 200)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.localAI[3] = 1;

						NPC.netUpdate = true;
					}

					break;
				}
			}
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<FrankenGoblinRelicItem>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BountyItem1>()));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //become aggressive on hit
			if (NPC.localAI[3] == 0)
			{
				NPC.localAI[3]++;

				NPC.netUpdate = true;
			}

            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 15; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FrankenGoblinGore" + numGores).Type);
                    }
                }
            }
        }
    }
}