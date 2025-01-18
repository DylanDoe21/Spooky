using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.SpookFishron.Projectiles;

namespace Spooky.Content.NPCs.Boss.SpookFishron
{
	public class SpookSharkron : ModNPC
	{
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.TrailCacheLength[NPC.type] = 7;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Position = new Vector2(32f, 0f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 0f
			};
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 450;
			NPC.damage = 100;
			NPC.defense = 0;
			NPC.width = 118;
			NPC.height = 46;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SpookSharkron"),
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions.PumpkinMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookFishron_Background", Color.White)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//after images
			for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
			{
				Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
				Color color = NPC.GetAlpha(Color.OrangeRed) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
				spriteBatch.Draw(NPCTexture.Value, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
			}

			//draw aura
			for (int i = 0; i < 360; i += 30)
			{
				Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.OrangeRed, Color.Orange, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5f), 0).RotatedBy(MathHelper.ToRadians(i));

				Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.05f, effects, 0);
			}

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter > 6)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 4)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override void AI()
		{
			int num1063 = 90;
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
			{
				NPC.TargetClosest(false);
				NPC.direction = 1;
				NPC.netUpdate = true;
			}
			if (NPC.ai[0] == 0f)
			{
				NPC.ai[1]++;
				NPC.dontTakeDamage = true;

				float num1064 = (float)Math.PI / 30f;
				float num1065 = NPC.ai[2] - (NPC.ai[2] / 2) - (NPC.width / 2) - 5;
				float num1066 = (float)(Math.Cos(num1064 * NPC.localAI[1])) * num1065;
				NPC.position.X -= num1066 * (float)(-NPC.direction);
				NPC.localAI[1]++;
				num1066 = (float)(Math.Cos(num1064 * NPC.localAI[1])) * num1065;
				NPC.position.X += num1066 * (float)(-NPC.direction);

				if (Math.Abs(Math.Cos(num1064 * NPC.localAI[1])) > 0.25)
				{
					NPC.spriteDirection = ((!(Math.Cos(num1064 * NPC.localAI[1]) - 0.5 >= 0.0)) ? 1 : (-1));
				}

				NPC.rotation = NPC.velocity.Y * (float)NPC.spriteDirection * 0.1f;

				if ((double)NPC.rotation < -0.2)
				{
					NPC.rotation = -0.2f;
				}
				if ((double)NPC.rotation > 0.2)
				{
					NPC.rotation = 0.2f;
				}

				NPC.alpha -= 6;
				if (NPC.alpha < 0)
				{
					NPC.alpha = 0;
				}

				if (NPC.ai[1] >= (float)num1063)
				{
					NPC.ai[0] = 1f;
					NPC.ai[1] = 0f;
					if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
					{
						NPC.ai[1] = 1f;
					}
					SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { Volume = 0.5f }, NPC.Center);
					NPC.TargetClosest();
					NPC.spriteDirection = NPC.direction;

					Vector2 ChargeSpeed = (Main.player[NPC.target].Center + Main.player[NPC.target].velocity * 30f) - NPC.Center;
					ChargeSpeed.Normalize();
					NPC.velocity = ChargeSpeed * 20f;

					NPC.rotation = NPC.velocity.ToRotation();

					if (NPC.direction == -1)
					{
						NPC.rotation += (float)Math.PI;
					}

					NPC.netUpdate = true;
				}
			}
			else
			{
				if (NPC.ai[0] != 1f)
				{
					return;
				}

				if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
				{
					if (NPC.ai[1] < 1f)
					{
						NPC.ai[1] = 1f;
					}
				}
				else
				{
					NPC.alpha -= 15;
					if (NPC.alpha < 150)
					{
						NPC.alpha = 150;
					}
				}

				if (NPC.ai[1] >= 1f)
				{
					NPC.alpha -= 60;
					if (NPC.alpha < 0)
					{
						NPC.alpha = 0;
					}

					NPC.dontTakeDamage = false;
					NPC.ai[1]++;

					int NPCCenterX = (int)(NPC.Center.X / 16f);
					int NPCCenterY = (int)(NPC.Center.Y / 16f);
					Tile tile = Main.tile[NPCCenterX, NPCCenterY];

					if (tile == null || !WorldGen.InWorld(NPCCenterX, NPCCenterY))
					{
						tile = new Tile();
					}

					//kill the npc if it touches water or collides with a solid tile
					if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || tile.LiquidAmount > 0)
					{
						NPC.life = 0;
						NPC.HitEffect();
						NPC.active = false;

						return;
					}
				}
				
				if (NPC.ai[1] >= 60f && NPC.velocity.Y < 15f)
				{
					NPC.velocity.Y = NPC.velocity.Y + 0.25f;
				}

				NPC.rotation = NPC.velocity.ToRotation();

				if (NPC.direction == -1)
				{
					NPC.rotation += (float)Math.PI;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
		{
			if (NPC.life <= 0) 
			{
				SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion, NPC.Center);

				NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SharkronExplosion>(), NPC.damage, 4.5f, 1);

				for (int numProjs = 0; numProjs < 3; numProjs++)
				{
					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-25, -12)), ModContent.ProjectileType<SharkronFireball>(), NPC.damage / 2, 4.5f, 1);
				}

				for (int numGores = 1; numGores <= 3; numGores++)
				{
					if (Main.netMode != NetmodeID.Server) 
					{
						Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.Center.X, NPC.Center.Y - 15), NPC.velocity, ModContent.Find<ModGore>("Spooky/SpookSharkronGore" + numGores).Type);
					}
				}
			}
		}
	}
}