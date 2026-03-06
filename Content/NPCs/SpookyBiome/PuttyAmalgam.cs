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

using Spooky.Core;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttyAmalgam : ModNPC
	{
		public ushort destinationX = 0;
		public ushort destinationY = 0;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 2500;
            NPC.damage = 55;
            NPC.defense = 0;
            NPC.width = 110;
            NPC.height = 34;
            NPC.npcSlots = 2f;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttyAmalgam"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
			if (NPC.ai[1] <= 0)
			{
				if (NPCGlobalHelper.IsCollidingWithFloor(NPC, true) || NPC.IsABestiaryIconDummy)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 4)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}
					if (NPC.frame.Y >= frameHeight * 2)
					{
						NPC.frame.Y = 0 * frameHeight;
					}
				}
				else
				{
					NPC.frame.Y = 2 * frameHeight;
				}
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 4)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}

				if (NPC.frame.Y < frameHeight * 4)
				{
					NPC.frame.Y = 3 * frameHeight;
				}

				if (NPC.frame.Y >= frameHeight * 7)
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
			if (!HasLineOfSight && NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
			{
				NPC.ai[3] = 1;
			}

			if (NPC.ai[3] > 0)
			{
				TeleportToPlayer(player);
			}

			if (NPC.ai[1] <= 0)
			{
				NPC.localAI[0]++;

				//jumping velocity
				Vector2 JumpTo = new Vector2(NPC.Center.X + (NPC.Center.X > player.Center.X ? -300 : 300), NPC.Center.Y - Main.rand.Next(350, 451));

				Vector2 velocity = JumpTo - NPC.Center;

				//actual jumping
				if (NPC.localAI[0] == 60)
				{
					NPC.aiStyle = -1;

					SoundEngine.PlaySound(SoundID.GlommerBounce with { Pitch = -1.2f }, NPC.Center);

					float speed = MathHelper.Clamp(velocity.Length() / 36, 10, 25);
					velocity.Normalize();
					velocity.Y -= 0.22f;
					velocity.X *= 1.05f;
					NPC.velocity = velocity * speed * 1.1f;

					NPC.netUpdate = true;
				}

				//fall down a bit before slamming
				if (NPC.localAI[0] > 60 && NPC.localAI[0] < 115)
				{
					NPC.velocity.Y += 0.5f;
				}

				//lower velocity before and while slaming down
				if (NPC.localAI[0] > 90)
				{
					NPC.velocity.Y += 0.25f;
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0f, 20f);

					NPC.velocity.X *= 0.95f;

					if (NPC.localAI[0] < 115 && NPC.Center.X <= player.Center.X + 3 && NPC.Center.X >= player.Center.X - 3)
					{
						NPC.localAI[0] = 115;
					}
				}

				//slam down
				if (NPC.localAI[0] == 115)
				{
					NPC.noGravity = true;
				}

				//set tile collide to true once it gets to the players level to prevent cheesing
				if (NPC.localAI[0] >= 115)
				{
					NPC.velocity.Y += 0.5f;
					NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0f, 20f);
				}

				//slam the ground
				if (NPC.localAI[0] >= 65 && NPC.localAI[1] == 0 && NPCGlobalHelper.IsCollidingWithFloor(NPC, true))
				{
					NPC.noGravity = false;

					NPC.velocity.X = 0;

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 250f);

					SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Pitch = 1.2f }, NPC.Center);

					for (int numProjs = 0; numProjs <= 6; numProjs++)
					{
						Vector2 Velocity = new Vector2(0, Main.rand.Next(-13, -4)).RotatedByRandom(MathHelper.ToRadians(50));
						NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-30, 31), NPC.Bottom.Y), Velocity, 
						ModContent.ProjectileType<PuttyBlob>(), NPC.damage, 4.5f, ai0: Main.rand.Next(0, 3));
					}
					
					//complete the slam attack
					NPC.localAI[1] = NPC.localAI[0];
				}

				//only loop attack if the jump has been completed
				if (NPC.localAI[0] >= NPC.localAI[1] + 20 && NPC.localAI[1] > 0)
				{
					NPC.localAI[0] = 30;
					NPC.localAI[1] = 0;

					NPC.netUpdate = true;
				}
			}
		}

		public void TeleportToPlayer(Player player)
		{
			NPC.velocity.X = 0;

			NPC.ai[1]++;
			if (NPC.ai[1] >= 30)
			{
				if (NPC.ai[1] >= 30 && destinationX == 0 && destinationY == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Point point = player.Center.ToTileCoordinates();
					Vector2 chosenTile = Vector2.Zero;
					if (NPCGlobalHelper.TeleportToSpot(NPC, player, ref chosenTile, point.X, point.Y, 55, 7, true))
					{
						destinationX = (ushort)chosenTile.X;
						destinationY = (ushort)chosenTile.Y;
						NPC.netUpdate = true;
					}
				}

				if (destinationX != 0 && destinationY != 0)
				{
					NPC.ai[2]++;
					if (NPC.ai[2] <= 10)
					{
						Dust dust = Dust.NewDustDirect(new Vector2((destinationX * 16f) - 30, (destinationY * 16f) - 20), NPC.width, NPC.height, DustID.Mud, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, Color.White, 2.5f);
						dust.color = Color.White;
						dust.noGravity = true;
					}
					else
					{
						NPC.position.X = destinationX * 16f - (float)(NPC.width / 2) + 8f;
						NPC.position.Y = destinationY * 16f - (float)NPC.height;
						NPC.velocity = Vector2.Zero;
						NPC.netOffset *= 0f;
						destinationX = 0;
						destinationY = 0;
						NPC.ai[1] = 0;
						NPC.ai[2] = 0;
						NPC.ai[3] = 0;
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;

						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with { Volume = 0.5f }, NPC.Center);
						Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 300f);

						NPC.netUpdate = true;
					}
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
            }
		}
    }
}