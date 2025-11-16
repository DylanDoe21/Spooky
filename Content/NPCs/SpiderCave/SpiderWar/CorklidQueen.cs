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

using Spooky.Core;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class CorklidQueen : ModNPC
	{
		int CurrentFrameX = 0; //0 = pop out of ground animation  1 = walking animation

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 30000;
            NPC.damage = 50;
			NPC.defense = 30;
			NPC.width = 124;
			NPC.height = 118;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath32;
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

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
			{
				NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
			}

			NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

			if (NPC.ai[0] == 0)
			{
				if (NPC.ai[2] == 0)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
				else
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 5)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 6)
					{
						NPC.frame.Y = 5 * frameHeight;
					}
				}
			}
			else
			{
				NPC.frameCounter++;

				if (NPC.velocity.X != 0)
				{
					if (NPC.frameCounter > 10 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
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
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			if (NPC.ai[0] == 0)
			{
				CurrentFrameX = 0;

				NPC.noGravity = false;
				NPC.noTileCollide = false;

				if (player.Distance(NPC.Center) < 300f)
				{
					NPC.ai[1]++;
					NPC.netUpdate = true;
				}

				if (NPC.ai[1] > 0)
				{
					NPC.ai[2]++;
					if (NPC.ai[2] > 35)
					{
						Screenshake.ShakeScreenWithIntensity(NPC.Center, 12f, 750f);

						NPC.ai[0]++;
						NPC.netUpdate = true;
					}
				}
			}
			else
			{
				CurrentFrameX = 1;

				NPC.noGravity = true;
				NPC.noTileCollide = true;

				float SpeedModifier = 6f;
				bool SlowDown = false;

				if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
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
				}
				
				int CollideWidth = 80;
				int CollideHeight = 20;
				Vector2 NPCCollisionPos = new Vector2(NPC.Center.X - 40, NPC.position.Y + (float)NPC.height - 20);
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
					Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 15;
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

			/*
			attack ideas:

			1) If the player gets out of line of sight, go into ground, re-emerge near player, and then jump out and fling damaging debris everywhere
			2) Erupt flaming web bolts upward that go up super high, then fall down slowly and you have to weave inbetween them to dodge them
			3) Shoot out giant homing missile that tracks player for a while and then makes a massive damaging explosion
			*/
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
            }
        }
	}
}