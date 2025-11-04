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
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class CamelColonel : ModNPC
	{
		List<CamelColonelLeg> legs;

		float SaveRotation;

		bool SpawnedSegments = false;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 3;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OgreKingBestiary",
				//Position = new Vector2(0f, 22f),
              	//PortraitPositionYOverride = 18f
            };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 23000;
            NPC.damage = 70;
			NPC.defense = 40;
			NPC.width = 86;
			NPC.height = 92;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.4f };
			NPC.DeathSound = SoundID.NPCDeath36 with { Pitch = 0.4f };
			NPC.aiStyle = -1;
            //SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
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

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

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

		public void UpdateSpiderLegs()
		{
			if (legs == null)
			{
				legs = new List<CamelColonelLeg>();
				Vector2[] Origin = { new Vector2(-18, -12), new Vector2(-18, -12) }; //this is for the origin point of the leg relative to the npc
				Vector2[] Destination = { new Vector2(90, -80), new Vector2(60, -65) }; //this is the destination where the very bottom point of the leg should go when it moves

				for (int numLegs = -1; numLegs <= 1; numLegs += 2)
				{
					for (int i = 0; i < Destination.Length; i++)
					{
						legs.Add(new CamelColonelLeg(new Vector2(Destination[i].X, Destination[i].Y * numLegs), new Vector2(Origin[i].X, Origin[i].Y * numLegs), numLegs));
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

			if (SaveRotation == 0 || NPC.ai[0] == 4)
			{
				float RotateDirection = NPC.velocity.ToRotation();
				float RotateSpeed = 0.1f;

				NPC.rotation = NPC.rotation.AngleTowards(RotateDirection, RotateSpeed);
			}

			UpdateSpiderLegs();

			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				if (!SpawnedSegments)
				{
					int latestNPC = NPC.whoAmI;
					for (int numSegments = 0; numSegments < 8; numSegments++)
					{
						latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<CamelColonelTail>(), NPC.whoAmI, 0, latestNPC);
						Main.npc[latestNPC].ai[2] = numSegments / 2;
						Main.npc[latestNPC].ai[3] = NPC.whoAmI;
						NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
					}

					latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<CamelColonelTailSniper>(), NPC.whoAmI, 0, latestNPC);
					Main.npc[latestNPC].ai[3] = NPC.whoAmI;
					NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

					SpawnedSegments = true;
					NPC.netUpdate = true;
				}
			}

			switch ((int)NPC.ai[0])
			{
				//move towards player slowly
				case 0: 
				{
					NPC.localAI[0]++;

					Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 3.5f;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

					//select random attack
					if (NPC.localAI[0] >= 240)
					{
						NPC.localAI[0] = 0;

						//use sniper attack when far away, but a slight chance to use venom bolts
						if (NPC.Distance(player.Center) >= 450f)
						{
							NPC.ai[0] = Main.rand.Next(4) > 0 ? 1 : 2;
						}
						//use venom bolts at mid range
						if (NPC.Distance(player.Center) < 450f && NPC.Distance(player.Center) >= 250f)
						{
							NPC.ai[0] = 2;
						}
						//if at close range, do running away behavior and then sniper attack
						if (NPC.Distance(player.Center) < 250f)
						{
							NPC.ai[0] = 3;
						}

						NPC.netUpdate = true;
					}

					break;
				}

				//shoot green sniper bolts from tail
				case 1:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 5)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 2;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

						SaveRotation = NPC.rotation;
					}
					else
					{
						NPC.rotation = SaveRotation;
						NPC.velocity = Vector2.Zero;
					}

					if (NPC.localAI[0] >= 180)
					{
						SaveRotation = 0;

						NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//rapid fire venom bolts from tail
				case 2:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 5)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 2;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

						SaveRotation = NPC.rotation;
					}
					else
					{
						NPC.rotation = SaveRotation;
						NPC.velocity = Vector2.Zero;
					}

					if (NPC.localAI[0] >= 180)
					{
						SaveRotation = 0;

						NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//run away, then switch to green sniper bolt behavior
				case 3:
				{
					NPC.localAI[0]++;

					Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * -6.5f;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

					if (NPC.localAI[0] >= 180)
					{
						SaveRotation = 0;

						NPC.localAI[0] = 0;
                        NPC.ai[0] = 1;
						NPC.netUpdate = true;
					}

					break;
				}

				//charge around wildly
				case 4:
				{
					NPC.localAI[0]++;
					NPC.localAI[1]++;
					if (NPC.localAI[1] == 2)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath25 with { Pitch = -1.5f }, NPC.Center);

						Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 350f);
					}
					if (NPC.localAI[0] >= 30 && NPC.localAI[0] <= 60)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 15;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}

					if (NPC.localAI[0] >= 60)
					{
						NPC.localAI[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}
			}

			/*
			attack ideas:

			1) Spawn line telegraph that locks onto the player, then stops in place for like a super short time and shoots a super high damage poison sniper bolt
			2) Shoot out spreads of venom darts with line telegraphs, randomized spreads of like 3 to 6 per spread
			3) If the player gets too close, shoot out clouds of poison gas and then retreat away from the player to create distance
			4) If the tail is destroyed, continously try and charge at the player and move super fast
			*/
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//Leg pattern:
			//3 1
			//2 0

			//on death destroy the rest of the legs
			if (NPC.life <= 0)
            {
				legs[0].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[1].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[2].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[3].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

				for (int numGores = 1; numGores <= 6; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CamelColonelGore" + numGores).Type);
					}
				}
			}
		}
	}

	//spider leg code referenced from IDGCaptainRussia94's SGAmod: https://github.com/IDGCaptainRussia94/Terraria-SGAmod/blob/master/NPCs/SpiderQueen/SpiderQueen.cs
	public class CamelColonelLeg
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

		public CamelColonelLeg(Vector2 Startleg, Vector2 BodyPosition, int LegSide)
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
			int LegSegmentLength = 42;
			int LegClawLength = 44;

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
				LegSegmentTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/SpiderWar/CamelColonelLegSegment");
				LegClawTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/SpiderWar/CamelColonelLegFront");

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
					Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/CamelColonelLegSegmentGore").Type);
					Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/CamelColonelLegFrontGore").Type);
				}
			}
		}
	}
}