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
		bool SpawnedBook = false;
		bool EyeClosed = true;

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
			//bools
            writer.Write(SpawnedBook);
			writer.Write(EyeClosed);

            //floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//bools
            SpawnedBook = reader.ReadBoolean();
			EyeClosed = reader.ReadBoolean();

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
			EyeClosedTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardEyeClosed");

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			
			if (EyeClosed && !NPC.IsABestiaryIconDummy)
			{
				Main.EntitySpriteDraw(EyeClosedTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			}
			else
			{
				Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
			}

			return false;
		}

		public override void FindFrame(int frameHeight)
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

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.X * 0.03f;

			if (!SpawnedBook)
			{
				int Book = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (Main.rand.NextBool() ? -100 : 100), (int)NPC.Center.Y, ModContent.NPCType<EyeWizardBook>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Book);
				}

				SpawnedBook = true;

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

				NPC.spriteDirection = NPC.direction;

				NPC.localAI[2]++;

				//passively spawn dust rings below to make it look like its floating with magic
				if (NPC.localAI[2] % 7 == 0)
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
				//fly to the player
				case 1:
				{
					EyeClosed = false;

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

				//use magic book to shoot out bouncing eyes
				case 2:
				{
					EyeClosed = true;

					NPC.velocity *= 0.95f;

					NPC.localAI[0]++;
				
					if (NPC.localAI[0] >= 260)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0]++;
					
						NPC.netUpdate = true;
					}

					break;
				}

				case 3:
				{
					goto case 1;
				}

				//book circles around little eye and shoots out runes
				case 4:
				{
					EyeClosed = true;

					NPC.velocity *= 0.95f;

					NPC.localAI[0]++;

					//go to the player
					if (player.Distance(NPC.Center) > 220f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 4;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.95f;
					}
				
					if (NPC.localAI[0] >= 260)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0]++;
					
						NPC.netUpdate = true;
					}

					break;
				}

				case 5:
				{
					goto case 1;
				}

				//use magic
				case 6:
				{
					EyeClosed = true;

					NPC.velocity *= 0.95f;

					NPC.localAI[0]++;

					//go to the player
					if (player.Distance(NPC.Center) > 220f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 4;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.95f;
					}

					int Repeats = NPC.life <= (NPC.lifeMax / 2) ? 20 : 12;

					//loop 12 times so the book shoots 12 lingering eye runes
					if (NPC.localAI[1] < Repeats)
					{
						if (NPC.localAI[0] >= 32)
						{
							NPC.localAI[1]++;
							NPC.localAI[0] = 0;
						
							NPC.netUpdate = true;
						}
					}
					else
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 1;
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