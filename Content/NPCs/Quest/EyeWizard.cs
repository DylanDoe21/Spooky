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

using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class EyeWizard : ModNPC
	{
		int CurrentFrameX = 0; //0 = idle flying animation  1 = go inside cloak

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 3000;
            NPC.damage = 25;
			NPC.defense = 5;
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
			NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossAdjustment);
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

			var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

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

			if (NPC.ai[0] == 0)
			{
				NPC.noGravity = false;
				NPC.noTileCollide = false;
			}
			else
			{
				NPC.noGravity = true;
				NPC.noTileCollide = true;
			}

			switch ((int)NPC.ai[0])
			{
				//passive staying still (nothing needs to be here)
				case 0:
				{
					break;
				}

				//fly towards the player 
				case 1:
				{
					NPC.localAI[0]++;

					//go to a random location
					if (NPC.localAI[0] < 120)
					{
						Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 250);

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 15);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
					}

					if (NPC.localAI[0] >= 120)
					{
						NPC.velocity *= 0.85f;
					}

					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0]++;
					}
					
					break;
				}

				//fly towards the player 
				case 2:
				{
					break;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//become aggressive on hit
			if (NPC.ai[0] == 0)
			{
				NPC.ai[0]++;

				NPC.netUpdate = true;
			}
		}
	}
}