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

using Spooky.Content.Items.Minibiomes.Desert;
using Spooky.Content.Items.Pets;

namespace Spooky.Content.NPCs.Minibiomes.Desert
{
	public class OpalTarCrawler : ModNPC
	{
		List<CrawlerLeg> legs;

		float IdleSpeed = 5f;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OpalTarCrawlerBestiary",
				Position = new Vector2(-40f, 0f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f
			};

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 1200;
            NPC.damage = 55;
			NPC.defense = 15;
			NPC.width = 72;
			NPC.height = 72;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OpalTarCrawler"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (legs != null)
			{
				//when npc is default rotation, legs look like this:
				//0      1
				//	body 
				//2      3

				legs[0].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegTop1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegTop2"),
				24, 54, NPC.Center, NPC.rotation, spriteBatch);

				legs[1].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegTop1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegTop2"),
				26, 54, NPC.Center, NPC.rotation, spriteBatch);

				legs[2].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegBottom1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegBottom2"),
				42, 54, NPC.Center, NPC.rotation, spriteBatch);

				legs[3].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegBottom1"), 
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegBottom2"),
				34, 48, NPC.Center, NPC.rotation, spriteBatch);
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public void UpdateLegs()
		{
			if (legs == null)
			{
				legs = new List<CrawlerLeg>();
				Vector2[] Origin = { new Vector2(-12, 6), new Vector2(25, 17) }; //this is for the origin point of the leg relative to the npc
				Vector2[] Destination = { new Vector2(25, 45), new Vector2(120, 60) }; //this is the destination where the very bottom point of the leg should go when it moves

				Vector2[] Origin2 = { new Vector2(0, 0), new Vector2(7, 12) };
				Vector2[] Destination2 = { new Vector2(25, 45), new Vector2(120, 60) };

				for (int numLegs = -1; numLegs <= 1; numLegs += 2)
				{
					for (int i = 0; i < Destination.Length; i++)
					{
						if (numLegs == -1)
						{
							legs.Add(new CrawlerLeg(new Vector2(Destination[i].X, Destination[i].Y * numLegs), new Vector2(Origin[i].X, Origin[i].Y * numLegs), i != 1 ? -numLegs : numLegs));
						}
						else
						{
							legs.Add(new CrawlerLeg(new Vector2(Destination2[i].X, Destination2[i].Y * numLegs), new Vector2(Origin2[i].X, Origin2[i].Y * numLegs), i != 1 ? -numLegs : numLegs));
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < legs.Count; i++)
				{
					legs[i].LegUpdate(NPC.Center, NPC.rotation, 128, NPC.velocity / 25);
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
			
			/*
			Vector2 RotateTowards = player.Center - NPC.Center;

			float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
			float RotateSpeed = 0.035f;

			NPC.rotation = NPC.rotation.AngleTowards(RotateDirection + MathHelper.PiOver2, RotateSpeed);
			*/

			//rotation stuff
            float RotateDirection = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X);
			float RotateSpeed = 0.1f;

            NPC.rotation = NPC.rotation.AngleTowards(RotateDirection, RotateSpeed);

			UpdateLegs();

			if (player.Distance(NPC.Center) >= 250f)
			{
				if (IdleSpeed < 15)
				{
					IdleSpeed += 0.05f;
				}
			}
			else
			{
				if (IdleSpeed > 5f)
				{
					IdleSpeed -= 0.1f;
				}
			}

			Vector2 MoveSpeed = player.Center - NPC.Center;
			MoveSpeed.Normalize();

			Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * IdleSpeed;
			NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
		}

		//Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            int[] DropList = new int[] 
            { 
                ModContent.ItemType<SabertoothScythe>(),
                ModContent.ItemType<HelicoprionSaw>(),
                ModContent.ItemType<TitanoboaWhip>(),
				ModContent.ItemType<LongisquamaWings>(),
                ModContent.ItemType<HallucigeniaSpine>()
            };

            npcLoot.Add(ItemDropRule.OneFromOptions(1, DropList));

			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScalyHockeyStick>(), 20));
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0)
            {
				for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

				legs[0].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegTop1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegTop2"),
				24, 54, NPC.Center, NPC.rotation, null, true, 0, NPC);
				legs[1].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegTop1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegTop2"),
				26, 54, NPC.Center, NPC.rotation, null, true, 1, NPC);
				legs[2].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegBottom1"),
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/BackLegBottom2"),
				42, 54, NPC.Center, NPC.rotation, null, true, 2, NPC);
				legs[3].Draw(ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegBottom1"), 
				ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Desert/CrawlerLegs/FrontLegBottom2"),
				34, 48, NPC.Center, NPC.rotation, null, true, 3, NPC);

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerGore" + numGores).Type);
                    }
                }

				for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerSpineGore" + Main.rand.Next(1, 3)).Type);
                    }
                }
			}
		}
	}

	//spider leg code referenced from IDGCaptainRussia94's SGAmod: https://github.com/IDGCaptainRussia94/Terraria-SGAmod/blob/master/NPCs/SpiderQueen/SpiderQueen.cs
	public class CrawlerLeg
	{
		int LegSide;

		float MaxLegDistance;
		float LerpValue = 1f;

		Vector2 BodyPosition;
		Vector2 LegPosition;
		Vector2 PreviousLegPosition;
		Vector2 CurrentLegPosition;
		Vector2 DesiredLegPosition;

		public CrawlerLeg(Vector2 Startleg, Vector2 BodyPosition, int LegSide)
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
			this.MaxLegDistance = MaxLegDistance;
			
			float dev = 1.5f;
			float forward = Math.Abs((Velocity.Length() - 4f) * 8f) * (DesiredLegPosition.X > -0 ? DesiredLegPosition.X / 200f : 1f);

			Vector2 Destination = Position + (new Vector2(forward, 0f) + DesiredLegPosition).RotatedBy(Angle);

			LerpValue += (1f - LerpValue) / dev;
			LegPosition = Vector2.Lerp(PreviousLegPosition, CurrentLegPosition, LerpValue);

			if ((LegPosition - Destination).Length() > (MaxLegDistance + ((dev - 4f) * 16f)) + 74)
			{
				PreviousLegPosition = LegPosition;
				CurrentLegPosition = Destination; //+ new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
				LerpValue = 0f;
			}
		}

		public void Draw(Asset<Texture2D> LegTexture1, Asset<Texture2D> LegTexture2, int LegSegmentLength1, int LegSegmentLength2, Vector2 Position, float Angle, SpriteBatch spriteBatch, 
		bool SpawnGore = false, int GoreLeg = 0, NPC NPC = null)
		{
			Vector2 start = Position + BodyPosition.RotatedBy(Angle);

			Vector2 middle = LegPosition - start;

			float angleleg1 = (LegPosition - start).ToRotation() + (MathHelper.Clamp((MathHelper.Pi / 2f) - MathHelper.ToRadians(middle.Length()), MathHelper.Pi / 12f, MathHelper.Pi / 2f) * LegSide);

			Vector2 legdist = angleleg1.ToRotationVector2();
			legdist.Normalize();
			Vector2 halfway1 = legdist;
			legdist *= LegSegmentLength1 - 3;

			Vector2 leg2 = (Position + BodyPosition.RotatedBy(Angle)) + legdist;

			float angleleg2 = (LegPosition - leg2).ToRotation();

			halfway1 *= LegSegmentLength1 / 2;
			Vector2 halfway2 = leg2 + (angleleg2.ToRotationVector2() * LegSegmentLength2 / 2);

			if (!SpawnGore)
			{
				//first leg segment
				Color LightColor = Lighting.GetColor((int)((start.X + halfway1.X) / 16f), (int)((start.Y + halfway1.Y) / 16f));
				spriteBatch.Draw(LegTexture1.Value, start - Main.screenPosition, null, LightColor, angleleg1, new Vector2(4, LegTexture1.Height() / 2f), 1f, SpriteEffects.None, 0f);

				//second leg segment
				LightColor = Lighting.GetColor((int)(halfway2.X / 16f), (int)(halfway2.Y / 16f));
				spriteBatch.Draw(LegTexture2.Value, leg2 - Main.screenPosition, null, LightColor, angleleg2, new Vector2(4, LegTexture2.Height() / 2f), 1f, SpriteEffects.None, 0f);
			}
			else
			{
				switch (GoreLeg)
				{
					case 0:
					{
						if (Main.netMode != NetmodeID.Server) 
                    	{
							Gore.NewGore(NPC.GetSource_Death(), start, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerBackLegTopGore").Type);
						}
						break;
					}
					case 1:
					{
						if (Main.netMode != NetmodeID.Server) 
                    	{
							Gore.NewGore(NPC.GetSource_Death(), start, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerFrontLegTopGore1").Type);
							Gore.NewGore(NPC.GetSource_Death(), leg2, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerFrontLegTopGore2").Type);
						}
						break;
					}
					case 2:
					{
						if (Main.netMode != NetmodeID.Server) 
                    	{
							Gore.NewGore(NPC.GetSource_Death(), start, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerBackLegBottomGore").Type);
						}
						break;
					}
					case 3:
					{
						if (Main.netMode != NetmodeID.Server) 
                    	{
							Gore.NewGore(NPC.GetSource_Death(), start, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerFrontLegBottomGore1").Type);
							Gore.NewGore(NPC.GetSource_Death(), leg2, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarCrawlerFrontLegBottomGore2").Type);
						}
						break;
					}
				}
			}
		}
	}
}