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

namespace Spooky.Content.NPCs.SpiderCave
{
	public class FishingSpider : ModNPC
	{
		List<FishingSpiderLeg> legs;

		Vector2 SavePosition = Vector2.Zero;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
        {
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/FishingSpiderBestiary"
            };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 350;
            NPC.damage = 45;
            NPC.defense = 18;
			NPC.width = 38;
			NPC.height = 38;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit22 with { Pitch = -0.65f };
			NPC.DeathSound = SoundID.NPCDeath31 with { Pitch = -0.5f };
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.FishingSpider"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (legs != null)
			{
				for (int i = 0; i < legs.Count; i++)
				{
					legs[i].Draw(NPC.Center, NPC.rotation, false, spriteBatch);
				}
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(Color.White)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public void UpdateSpiderLegs()
		{
			if (legs == null)
			{
				legs = new List<FishingSpiderLeg>();
				Vector2[] Origin = { new Vector2(-11, -7)  }; //this is for the origin point of the leg relative to the npc
				Vector2[] Destination = { new Vector2(90, -85) }; //this is the destination where the very bottom point of the leg should go when it moves

				for (int numLegs = -1; numLegs <= 1; numLegs += 2)
				{
					for (int i = 0; i < Destination.Length; i++)
					{
						legs.Add(new FishingSpiderLeg(new Vector2(Destination[i].X, Destination[i].Y * numLegs), new Vector2(Origin[i].X, Origin[i].Y * numLegs), numLegs));
					}
				}
			}
			else
			{
				for (int i = 0; i < legs.Count; i++)
				{
					legs[i].LegUpdate(NPC.Center, NPC.rotation, 80, NPC.velocity);
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.rotation = NPC.velocity.ToRotation();

			UpdateSpiderLegs();

			SavePosition = Vector2.Zero;

			Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 7;
			NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//Leg pattern:
			//1 0

			//on death destroy the rest of the legs
			if (NPC.life <= 0)
            {
				legs[0].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);
				legs[1].Draw(NPC.Center, NPC.rotation, true, Main.spriteBatch, NPC);

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FishingSpiderGore" + numGores).Type);
                    }
                }
			}
		}
	}

	//spider leg code referenced from IDGCaptainRussia94's SGAmod: https://github.com/IDGCaptainRussia94/Terraria-SGAmod/blob/master/NPCs/SpiderQueen/SpiderQueen.cs
	public class FishingSpiderLeg
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

		public FishingSpiderLeg(Vector2 Startleg, Vector2 BodyPosition, int LegSide)
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
			int LegSegmentLength = 40;
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
				LegSegmentTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/FishingSpiderLegSegment");
				LegClawTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/FishingSpiderLegFront");

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
					Gore.NewGore(NPC.GetSource_Death(), start, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/FishingSpiderLegGore1").Type);
					Gore.NewGore(NPC.GetSource_Death(), halfway2, new Vector2(Main.rand.Next(-2, 3), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/FishingSpiderLegGore2").Type);
				}
			}
		}
	}
}