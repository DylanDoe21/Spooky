using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class EyeWizardBook : ModNPC
	{
		bool Shake = false;
		bool AfterImages = false;
		bool SpawnedGlassEye = false;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> AuraTexture;
		private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
			NPCID.Sets.TrailCacheLength[NPC.type] = 35;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			//bools
            writer.Write(Shake);
			writer.Write(AfterImages);
			writer.Write(SpawnedGlassEye);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//bools
            Shake = reader.ReadBoolean();
			AfterImages = reader.ReadBoolean();
			SpawnedGlassEye = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 32;
			NPC.defense = 0;
			NPC.width = 48;
			NPC.height = 48;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.HitSound = SoundID.Tink with { Volume = 0.75f, Pitch = 1.25f };
			NPC.DeathSound = SoundID.Shatter;
			NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardBookAura");
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/EyeWizardBookGlow");

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - screenPos;

			for (int i = 0; i < 360; i += 30)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(1f, 2f)).RotatedBy(MathHelper.ToRadians(i));

				Color color = Color.White;

				if (Parent.ai[0] == 2)
				{
					color = Color.Red;
				}
				if (Parent.ai[0] == 4)
				{
					color = Color.Lime;
				}
				if (Parent.ai[0] == 6)
				{
					color = Color.Blue;
				}

                Main.EntitySpriteDraw(AuraTexture.Value, drawPosition + circular, NPC.frame, color * 0.15f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.05f, effects, 0);
            }

            Main.EntitySpriteDraw(NPCTexture.Value, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
			Main.EntitySpriteDraw(GlowTexture.Value, drawPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);

            return false;
		}

		public override void FindFrame(int frameHeight)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			if (Parent.ai[0] == 2 || Parent.ai[0] == 4 || Parent.ai[0] == 6 || (Parent.ai[0] == 0 && NPC.life <= 1))
			{
				NPC.frame.Y = 1 * frameHeight;
			}
			else
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override bool CheckDead()
		{
			Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 200f);

			NPC Parent = Main.npc[(int)NPC.ai[0]];

			for (int numProjectiles = 0; numProjectiles < 6; numProjectiles++)
			{
				NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-7, -3)), ModContent.ProjectileType<EyeWizardEnergy>(), 0, 0f, ai0: Parent.whoAmI);
			}

			NPC.life = 1;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];
			Player player = Main.player[Parent.target];

			bool Phase2 = Parent.life <= (Parent.lifeMax / 2);

			//kill this npc if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyeWizard>())
			{
                NPC.active = false;
			}

			if (Parent.ai[0] == 0)
			{
				Parent.spriteDirection = Parent.direction = NPC.Center.X < Parent.Center.X ? -1 : 1;

				if (NPC.ai[1] == 0)
				{
					NPC.ai[3] = NPC.position.Y;
					NPC.ai[1]++;
				}

				NPC.ai[2]++;
				NPC.position.Y = NPC.ai[3] + (float)Math.Sin(NPC.ai[2] / 30) * 30;
			}
			else
			{
				NPC.spriteDirection = NPC.direction = NPC.Center.X < Parent.Center.X ? -1 : 1;

				//fire off eye orb to shoot bouncing eyes
				if (Parent.ai[0] == 2)
				{
					NPC.velocity *= 0.95f;
					NPC.rotation = 0;

					if (Parent.localAI[0] == 20)
					{
						SoundEngine.PlaySound(SoundID.Item9 with { Pitch = -0.75f }, NPC.Center);

						float maxAmount = 15;
						int currentAmount = 0;
						while (currentAmount <= maxAmount)
						{
							Vector2 velocity = new Vector2(5f, 5f);
							Vector2 Bounds = new Vector2(3f, 3f);
							float intensity = 5f;

							Vector2 vector12 = Vector2.UnitX * 0f;
							vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
							vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
							int newDust = Dust.NewDust(NPC.Center, 0, 0, DustID.RedTorch, 0f, 0f, 100, default, 3f);
							Main.dust[newDust].noGravity = true;
							Main.dust[newDust].position = NPC.Center + vector12;
							Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity + new Vector2(0, -2) + NPC.velocity;
							currentAmount++;
						}

						NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Top, new Vector2(0, -2) + NPC.velocity, ModContent.ProjectileType<EyeWizardOrb>(), NPC.damage, 4.5f, ai1: Phase2 ? 20 : 30);
					}
				}
				//spin and fire out runes
				else if (Parent.ai[0] == 4)
				{
					//first, go to the location where the book needs to start spinning so the book doesnt just randomly teleport to where it starts spinning
					if (Parent.localAI[0] < 40)
					{
						double rad = NPC.ai[1] * (Math.PI / 180);
						int distance = 45;

						Vector2 GoTo = new Vector2(Parent.Center.X - (int)(Math.Cos(rad) * distance), Parent.Center.Y - (int)(Math.Sin(rad) * distance));

						Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * (Phase2 ? 15 : 10);
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					//afterward, rotate towards and then spin around bigger eye while shooting out runes
					else
					{
						float RotateX = Parent.Center.X - NPC.Center.X;
						float RotateY = Parent.Center.Y - NPC.Center.Y;
						NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

						NPC.ai[1] += 3.475f;
						double rad = NPC.ai[1] * (Math.PI / 180);
						int distance = 45;
						NPC.position.X = Parent.Center.X - (int)(Math.Cos(rad) * distance) - NPC.width / 2;
						NPC.position.Y = Parent.Center.Y - (int)(Math.Sin(rad) * distance) - NPC.height / 2;

						if (Parent.localAI[0] % 9 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item46 with { Pitch = -0.75f }, NPC.Center);

							Vector2 ShootSpeed = Parent.Center - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= -2f;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;
							Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

							if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
							{
								position += muzzleOffset;
							}

							for (int numDusts = 0; numDusts < 12; numDusts++)
							{
								Dust dust = Dust.NewDustPerfect(new Vector2(NPC.Center.X + muzzleOffset.X, NPC.Center.Y + muzzleOffset.Y), DustID.GreenTorch,
								new Vector2(ShootSpeed.X + Main.rand.Next(-7, 8), ShootSpeed.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
								dust.noGravity = true;
							}

							NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<EyeBookRune>(), NPC.damage, 4.5f, ai1: Main.rand.Next(0, 4));
						}
					}
				}
				//zip to locations and fire out lingering eyes
				else if (Parent.ai[0] == 6)
				{
					AfterImages = true;

					float RotateX = player.Center.X - NPC.Center.X;
					float RotateY = player.Center.Y - NPC.Center.Y;
					NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + MathHelper.PiOver2;

					if (Parent.localAI[0] == 10)
					{
						Vector2 NPCNewPosition = new Vector2(620, 0).RotatedByRandom(360);

						MoveToPlayer(player, NPCNewPosition.X, NPCNewPosition.Y);
					}
					else
					{
						NPC.velocity *= 0.65f;
					}

					if (Parent.localAI[0] == 30)
					{
						SoundEngine.PlaySound(SoundID.Item46 with { Pitch = -0.75f }, NPC.Center);

						Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 15f;

						Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;
						Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

						if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
						{
							position += muzzleOffset;
						}

						for (int numDusts = 0; numDusts < 12; numDusts++)
						{
							Dust dust = Dust.NewDustPerfect(new Vector2(NPC.Center.X + muzzleOffset.X, NPC.Center.Y + muzzleOffset.Y), DustID.BlueTorch,
							new Vector2(Main.rand.Next(-7, 8), Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
							dust.noGravity = true;
						}

						NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<EyeRune>(), NPC.damage, 4.5f, ai0: NPC.rotation, ai1: Main.rand.Next(0, 4));
					}
				}
				else
				{
					if (Parent.localAI[0] == 2)
					{
						NPC.rotation = 0;
						AfterImages = false;
					}

					//shake the book a little bit for added effect
					if (Shake)
					{
						NPC.rotation += 0.01f;
						if (NPC.rotation > 0.12f)
						{
							Shake = false;
						}
					}
					else
					{
						NPC.rotation -= 0.01f;
						if (NPC.rotation < -0.12f)
						{
							Shake = true;
						}
					}

					//go to location relative to bigger eye based on its direction
					Vector2 GoTo = Parent.spriteDirection == -1 ? new Vector2(-75, 0) : new Vector2(75, 0);

					Vector2 desiredVelocity = NPC.DirectionTo(Parent.Center + GoTo) * 9;
					NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
				}
			}
		}

        public void MoveToPlayer(Player target, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = target.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }
	}
}