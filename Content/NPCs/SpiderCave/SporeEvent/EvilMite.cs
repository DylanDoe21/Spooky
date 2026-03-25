using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Armor;
using Spooky.Content.NPCs.SpiderCave.Projectiles;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
	public class EvilMite : ModNPC
	{
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(20f, 15f)
            };

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
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
            NPC.lifeMax = 3000;
            NPC.damage = 55;
            NPC.defense = 0;
			NPC.width = 200;
			NPC.height = 96;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 10, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit20 with { Pitch = -1f };
			NPC.DeathSound = SoundID.NPCDeath23 with { Pitch = -0.65f };
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EvilMite"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
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
				if (NPC.frame.Y >= frameHeight * 6)
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
				else if (Collision.SolidCollision(NPCCollisionPos, CollideWidth, CollideHeight) && !HasLineOfSight && NPC.Bottom.Y < player.Top.Y)
				{
					Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 10;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
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
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					int Death = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EvilMiteDeath>());
					Main.npc[Death].direction = NPC.direction;
					Main.npc[Death].velocity = NPC.velocity;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: Death);
					}
				}

				for (int numGores = 1; numGores <= 6; numGores++)
                {
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EvilMiteGore" + numGores).Type);
					}
				}
            }
        }
	}
}