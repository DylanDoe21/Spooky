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
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.NPCs.SpiderCave.Projectiles;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class EmperorMortar : ModNPC
	{
		bool SpawnedSegments = false;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/EmperorMortarBestiary",
				Position = new Vector2(0f, -75f),
				PortraitPositionXOverride = 0f,
              	PortraitPositionYOverride = -60f
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//bools
			writer.Write(SpawnedSegments);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//bools
			SpawnedSegments = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 15000;
            NPC.damage = 50;
			NPC.defense = 40;
			NPC.width = 104;
			NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.DD2_SkeletonHurt with { Pitch = -1f, Volume = 0.65f };
			NPC.DeathSound = SoundID.NPCDeath27 with { Pitch = -0.65f };
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EmperorMortar"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

			if (NPC.velocity.X != 0)
			{
				if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 1 * frameHeight;
				}
			}
			else
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				if (!SpawnedSegments)
				{
					NPC.realLife = NPC.whoAmI;
					int latestNPC = NPC.whoAmI;
					for (int numSegments = 0; numSegments < 6; numSegments++)
					{
						latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<EmperorMortarSegment>(), NPC.whoAmI, 0, latestNPC);
						Main.npc[latestNPC].lifeMax = NPC.lifeMax;
						Main.npc[latestNPC].realLife = NPC.whoAmI;
						Main.npc[latestNPC].ai[2] = numSegments;
						Main.npc[latestNPC].ai[3] = NPC.whoAmI;
						NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
					}

					SpawnedSegments = true;
					NPC.netUpdate = true;
				}
			}

			float SpeedModifier = 3.5f;
			bool SlowDown = false;

			int CollideWidth = 80;
			int CollideHeight = 20;
			Vector2 NPCCollisionPos = new Vector2(NPC.Center.X - 40, NPC.position.Y + (float)NPC.height - 20);

			if (Math.Abs(NPC.Center.X - player.Center.X) < 50f || NPC.ai[0] == 1 || NPC.ai[0] == 2)
			{
				SlowDown = true;
			}
			if (SlowDown)
			{
				NPC.velocity.X *= 0.9f;
				if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
				{
					NPC.velocity.X = 0f;
				}

				if (Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight))
				{
					if (NPC.velocity.Y > 0f)
					{
						NPC.velocity.Y = 0f;
					}
					if (NPC.velocity.Y > -0.2)
					{
						NPC.velocity.Y -= 0.025f;
					}
					else
					{
						NPC.velocity.Y -= 0.2f;
					}
					if (NPC.velocity.Y < -10f)
					{
						NPC.velocity.Y = -10f;
					}
				}
				else
				{
					if (NPC.velocity.Y < 0f)
					{
						NPC.velocity.Y = 0f;
					}
					if (NPC.velocity.Y < 0.1)
					{
						NPC.velocity.Y += 0.025f;
					}
					else
					{
						NPC.velocity.Y += 0.25f;
					}
				}
				if (NPC.velocity.Y > 10f)
				{
					NPC.velocity.Y = 10f;
				}
			}
			else
			{
				if (NPC.direction > 0)
				{
					NPC.velocity.X = (NPC.velocity.X * 20f + SpeedModifier) / 21f;
				}
				if (NPC.direction < 0)
				{
					NPC.velocity.X = (NPC.velocity.X * 20f - SpeedModifier) / 21f;
				}

				bool IncreaseFallSpeed = false;
				bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

				if (NPC.position.X < player.position.X && NPC.position.X + (float)NPC.width > player.position.X + (float)player.width &&
				NPC.position.Y + (float)NPC.height < player.position.Y + (float)player.height - 16f)
				{
					IncreaseFallSpeed = true;
				}
				if (IncreaseFallSpeed)
				{
					NPC.velocity.Y += 0.5f;
				}
				else if ((!Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight) && !HasLineOfSight) ||
				(Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight) && !HasLineOfSight && NPC.Bottom.Y < player.Top.Y))
				{
					Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 10;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Bottom + (NPC.velocity * 5), Vector2.Zero, ModContent.ProjectileType<MortarWebTrail>(), 0, 0f, ai0: NPC.velocity.ToRotation());
				}
				else if (Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight))
				{
					if (NPC.velocity.Y > 0f)
					{
						NPC.velocity.Y = 0f;
					}
					if (NPC.velocity.Y > -0.2)
					{
						NPC.velocity.Y -= 0.025f;
					}
					else
					{
						NPC.velocity.Y -= 0.2f;
					}
					if (NPC.velocity.Y < -10f)
					{
						NPC.velocity.Y = -10f;
					}
				}
				else
				{
					if (NPC.velocity.Y < 0f)
					{
						NPC.velocity.Y = 0f;
					}
					if (NPC.velocity.Y < 0.1)
					{
						NPC.velocity.Y += 0.025f;
					}
					else
					{
						NPC.velocity.Y += 0.25f;
					}
				}
				if (NPC.velocity.Y > 10f)
				{
					NPC.velocity.Y = 10f;
				}
			}

			//most of the actual behavior here is handled in the emperor mortar segments
			switch ((int)NPC.ai[0])
			{
				//walk at player
				case 0:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0] = player.Distance(NPC.Center) <= 350f ? 3 : (Main.rand.NextBool() ? 1 : 2);
						NPC.netUpdate = true;
					}

					break;
				}

				//shoot out web bombs from the bigger holes toward the bottom
				case 1:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//shoot out rockets from the smaller holes in the middle
				case 2:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//erupt flamethrowers from the eye holes at the top, directly at the player
				case 3:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] >= 180)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
			npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderWarItemDropCondition(), ModContent.ItemType<EmperorMortarTrophyItem>()));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				if (SpiderWarWorld.SpiderWarActive)
				{
					SpiderWarWorld.SpiderWarPoints++;
				}

				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<SpotlightFirefly>() && npc.ai[3] == NPC.whoAmI)
					{
						npc.ai[0]++;
					}
				}

				for (int numGores = 1; numGores <= 7; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EmperorMortarGore" + numGores).Type);
					}
				}
            }
        }
	}
}