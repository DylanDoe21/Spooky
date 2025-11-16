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
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class OgreKing : ModNPC
	{
		public int Spin = 0;

		public float rotate = 0;
		public float SpinX = 0;
		public float SpinY = 0;

		List<OgreKingLeg> legs;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> BodyTexture;

		public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 4;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OgreKingBestiary",
				//Position = new Vector2(0f, 22f),
              	//PortraitPositionYOverride = 18f
            };
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 26000;
            NPC.damage = 55;
			NPC.defense = 40;
			NPC.width = 86;
			NPC.height = 92;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.4f };
			NPC.DeathSound = SoundID.NPCDeath36 with { Pitch = 0.4f };
			NPC.aiStyle = -1;
            //SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		/*
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Harvestmen"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
		*/

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			BodyTexture ??= ModContent.Request<Texture2D>(Texture + "Body");

			Main.EntitySpriteDraw(BodyTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), MathHelper.PiOver2 + (NPC.velocity.X * 0.045f), NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			if (legs != null)
			{
				for (int i = 0; i < legs.Count; i++)
				{
					legs[i].Draw(NPC.Center, NPC.rotation, false, spriteBatch);
				}
			}

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if ((NPC.ai[0] == 2 && NPC.localAI[0] < 110) || NPC.ai[0] == 3)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 5)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 3)
				{
					NPC.frame.Y = 2 * frameHeight;
				}
			}
			else
			{
				if (NPC.frame.Y > frameHeight * 0)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 5)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 4)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}
			}
		}

		public void UpdateSpiderLegs()
		{
			if (legs == null)
			{
				legs = new List<OgreKingLeg>();
				Vector2[] Origin = { new Vector2(-10, -12), new Vector2(-10, -12), new Vector2(-10, -12) }; //this is for the origin point of the leg relative to the npc
				Vector2[] Destination = { new Vector2(100, -95), new Vector2(70, -120), new Vector2(50, -80) }; //this is the destination where the very bottom point of the leg should go when it moves

				for (int numLegs = -1; numLegs <= 1; numLegs += 2)
				{
					for (int i = 0; i < Destination.Length; i++)
					{
						legs.Add(new OgreKingLeg(new Vector2(Destination[i].X, Destination[i].Y * numLegs), new Vector2(Origin[i].X, Origin[i].Y * numLegs), numLegs));
					}
				}
			}
			else
			{
				for (int i = 0; i < legs.Count; i++)
				{
					legs[i].LegUpdate(NPC.Center, NPC.velocity.ToRotation(), 80, NPC.velocity);
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.rotation = MathHelper.PiOver2 + (NPC.velocity.X * 0.075f);

			UpdateSpiderLegs();

			switch ((int)NPC.ai[0])
			{
				//go above player
				case 0:
				{
					NPC.localAI[0]++;

					if (NPC.Distance(player.Center - new Vector2(0, 250)) > 100f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center - new Vector2(0, 250)) * 12;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}

					//select random attack
					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
                        NPC.ai[0] = 2;
						NPC.netUpdate = true;
					}

					break;
				}

				//Go around to random positions and put down web landmines
				case 1:
				{
					break;
				}

				//go around player and spawn webs to trap them
				case 2:
				{
					NPC.localAI[0]++;

                    //go to the top of the player
                    if (NPC.localAI[0] < 60)
                    {
						if (NPC.Distance(player.Center - new Vector2(0, 350)) > 100f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(player.Center - new Vector2(0, 350)) * 12;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
                    }

                    //curl towards the player's location
                    if (NPC.localAI[0] > 60 && NPC.localAI[0] < 240)
                    {
						//determine rotate direction randomly
						if (NPC.localAI[1] == 0)
						{
							NPC.localAI[1] = NPC.Center.X > player.Center.X ? -1 : 1;
						}

						rotate += NPC.localAI[1] * 2;

						Vector2 SpinTo = new Vector2(0, -350).RotatedBy(MathHelper.ToRadians(rotate * 1.57f));
                        
						SpinX = player.Center.X + SpinTo.X - NPC.Center.X;
						SpinY = player.Center.Y + SpinTo.Y - NPC.Center.Y;
                            
						float distance = (float)Math.Sqrt((double)(SpinX * SpinX + SpinY * SpinY));

						if (distance > 55)
						{
							distance = 6.5f / distance;
                                                
							SpinX *= distance * 3;
							SpinY *= distance * 3;
                                
							NPC.velocity.X = SpinX;
							NPC.velocity.Y = SpinY;
						}
						else
						{
							NPC.velocity.X = SpinX;
							NPC.velocity.Y = SpinY;
                                
							distance = 6.5f / distance;
                                                
							SpinX *= distance * -1;
							SpinY *= distance * -1;
						}

						/*
						if (Main.rand.NextBool(15))
						{
							SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

							Vector2 ShootSpeed = player.Center - NPC.Center;
							ShootSpeed.Normalize();
                    
							ShootSpeed = ShootSpeed * -5;

							NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-60, 60), NPC.Center.Y + Main.rand.Next(-60, 60)), 
							ShootSpeed, ModContent.ProjectileType<Phantom>(), NPC.damage, 4.5f);
						}
						*/
                    }

                    //loop attack
                    if (NPC.localAI[0] >= 240)
                    {
						SpinX = 0;
						SpinY = 0;
						rotate = 0;

						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;
						NPC.netUpdate = true;
                    }

					break;
				}

				//chase the player in quick bursts
				case 3:
				{
					break;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//Leg pattern:
			//5 2
			//4 1
			//3 0

			//on death destroy the rest of the legs
			if (NPC.life <= 0)
            {
				legs[0].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[1].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[2].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[3].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[4].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[5].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HarvestmenGore").Type);
				}
			}
		}
	}

	//spider leg code referenced from IDGCaptainRussia94's SGAmod: https://github.com/IDGCaptainRussia94/Terraria-SGAmod/blob/master/NPCs/SpiderQueen/SpiderQueen.cs
	public class OgreKingLeg
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

		public OgreKingLeg(Vector2 Startleg, Vector2 BodyPosition, int LegSide)
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
				CurrentLegPosition = Destination;
				LerpValue = 0f;
			}
		}

		public void Draw(Vector2 Position, float Angle, bool SpawnGore, SpriteBatch spriteBatch, NPC NPC = null)
		{
			int LegSegmentLength = 102;
			int LegClawLength = 118;

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
				LegSegmentTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/SpiderWar/OgreKingLegSegment");
				LegClawTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/SpiderWar/OgreKingLegFront");

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
					//Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/OgreKingLegGore1").Type);
					//Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/OgreKingLegGore2").Type);
					//Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/OgreKingLegGore3").Type);
					//Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/OgreKingLegGore4").Type);
				}
			}
		}
	}
}