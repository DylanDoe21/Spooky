using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class PitcherVine : ModProjectile
	{
		bool runOnce = true;
		bool AltColor = false;
		
		Vector2[] trailLength = new Vector2[50];
		float[] rotations = new float[50];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> ProjTexture2;
		private static Asset<Texture2D> TrailTexture;

		public static readonly SoundStyle GrowSound = new("Spooky/Content/Sounds/BigBone/PlantGrow", SoundType.Sound);
		public static readonly SoundStyle KillSound = new("Spooky/Content/Sounds/BigBone/PlantDestroy", SoundType.Sound) { Volume = 0.5f };

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                writer.WriteVector2(trailLength[i]);
				writer.Write(rotations[i]);
            }

            //bools
            writer.Write(runOnce);
			writer.Write(AltColor);

            //floats
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
			writer.Write(Projectile.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                trailLength[i] = reader.ReadVector2();
				rotations[i] = reader.ReadSingle();
            }

            //bools
            runOnce = reader.ReadBoolean();
			AltColor = reader.ReadBoolean();

            //floats
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
			Projectile.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 42;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 90;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			DrawChain(false);

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			ProjTexture2 ??= ModContent.Request<Texture2D>(Texture + "Yellow");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			Main.EntitySpriteDraw(AltColor ? ProjTexture2.Value : ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), 0f, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

		public bool DrawChain(bool SpawnGore)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/VineBase");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				Color color = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16));

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / 15;

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					if (!SpawnGore)
					{
						Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, rotations[k], drawOrigin, 1f, SpriteEffects.None, 0f);
					}
					else
					{
						Dust Grass = Dust.NewDustPerfect(previousPosition + -betweenPositions * (i / max), DustID.Grass, Vector2.Zero, default, default, 1.5f);
						Grass.noGravity = true;
					}
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[2] > 5)
			{
				Projectile.ai[2] = 36;
			}

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{	
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				SoundEngine.PlaySound(GrowSound, Projectile.Center);

				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					rotations[i] = 0f;
				}

				AltColor = Main.rand.NextBool();

				runOnce = false;
				Projectile.netUpdate = true;
			}

			Projectile.ai[2]++;
			if (Projectile.ai[2] <= 35)
			{
				Projectile.spriteDirection = Projectile.velocity.X < 0 ? -1 : 1;

				Projectile.timeLeft = 90;

				int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass);
				Main.dust[ProjDust].noGravity = true;
				Main.dust[ProjDust].scale = 1.8f;
				Main.dust[ProjDust].velocity /= 4f;
				Main.dust[ProjDust].velocity += Projectile.velocity / 2;

				//save previous positions, rotations, and direction
				if (Projectile.velocity != Vector2.Zero)
				{
					Vector2 current = Projectile.Center;
					float currentRot = Projectile.rotation;
					for (int i = 0; i < trailLength.Length; i++)
					{
						Vector2 previousPosition = trailLength[i];
						trailLength[i] = current;
						current = previousPosition;

						float previousRot = rotations[i];
						rotations[i] = currentRot;
						currentRot = previousRot;
					}
				}

				Projectile.localAI[0] = Main.rand.NextFloat(-7.5f, 7.5f);
				Projectile.localAI[1] = Main.rand.NextFloat(-10f, 11f);

				Projectile.netUpdate = true;

				Projectile.ai[0]++;
				if (Projectile.ai[1] == 0)
				{
					if (Projectile.ai[0] > Projectile.localAI[1] * 0.5f)
					{
						Projectile.ai[0] = 0;
						Projectile.ai[1] = 1;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-Projectile.localAI[0]));
						Projectile.velocity = perturbedSpeed;
					}

					Projectile.netUpdate = true;
				}
				else
				{
					if (Projectile.ai[0] <= Projectile.localAI[1])
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(Projectile.localAI[0]));
						Projectile.velocity = perturbedSpeed;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-Projectile.localAI[0]));
						Projectile.velocity = perturbedSpeed;
					}
					
					if (Projectile.ai[0] >= Projectile.localAI[1] * 2)
					{
						Projectile.ai[0] = 0;
					}

					Projectile.netUpdate = true;
				}
			}
			else
			{
				Projectile.velocity = Vector2.Zero;

				Projectile.localAI[2]++;
				if (Projectile.localAI[2] > 30 && Projectile.localAI[2] < 60)
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 4 && Projectile.frame < 2)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;
					}
				}

				if (Projectile.localAI[2] == 60)
				{
					SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

					for (int numProj = 0; numProj < 2; numProj++)
					{
						int ShootSpeedX = Main.rand.Next(-6, 7);
						int ShootSpeedY = Main.rand.Next(-10, -7);

						int ProjType = Main.rand.NextBool() ? ModContent.ProjectileType<PitcherOoze1>() : ModContent.ProjectileType<PitcherOoze2>();

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Top, new Vector2(ShootSpeedX, ShootSpeedY), ProjType, Projectile.damage, Projectile.knockBack);
						}
					}
				}

				if (Projectile.localAI[2] > 75)
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 4 && Projectile.frame > 0)
					{
						Projectile.frameCounter = 0;
						Projectile.frame--;
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			if (Main.netMode != NetmodeID.Server) 
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, AltColor ? ModContent.Find<ModGore>("Spooky/PitcherGore2").Type : ModContent.Find<ModGore>("Spooky/PitcherGore1").Type);
			}

			DrawChain(true);
		}
	}
}