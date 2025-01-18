using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Vegetable.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
	public class Eggplant : ModNPC
	{
		int MoveSpeedX;
        int MoveSpeedY;

		private static Asset<Texture2D> ChainTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/EggplantBestiary",
				Position = new Vector2(0f, -20f)
			};

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 100;
			NPC.damage = 30;
			NPC.defense = 10;
			NPC.width = 24;
			NPC.height = 40;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.2f;
			NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = false;
			NPC.behindTiles = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Eggplant"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public void DrawBody(bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];

			int segments = 18;

			if (Parent.active && Parent.type == ModContent.NPCType<EggplantBase>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Vegetable/EggplantBody");

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - Vector2.Zero.RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - Vector2.Zero.RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = drawPosNext - drawPos2;
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - Vector2.Zero.RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - Vector2.Zero.RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/EggplantBodyGore").Type);
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawBody(false);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];

			NPC.TargetClosest();
			Player player = Main.player[NPC.target];

			//EoC rotation
			Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
			float RotateX = Parent.Center.X - vector.X;
			float RotateY = Parent.Center.Y - vector.Y;
			NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			if (Parent.active && Parent.type == ModContent.NPCType<EggplantBase>())
            {
                bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);

				//shoot out ooze
				if (player.Distance(Parent.Center) <= 360f && !player.dead && HasLineOfSight)
				{
					NPC.ai[0]++;
					if (NPC.ai[0] % 20 == 0)
                	{
						SoundEngine.PlaySound(SoundID.NPCDeath13 with { Volume = 0.5f, Pitch = -1.2f }, NPC.Center);

						//spawn ooze
						NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y), new Vector2(Main.rand.Next(-3, 4), 2), ModContent.ProjectileType<RottenOoze>(), NPC.damage, 4.5f);
					}
				}
				else
				{
					NPC.ai[0] = 0;
				}

				int MaxSpeed = 3;
				float Acceleration = 0.01f;

				//flies to parent X position
				if (NPC.Center.X >= Parent.Center.X && MoveSpeedX >= -MaxSpeed) 
				{
					MoveSpeedX--;
				}
				else if (NPC.Center.X <= Parent.Center.X && MoveSpeedX <= MaxSpeed)
				{
					MoveSpeedX++;
				}

				NPC.velocity.X += MoveSpeedX * Acceleration;
				NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed, MaxSpeed);
				
				//flies to parent Y position
				if (NPC.Center.Y >= Parent.Center.Y + 140 && MoveSpeedY >= -MaxSpeed)
				{
					MoveSpeedY--;
				}
				else if (NPC.Center.Y <= Parent.Center.Y + 140 && MoveSpeedY <= MaxSpeed)
				{
					MoveSpeedY++;
				}

				NPC.velocity.Y += MoveSpeedY * Acceleration;
				NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);

				NPC.velocity *= 0.985f;
            }
            else
            {
                NPC.active = false;
            }
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				NPC Parent = Main.npc[(int)NPC.ai[2]];

				Parent.active = false;

				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EggplantGore").Type);
				}

				DrawBody(true);
			}
		}
	}

	public class EggplantBase : ModNPC
	{
		public override string Texture => "Spooky/Content/Projectiles/Blank";

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 2;
			NPC.height = 2;
			NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.alpha = 255;
			NPC.aiStyle = -1;
		}

		public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

			if (NPC.ai[0] == 0)
			{
				int EarWorm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<Eggplant>(), ai1: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: EarWorm);
				}

				NPC.ai[0]++;

				NPC.netUpdate = true;
			}
		}
	}
}