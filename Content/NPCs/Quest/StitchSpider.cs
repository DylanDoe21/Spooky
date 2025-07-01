using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Quest
{
	public class StitchSpider : ModNPC
	{
		List<SpiderLeg> legs;

		float IdleSpeed = 0f;
		float SaveRotation;

		bool LostLeg1 = false;
		bool LostLeg2 = false;
		bool LostLeg3 = false;
		bool LostLeg4 = false;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
			NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/StitchSpiderBestiary",
                Position = new Vector2(1f, -25f),
                PortraitPositionXOverride = 1f,
                PortraitPositionYOverride = -20f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(LostLeg1);
            writer.Write(LostLeg2);
            writer.Write(LostLeg3);
            writer.Write(LostLeg4);

            //floats
			writer.Write(IdleSpeed);
			writer.Write(SaveRotation);
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();

            //bools
            LostLeg1 = reader.ReadBoolean();
            LostLeg2 = reader.ReadBoolean();
            LostLeg3 = reader.ReadBoolean();
            LostLeg4 = reader.ReadBoolean();

            //floats
            IdleSpeed = reader.ReadSingle();
            SaveRotation = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 4000;
            NPC.damage = 35;
			NPC.defense = 15;
			NPC.width = 58;
			NPC.height = 58;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.Item14;
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.StitchSpider"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (legs != null)
			{
				for (int i = 0; i < legs.Count; i++)
				{
					if (i == 3 && LostLeg1) continue;
					if (i == 5 && LostLeg2) continue;
					if (i == 7 && LostLeg3) continue;
					if (i == 1 && LostLeg4) continue;

					legs[i].Draw(NPC.Center, NPC.rotation, false, spriteBatch);
				}
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/StitchSpiderGlow");

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(Color.White)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
            //flying animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return NPC.ai[0] != 0;
        }

		public void UpdateSpiderLegs()
		{
			if (legs == null)
			{
				legs = new List<SpiderLeg>();
				Vector2[] legbody = { new Vector2(-10, -12), new Vector2(0, -12), new Vector2(10, -12), new Vector2(20, -8) };
				Vector2[] legbodyExtended = { new Vector2(-12, -64), new Vector2(32, -84), new Vector2(72, -84), new Vector2(100, -80) };

				for (int numLegs = -1; numLegs < 2; numLegs += 2)
				{
					for (int i = 0; i < legbodyExtended.Length; i++)
					{
						legs.Add(new SpiderLeg(new Vector2(legbodyExtended[i].X, legbodyExtended[i].Y * numLegs), new Vector2(legbody[i].X, legbody[i].Y * numLegs), numLegs));
					}
				}
			}
			else
			{
				for (int i = 0; i < legs.Count; i += 1)
				{
					legs[i].LegUpdate(NPC.Center, NPC.rotation, 128, NPC.velocity);
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.rotation = NPC.velocity.ToRotation();

			UpdateSpiderLegs();

			switch ((int)NPC.ai[0])
			{
				//passive movement before becoming hostile
				case 0:
				{
					if (IdleSpeed > 0.06f)
					{
						IdleSpeed -= 0.1f;
					}
					if (IdleSpeed < 0.05f)
					{
						IdleSpeed += 0.01f;
					}

					Vector2 MoveSpeed = player.Center - NPC.Center;
					MoveSpeed.Normalize();
							
					MoveSpeed *= IdleSpeed;
					NPC.velocity += MoveSpeed / 4;

					NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -5f, 5f);
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -5f, 5f);

					break;
				}

				//chase the player directly
				case 1:
				{
					NPC.localAI[0]++;

					if (player.Distance(NPC.Center) >= 25f)
					{
						Vector2 MoveSpeed = player.Center - NPC.Center;
						MoveSpeed.Normalize();
								
						MoveSpeed *= 4f;
						NPC.velocity = MoveSpeed;
					}

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//go above the player and then charge at the player
				case 2:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 100)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += (NPC.Center.X < player.Center.X) ? -600 : 600;
						GoTo.Y += (NPC.Center.Y < player.Center.Y) ? -600 : 600;

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] == 100)
					{
						SoundEngine.PlaySound(SoundID.NPCHit29 with { Pitch = -0.5f }, NPC.Center);

						NPC.velocity *= 0.1f;

						SavePlayerPosition = player.Center;
					}

					if (NPC.localAI[0] == 130)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath31 with { Pitch = -0.5f }, NPC.Center);

						Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
						ChargeDirection.Normalize();
								
						ChargeDirection *= 45f;
						NPC.velocity = ChargeDirection;
					}

					if (NPC.localAI[0] >= 130)
					{
						NPC.velocity *= 0.965f;
					}

					if (NPC.localAI[0] >= 270)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//shoot web to trap the player in
				case 3:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] <= 45)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += (NPC.Center.X < player.Center.X) ? -600 : 600;

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}
					else
					{
						if (player.Distance(NPC.Center) >= 25f)
						{
							Vector2 MoveSpeed = player.Center - NPC.Center;
							MoveSpeed.Normalize();
									
							MoveSpeed *= 2f;
							NPC.velocity = MoveSpeed;
						}
					}

					if (NPC.localAI[0] >= 180 && NPC.localAI[0] <= 240)
					{
						if (NPC.localAI[0] % 10 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

							Vector2 ShootSpeed = (player.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21))) - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= 35f;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 75f;
							Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

							if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
							{
								position += muzzleOffset;
							}

							NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<SpiderWeb>(), NPC.damage, 3.5f);
						}
					}

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//go to the side of the player, charge horizontally, repeat 3 times
				case 4:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[1] < (NPC.life < (NPC.lifeMax / 2) ? 3 : 2))
					{
						if (NPC.localAI[0] < 45)
						{
							Vector2 GoTo = player.Center;
							GoTo.X += (NPC.Center.X < player.Center.X) ? -600 : 600;

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
						}

						if (NPC.localAI[0] == 45)
						{
							NPC.velocity *= 0.1f;

							SavePlayerPosition = player.Center;
						}

						if (NPC.localAI[0] == 55)
						{
							SoundEngine.PlaySound(SoundID.NPCDeath31 with { Pitch = -0.5f }, NPC.Center);

							Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
							ChargeDirection.Normalize();
									
							ChargeDirection *= 35f;
							NPC.velocity.X = ChargeDirection.X;
							NPC.velocity.Y = ChargeDirection.Y / 5;
						}

						if (NPC.localAI[0] >= 55)
						{
							NPC.velocity *= 0.98f;
						}

						if (NPC.localAI[0] >= 130)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1]++;

							NPC.netUpdate = true;
						}
					}
					else
					{
						if (NPC.localAI[0] >= 20)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.ai[0] = NPC.life < (NPC.lifeMax / 2) ? 5 : 1;

							NPC.netUpdate = true;
						}
					}

					break;
				}

				//rapid fire detonating flames out of its cannon
				case 5:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 55)
					{
						Vector2 GoTo = player.Center;
						GoTo.X += (NPC.Center.X < player.Center.X) ? -350 : 350;
						GoTo.Y -= 150;

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] == 55)
					{
						NPC.velocity.X *= 0;
						NPC.velocity.Y = 5;
					}

					if (NPC.localAI[0] >= 55 && NPC.localAI[0] <= 150)
					{
						NPC.velocity *= 0.97f;
					}

					if (NPC.localAI[0] >= 100 && NPC.localAI[0] <= 240 && NPC.localAI[0] % 10 == 0)
					{
						SoundEngine.PlaySound(SoundID.Item61, NPC.Center);

						NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + 1, NPC.Center.Y - 55), 
						new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-6, -3)), ModContent.ProjectileType<SpiderMissile>(), NPC.damage, 3.5f);
					}

					if (NPC.localAI[0] >= 360)
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
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<StitchSpiderRelicItem>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BountyItem3>()));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//make spider aggressive
			if (NPC.ai[0] == 0)
			{
				NPC.ai[0]++;

				NPC.netUpdate = true;
			}

			//Leg pattern:
			//7 3
			//6 2
			//5 1
			//4 0

			//cause legs to fall off at certain hp intervals
            if (NPC.life <= (NPC.lifeMax * 0.8f) && !LostLeg1)
            {
				legs[3].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				LostLeg1 = true;
			}
			if (NPC.life <= (NPC.lifeMax * 0.6f) && !LostLeg2)
            {
				legs[5].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				LostLeg2 = true;
			}
			if (NPC.life <= (NPC.lifeMax * 0.4f) && !LostLeg3)
            {
				legs[7].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				LostLeg3 = true;
			}
			if (NPC.life <= (NPC.lifeMax * 0.2f) && !LostLeg4)
            {
				legs[1].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				LostLeg4 = true;
			}

			//on death destroy the rest of the legs
			if (NPC.life <= 0)
            {
				legs[0].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[2].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[4].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[6].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/StitchSpiderGore" + numGores).Type);
                    }
                }
			}
		}
	}

	//spider leg code referenced from IDGCaptainRussia94's SGAmod: https://github.com/IDGCaptainRussia94/Terraria-SGAmod/blob/master/NPCs/SpiderQueen/SpiderQueen.cs
	public class SpiderLeg
	{
		int LegSide;

		float MaxLegDistance;
		float LerpValue = 1f;

		Vector2 BodyPosition;
		Vector2 LegPosition;
		Vector2 PreviousLegPosition;
		Vector2 CurrentLegPosition;
		Vector2 DesiredLegPosition;

		private static Asset<Texture2D> LegSegmentTexture;
		private static Asset<Texture2D> LegClawTexture;

		public static readonly SoundStyle WalkSound = new("Spooky/Content/Sounds/SpiderWalk", SoundType.Sound) { Volume = 1.2f, PitchVariance = 0.6f };

		public SpiderLeg(Vector2 Startleg, Vector2 BodyPosition, int LegSide)
		{
			LegPosition = Startleg;
			PreviousLegPosition = Startleg;
			CurrentLegPosition = Startleg;
			DesiredLegPosition = Startleg;
			this.BodyPosition = BodyPosition;
			this.LegSide = LegSide;
		}

		public void LegUpdate(Vector2 Position, float Angle, float MaxLegDistance, Vector2 Velocity)
		{
			bool spin = MaxLegDistance < 94;
			this.MaxLegDistance = MaxLegDistance;
			float dev = 2f;
			float forward = Math.Abs((Velocity.Length() - 4f) * 8f) * (DesiredLegPosition.X > -0 ? DesiredLegPosition.X / 100f : 1f);

			if (spin)
			{
				forward -= (125f - DesiredLegPosition.X);
			}

			Vector2 Destination = Position + (new Vector2(forward, 0f) + DesiredLegPosition).RotatedBy(Angle);

			LerpValue += (1f - LerpValue) / dev;
			LegPosition = Vector2.Lerp(PreviousLegPosition, CurrentLegPosition, LerpValue);

			if ((LegPosition - Destination).Length() > (MaxLegDistance + ((dev - 4f) * 16f)) + 74)
			{
				SoundEngine.PlaySound(WalkSound, LegPosition);

				PreviousLegPosition = LegPosition;
				CurrentLegPosition = Destination + new Vector2(Main.rand.Next(-24, 24), Main.rand.Next(-24, 24));
				LerpValue = 0f;
			}
		}

		public void Draw(Vector2 Position, float Angle, bool SpawnGore, SpriteBatch spriteBatch, NPC NPC = null)
		{
			int LegSegmentLength = 68;
			int LegClawLength = 78;

			Vector2 start = Position + BodyPosition.RotatedBy(Angle);

			Vector2 middle = LegPosition - start;

			float angleleg1 = (LegPosition - start).ToRotation() + (MathHelper.Clamp((MathHelper.Pi / 2f) - MathHelper.ToRadians(middle.Length() / 1.6f), MathHelper.Pi / 12f, MathHelper.Pi / 2f) * LegSide);

			Vector2 legdist = angleleg1.ToRotationVector2();
			legdist.Normalize();
			Vector2 halfway1 = legdist;
			legdist *= LegSegmentLength - 7;

			Vector2 leg2 = (Position + BodyPosition.RotatedBy(Angle)) + legdist;

			float angleleg2 = (LegPosition - leg2).ToRotation();

			halfway1 *= LegSegmentLength / 2;
			Vector2 halfway2 = leg2 + (angleleg2.ToRotationVector2() * LegClawLength / 2);

			if (!SpawnGore)
			{
				LegSegmentTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/StitchSpiderLegSegment");
				LegClawTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/StitchSpiderLegSegmentClaw");

				//first leg segment
				Color LightColor = Lighting.GetColor((int)((start.X + halfway1.X) / 16f), (int)((start.Y + halfway1.Y) / 16f));
				spriteBatch.Draw(LegSegmentTexture.Value, start - Main.screenPosition, null, LightColor, angleleg1, new Vector2(4, LegSegmentTexture.Height() / 2f), 1f, angleleg1.ToRotationVector2().X > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);

				//second leg segment
				LightColor = Lighting.GetColor((int)(halfway2.X / 16f), (int)(halfway2.Y / 16f));
				spriteBatch.Draw(LegClawTexture.Value, leg2 - Main.screenPosition, null, LightColor, angleleg2, new Vector2(4, LegClawTexture.Height() / 2f), 1f, angleleg2.ToRotationVector2().X > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
			}
			else
			{
				if (Main.netMode != NetmodeID.Server)
				{
					Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore1").Type);
					Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore2").Type);
					Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore3").Type);
					Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore4").Type);
				}
			}
		}
	}
}