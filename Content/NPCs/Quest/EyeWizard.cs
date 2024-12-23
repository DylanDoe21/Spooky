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
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Quest
{
	public class EyeWizard : ModNPC
	{
		int CurrentFrameX = 0; //0 = idle flying animation  1 = go inside cloak

		bool SpawnedOrb = false;
		bool SpawnedClone = false;

		Vector2 SaveNPCPosition;
        Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> EyeClosedTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
			NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
            writer.WriteVector2(SavePlayerPosition);
            writer.WriteVector2(SaveNPCPosition);

			//ints
            writer.Write(CurrentFrameX);

			//bools
            writer.Write(SpawnedOrb);
			writer.Write(SpawnedClone);

            //floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
            SavePlayerPosition = reader.ReadVector2();
            SaveNPCPosition = reader.ReadVector2();

			//ints
            CurrentFrameX = reader.ReadInt32();

			//bools
            SpawnedOrb = reader.ReadBoolean();
			SpawnedClone = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 3700;
            NPC.damage = 32;
			NPC.defense = 12;
			NPC.width = 54;
			NPC.height = 116;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EyeWizard"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardGlow");
			EyeClosedTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardClosed");

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			if (CurrentFrameX == 1)
            {
                for (int i = 0; i < 360; i += 90)
                {
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.HotPink);

                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
                    spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.1f, effects, 0);
                }
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			
			if (NPC.ai[0] == 0 && !NPC.IsABestiaryIconDummy)
			{
				spriteBatch.Draw(EyeClosedTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			}
			else
			{
				spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			}

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            //flying animation
			if (CurrentFrameX == 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 8)
				{
					NPC.frame.Y = 4 * frameHeight;
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.ai[0] != 0;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.X * 0.03f;

			if (!SpawnedOrb)
			{
				int Orb = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (Main.rand.NextBool() ? -100 : 100), (int)NPC.Center.Y, ModContent.NPCType<EyeWizardOrb>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Orb);
				}

				SpawnedOrb = true;

				NPC.netUpdate = true;
			}

			if (NPC.life < (NPC.lifeMax / 2) && !SpawnedClone)
			{
				int Clone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<EyeWizardClone>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Clone);
				}

				SpawnedClone = true;

				NPC.netUpdate = true;
			}

			if (NPC.ai[0] == 0)
			{
				NPC.noGravity = false;
				NPC.noTileCollide = false;

				NPC.immortal = true;
				NPC.dontTakeDamage = true;
			}
			else
			{
				NPC.noGravity = true;
				NPC.noTileCollide = true;

				NPC.immortal = false;
				NPC.dontTakeDamage = false;

				NPC.localAI[2]++;

				//passively spawn dust rings below to make it look like its floating
				if (NPC.localAI[2] % 10 == 0)
				{
					SoundEngine.PlaySound(SoundID.Item24, NPC.Center);

					Vector2 NPCVelocity = NPC.velocity * 0.4f + Vector2.UnitY;
					Vector2 NPCOffset = NPC.Center + new Vector2(0, NPC.height / 2);

					for (int i = 0; i <= 20; i++)
					{
						Vector2 position = -Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 20) * new Vector2(1f, 0.25f);
						Vector2 velocity = NPCVelocity + position * 1.25f;
						position = position * 12 + NPCOffset;
						Dust dust = Dust.NewDustPerfect(position, 90, velocity);
						dust.noGravity = true;
						dust.scale = 0.8f + 10 * 0.04f;
					}
				}
			}

			switch ((int)NPC.ai[0])
			{
				//fly to the player briefly
				case 1:
				{
					NPC.localAI[0]++;

					//go to the player
					if (player.Distance(NPC.Center) > 220f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 7;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.95f;
					}

					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0]++;
					
						NPC.netUpdate = true;
					}

					break;
				}

				//shoot out bolts that turn into lingering eye runes
				case 2:
				{
					NPC.localAI[0]++;

					//go to the player
					if (NPC.localAI[0] < 70)
					{
						Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 200);

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 15);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[1] < 3)
					{
						if (NPC.localAI[0] >= 70)
						{
							NPC.velocity *= 0.85f;
						}

						if (NPC.localAI[0] == 75)
						{
							CurrentFrameX = 1;
							NPC.frame.Y = 0;

							SaveNPCPosition = NPC.Center;

							NPC.netUpdate = true;
						}

						if (NPC.localAI[0] > 75 && NPC.localAI[0] <= 125)
						{
							//NPC.Center = SaveNPCPosition;
							NPC.Center += Main.rand.NextVector2Square(-5, 5);
						}

						if (NPC.localAI[0] == 125)
						{
							SoundEngine.PlaySound(SoundID.Item80, NPC.Center);

							Vector2 NPCPosition = NPC.Center + new Vector2(0, 25).RotatedByRandom(360);

							Vector2 ShootSpeed = NPC.Center - NPCPosition;
							ShootSpeed.Normalize();
							ShootSpeed *= -3.5f;

							NPCGlobalHelper.ShootHostileProjectile(NPC, NPCPosition, ShootSpeed, ModContent.ProjectileType<LingeringEyeSpawner>(), NPC.damage, 3.5f);
						}

						if (NPC.localAI[0] >= 145)
						{
							CurrentFrameX = 0;
							NPC.frame.Y = 0;

							NPC.localAI[0] = 0;
							NPC.localAI[1]++;

							NPC.netUpdate = true;
						}
					}
					else
					{
						if (NPC.localAI[0] >= 65)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.ai[0]++;
						
							NPC.netUpdate = true;
						}
					}

					break;
				}

				//shout out homing eyes
				case 3:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 420)
					{
						Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 200);

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 10);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] >= 180)
					{
						NPC.velocity *= 0.85f;
					}

					if (NPC.localAI[0] == 150)
					{
						CurrentFrameX = 1;
						NPC.frame.Y = 0;
					}

					if (NPC.localAI[0] == 180)
					{
						SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
					}

					if (NPC.localAI[0] > 180 && NPC.localAI[0] <= 420)
					{
						SaveNPCPosition = NPC.Center;

						//NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);

						if (NPC.localAI[0] % 40 == 0)
						{
							Vector2 NPCPosition = NPC.Center + new Vector2(0, 25).RotatedByRandom(360);

							SoundEngine.PlaySound(SoundID.Item79, NPCPosition);

							Vector2 ShootSpeed = NPC.Center - NPCPosition;
							ShootSpeed.Normalize();
							ShootSpeed *= Main.rand.NextFloat(-10f, -5f);

							NPCGlobalHelper.ShootHostileProjectile(NPC, NPCPosition, ShootSpeed, ModContent.ProjectileType<HomingEye>(), NPC.damage, 3.5f);
						}
					}

					if (NPC.localAI[0] == 420)
					{
						CurrentFrameX = 0;
						NPC.frame.Y = 0;
					}

					//float towards player
					if (NPC.localAI[0] >= 420)
					{
						//go to the player
						if (player.Distance(NPC.Center) > 220f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 7;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else
						{
							NPC.velocity *= 0.95f;
						}
					}

					if (NPC.localAI[0] >= 630)
					{
						CurrentFrameX = 0;
						NPC.frame.Y = 0;

						NPC.localAI[0] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//shout out bouncing eyes
				case 4:
				{
					NPC.localAI[0]++;

					//go to the player
					if (NPC.localAI[0] < 300)
					{
						Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 200);

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 8);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] >= 240)
					{
						NPC.velocity *= 0.85f;
					}

					if (NPC.localAI[0] == 240)
					{
						CurrentFrameX = 1;
						NPC.frame.Y = 0;

                        NPC.netUpdate = true;
					}

					if (NPC.localAI[0] > 240 && NPC.localAI[0] <= 360)
					{
						SaveNPCPosition = NPC.Center;

						//NPC.Center = SaveNPCPosition;
                        NPC.Center += Main.rand.NextVector2Square(-5, 5);

						if (NPC.localAI[0] % 10 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item85, NPC.Center);

							NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y - 45),
							new Vector2(Main.rand.Next(-15, 16), Main.rand.Next(-15, -6)), ModContent.ProjectileType<BouncingEye>(), NPC.damage, 3.5f);
						}
					}

					if (NPC.localAI[0] == 360)
					{
						CurrentFrameX = 0;
						NPC.frame.Y = 0;
					}

					//float towards player
					if (NPC.localAI[0] >= 360)
					{
						//go to the player
						if (player.Distance(NPC.Center) > 220f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 7;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else
						{
							NPC.velocity *= 0.95f;
						}
					}

					if (NPC.localAI[0] >= 600)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0] = 1;

						NPC.netUpdate = true;
					}

					break;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<EyeWizardRelicItem>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BountyItem4>()));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 3, ModContent.Find<ModGore>("Spooky/EyeWizardHatGore").Type);
				}

				for (int Repeats = 1; Repeats <= 2; Repeats++)
                {
					for (int numGores = 1; numGores <= 9; numGores++)
					{
						if (Main.netMode != NetmodeID.Server) 
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-45, 46)), NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeWizardClothGore" + numGores).Type);
						}
					}
				}
            }
        }
	}
}