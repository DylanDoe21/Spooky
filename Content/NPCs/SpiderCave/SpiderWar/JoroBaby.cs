using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class JoroBaby : ModNPC
	{
		float FlashOpacity = 0f;
		float Timer1 = 0f;
		float Timer2 = 0f;
		float Timer3 = 0f;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> FlashTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Position = new Vector2(10f, 0f),
              	PortraitPositionXOverride = 0f,
              	PortraitPositionYOverride = 0f
            };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 3500;
            NPC.damage = 50;
			NPC.defense = 10;
			NPC.width = 74;
			NPC.height = 50;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.4f };
			NPC.DeathSound = SoundID.NPCDeath36 with { Pitch = -1f };
            NPC.aiStyle = 22;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
		}

		public override bool CheckActive()
		{
			return !SpiderWarWorld.SpiderWarActive;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JoroBaby"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
			NPC.frameCounter++;
			if (NPC.frameCounter > 2)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 6)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (NPC.IsABestiaryIconDummy)
			{
                return true;
            }

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
			AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");
			FlashTexture ??= ModContent.Request<Texture2D>(Texture + "Flash");

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 vector = NPC.Center - screenPos;

			if (Timer1 >= 300 && Timer1 <= 420)
			{
				Color AuraColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.DarkRed);

				float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

				Rectangle AuraFrame = new Rectangle(0, 0, 42, 42);
				Vector2 drawOriginAura = new Vector2(AuraTexture.Width() * 0.5f, AuraTexture.Height() * 0.5f);

                for (int i = 0; i < 360; i += 30)
                {
                    Vector2 circular = Vector2.One.RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, AuraFrame, AuraColor * 0.1f, NPC.rotation + i, drawOriginAura, Timer2 / 37 + (Timer2 < 420 ? time : time2), SpriteEffects.None, 0);
                }
			}

			//npc texture
			Main.EntitySpriteDraw(NPCTexture.Value, vector, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			//Main.EntitySpriteDraw(GlowTexture.Value, vector, NPC.frame, NPC.GetAlpha(Color.White * 0.5f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			//flashing texture when exploding
			if (FlashOpacity > 0f)
            {
                Main.EntitySpriteDraw(FlashTexture.Value, vector, NPC.frame, Color.White * FlashOpacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            }

			return false;
		}

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			NPC.rotation = NPC.velocity.X * 0.015f;

			if (FlashOpacity > 0f)
            {
                FlashOpacity -= 0.025f;
            }

			Timer1++;
			if (Timer1 >= 300)
			{
				NPC.velocity *= 0.75f;

				if (Timer2 < 420)
                {
                    Timer2 += 5;
                }
				else
				{
					Timer3++;
					if (Timer3 % 15 == 0)
					{
						FlashOpacity = 1f;
					}
				}

				if (Timer1 >= 420)
				{
					SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.25f }, NPC.Center);

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 10f, 400f);

					float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

					int Damage = Main.masterMode ? 300 : Main.expertMode ? 200 : 100;

					foreach (var activePlayer in Main.ActivePlayers)
					{
						if (!activePlayer.dead && activePlayer.Distance(NPC.Center) <= Timer2 * 0.65f + time)
						{
							activePlayer.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.CorklidNuke").ToNetworkText(activePlayer.name)), Damage + Main.rand.Next(-10, 30), 0);
						}
					}

					float maxAmount = 25;
					int currentAmount = 0;
					while (currentAmount <= maxAmount)
					{
						Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
						Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
						float intensity = Main.rand.NextFloat(2f, 25f);

						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
						vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

						int Fire = Dust.NewDust(NPC.Center, 0, 0, DustID.InfernoFork, 0f, 0f, 100, default, 5f);
						Main.dust[Fire].noGravity = true;
						Main.dust[Fire].position = NPC.Center + vector12;
						Main.dust[Fire].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

						if (currentAmount % 2 == 0)
						{
							int Smoke = Dust.NewDust(NPC.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.5f, 2f));
							Main.dust[Smoke].noGravity = true;
							Main.dust[Smoke].position = NPC.Center + vector12;
							Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;
						}

						currentAmount++;
					}

					Timer1 = 0;
					Timer2 = 0;
					Timer3 = 0;
					NPC.netUpdate = true;
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
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JoroBabyGore" + numGores).Type);
					}
				}
            }
        }
	}
}