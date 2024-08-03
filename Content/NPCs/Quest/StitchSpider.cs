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
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Quest
{
	public class StitchSpider : ModNPC
	{
		List<SpiderLeg> legs;

		float MovementSpeed = 0f;

		bool LostLeg1 = false;
		bool LostLeg2 = false;
		bool LostLeg3 = false;
		bool LostLeg4 = false;

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/StitchSpiderBestiary",
                Position = new Vector2(0f, -25f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -20f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 3000;
            NPC.damage = 25;
			NPC.defense = 5;
			NPC.width = 58;
			NPC.height = 58;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath35;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
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

					legs[i].Draw(NPC.Center, NPC.rotation, false, spriteBatch, NPC.whoAmI);
				}
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/StitchSpiderGlow");

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

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
					legs[i].LegUpdate(NPC.Center, NPC.rotation, 128, NPC.velocity * 2);
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.direction = NPC.velocity.X > 0 ? -1 : 1;

			NPC.rotation = NPC.velocity.ToRotation();

            if (NPC.spriteDirection == 1)
            {
				NPC.rotation += MathHelper.Pi;
            }

			UpdateSpiderLegs();

			switch ((int)NPC.ai[0])
			{
				//passive movement before becoming hostile
				case 0:
				{
					if (MovementSpeed > 0.06f)
					{
						MovementSpeed -= 0.1f;
					}
					if (MovementSpeed < 0.05f)
					{
						MovementSpeed += 0.01f;
					}

					Vector2 ChargeDirection = player.Center - NPC.Center;
					ChargeDirection.Normalize();
							
					ChargeDirection *= MovementSpeed;
					NPC.velocity += ChargeDirection / 4;

					NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -5f, 5f);
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -5f, 5f);

					break;
				}

				//chase the player directly
				case 1:
				{
					Vector2 ChargeDirection = player.Center - NPC.Center;
					ChargeDirection.Normalize();
							
					ChargeDirection *= 4f;
					NPC.velocity = ChargeDirection;

					break;
				}

				//go above the player and then charge at the player
				case 2:
				{
					break;
				}

				//go above the player and then charge at the player
				case 3:
				{
					break;
				}

				//shoot webs out of its back that turn back around toward the player
				case 4:
				{
					break;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//Leg pattern:
			//7 3
			//6 2
			//5 1
			//4 0

            if (NPC.life <= (NPC.lifeMax * 0.8f) && !LostLeg1)
            {
				legs[3].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);

				LostLeg1 = true;
			}

			if (NPC.life <= (NPC.lifeMax * 0.6f) && !LostLeg2)
            {
				legs[5].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);

				LostLeg2 = true;
			}

			if (NPC.life <= (NPC.lifeMax * 0.4f) && !LostLeg3)
            {
				legs[7].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);

				LostLeg3 = true;
			}

			if (NPC.life <= (NPC.lifeMax * 0.2f) && !LostLeg4)
            {
				legs[1].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);

				LostLeg4 = true;
			}

			if (NPC.life <= 0)
            {
				legs[0].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);
				legs[2].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);
				legs[4].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);
				legs[6].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC.whoAmI);
			}
		}
	}

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

		public void Draw(Vector2 Position, float Angle, bool gibs, SpriteBatch spriteBatch, int whoAmI)
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

			if (!gibs)
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
				NPC Parent = Main.npc[whoAmI];

				Gore.NewGore(Parent.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore1").Type);
				Gore.NewGore(Parent.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore2").Type);
				Gore.NewGore(Parent.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore3").Type);
				Gore.NewGore(Parent.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/StitchSpiderLegGore4").Type);
			}
		}
	}
}