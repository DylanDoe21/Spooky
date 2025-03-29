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
using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
	public class EarWorm : ModNPC
	{
		float SaveRotation = 0f;

		bool Shake = false;

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> ChainTexture;

		public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/EggEvent/EarWormScreech", SoundType.Sound) { Volume = 1.35f };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/EarWormBestiary",
				Position = new Vector2(35f, 20f),
				PortraitPositionXOverride = 0f,
				PortraitPositionYOverride = 20f
			};

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			//bools
			writer.Write(Shake);

            //floats
            writer.Write(SaveRotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//bools
			Shake = reader.ReadBoolean();

            //floats
            SaveRotation = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
			NPC.lifeMax = 520;
			NPC.damage = 45;
			NPC.defense = 10;
			NPC.width = 52;
			NPC.height = 74;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 2, 0);
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.Zombie40 with { Pitch = 0.45f };
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EarWorm"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public void DrawBody(bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];

			if (Parent.active && Parent.type == ModContent.NPCType<EarWormBase>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/EarWormSegment");

				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

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
				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/EarWormSegmentGore").Type);
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawBody(false);

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/EarWormGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return NPC.alpha <= 0;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];

			Player player = Main.player[Parent.target];

			NPC.spriteDirection = Parent.direction;

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EarWormBase>())
			{
				NPC.active = false;
			}

			Parent.ai[0] = NPC.whoAmI;

			if (NPC.ai[1] < 340 || NPC.ai[1] > 400)
			{
				//EoC rotation
				Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
				float RotateX = player.Center.X - vector.X;
				float RotateY = player.Center.Y - vector.Y;
				NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
			}

			if (Parent.ai[1] == 0)
			{
				NPC.ai[1]++;

				//fade in and go up
				if (NPC.ai[1] < 340)
				{
					NPC.ai[0] += 0.05f;

					Vector2 offset = new Vector2(Parent.direction).RotatedBy((float)Math.PI * 20f * (NPC.ai[0] / 60f));
					Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - 300) + offset * 15f;

					Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 3;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

					SaveRotation = NPC.rotation;

					if (NPC.alpha > 0)
					{
						NPC.alpha -= 15;
					}
				}

				//shake before attacking
				if (NPC.ai[1] > 340 && NPC.ai[1] < 400)
				{
					NPC.velocity = Vector2.Zero;

					if (Shake)
					{
						NPC.rotation += 0.2f;
						if (NPC.rotation > SaveRotation + 0.35f)
						{
							Shake = false;
						}
					}
					else
					{
						NPC.rotation -= 0.2f;
						if (NPC.rotation < SaveRotation - 0.35f)
						{
							Shake = true;
						}
					}
				}

				//shoot sound blast
				if (NPC.ai[1] == 400)
				{
					SoundEngine.PlaySound(ScreechSound, NPC.Center);

					Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 400f);

					Vector2 ShootSpeed = player.Center - NPC.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 10;

					NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<EarWormSoundBase>(), NPC.damage, 4.5f);

					NPC.velocity = -Vector2.Normalize(player.Center - NPC.Center) * Main.rand.Next(15, 23);
				}

				//slow down after recoil
				if (NPC.ai[1] >= 400 && NPC.ai[1] < 460)
				{
					NPC.velocity *= 0.8f;
				}

				if (NPC.ai[1] >= 460)
				{
					Vector2 desiredVelocity = NPC.DirectionTo(Parent.Center) * 15;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

					if (NPC.alpha < 255 && NPC.Distance(Parent.Center) < 40f)
					{
						NPC.alpha += 50;
					}

					if (NPC.alpha >= 255)
					{
						NPC.velocity = Vector2.Zero;

						NPC.immortal = true;
						NPC.dontTakeDamage = true;

						Parent.ai[1] = 1;
					}
				}
			}
			else
			{
				NPC.immortal = false;
				NPC.dontTakeDamage = false;

				NPC.ai[1] = 0;
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 5, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GiantEar>(), 40));
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
				NPC Parent = Main.npc[(int)NPC.ai[2]];

				Parent.active = false;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					//spawn splatter
					for (int i = 0; i < 6; i++)
					{
						NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, -1)), ModContent.ProjectileType<GreenSplatter>(), 0, 0f);
					}
				}

				for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EarWormGore" + numGores).Type);
                    }
                }

				DrawBody(true);
			}
		}
	}

	public class EarWormBase : ModNPC
	{
		public override string Texture => "Spooky/Content/Projectiles/Blank";

		public float destinationX = 0f;
		public float destinationY = 0f;

		bool FoundPosition = false;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			//bools
			writer.Write(FoundPosition);

            //floats
            writer.Write(destinationX);
            writer.Write(destinationY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//bools
			FoundPosition = reader.ReadBoolean();

            //floats
            destinationX = reader.ReadSingle();
            destinationY = reader.ReadSingle();
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
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			if (NPC.ai[3] == 0)
			{
				int EarWorm = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<EarWorm>(), ai2: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: EarWorm);
				}

				NPC.ai[3]++;

				NPC.netUpdate = true;
			}

			//teleport to another location
			if (NPC.ai[1] == 1)
			{
				if (!FoundPosition && destinationX == 0f && destinationY == 0f)
				{
					Vector2 center = new Vector2(player.Center.X, player.Center.Y - 100);

					center.X += Main.rand.Next(-500, 500);

					int numtries = 0;
					int x = (int)(center.X / 16);
					int y = (int)(center.Y / 16);

					while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
					{
						y++;
						center.Y = y * 16;
					}
					while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
					{
						numtries++;
						y--;
						center.Y = y * 16;
					}

					destinationX = center.X;
					destinationY = center.Y;

					FoundPosition = true;
				}
				else
				{
					NPC.ai[2]++;
					if (NPC.ai[2] > 90 && destinationX != 0f && destinationY != 0f)
					{
						NPC.position.X = destinationX - (float)(NPC.width / 2);
						NPC.position.Y = destinationY - (float)NPC.height + 25f;
						destinationX = 0f;
						destinationY = 0f;

						SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

						for (int numDusts = 0; numDusts < 15; numDusts++)
						{
							Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y + 24), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, default, 2.5f);
							dust.noGravity = true;
						}

						Main.npc[(int)NPC.ai[0]].Center = NPC.Center;
						
						NPC.ai[1] = 0;
						NPC.ai[2] = 0;

						FoundPosition = false;

						NPC.netUpdate = true;
					}
				}
			}
		}
	}
}