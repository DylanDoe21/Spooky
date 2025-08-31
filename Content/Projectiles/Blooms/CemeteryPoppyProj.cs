using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Blooms
{
	public class CemeteryPoppyProj : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];
		float[] rotations = new float[10];

		float SaveRotation = 0;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

		public override void SetDefaults()
		{
			Projectile.width = 46;
			Projectile.height = 48;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CemeteryMarigoldProjStem");

			Vector2 drawOriginTrail = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color lightColorAtPos = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16));
				Color color = Color.Lerp(lightColorAtPos * 0f, lightColorAtPos, scale);

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / 15;

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Color colorWithAlphaWhenDying = Projectile.timeLeft <= 60 ? Projectile.GetAlpha(color) : color;

					Main.EntitySpriteDraw(TrailTexture.Value, drawPos, null, colorWithAlphaWhenDying, rotations[k], drawOriginTrail, 1f, SpriteEffects.None, 0);
				}

				previousPosition = currentPos;
			}

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					rotations[i] = 0f;
				}

				runOnce = false;
			}

			if (Projectile.scale < 1f)
			{
				Projectile.scale += 0.12f;
			}

			if (Projectile.timeLeft <= 30)
			{
				Projectile.alpha += 10;
			}

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] <= 10)
			{
				SaveRotation = Projectile.rotation;

				int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch);
				Main.dust[ProjDust].noGravity = true;
				Main.dust[ProjDust].scale = 1.2f;
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

				//move in a random wavy pattern
				float WaveIntensity = Main.rand.NextFloat(-10f, 11f);
				float Wave = Main.rand.NextFloat(-10f, 11f);

				Projectile.ai[0]++;
				if (Projectile.ai[1] == 0)
				{
					if (Projectile.ai[0] > Wave * 0.5f)
					{
						Projectile.ai[0] = 0;
						Projectile.ai[1] = 1;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
				}
				else
				{
					if (Projectile.ai[0] <= Wave)
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					if (Projectile.ai[0] >= Wave * 2)
					{
						Projectile.ai[0] = 0;
					}
				}
			}
			else
			{
				Projectile.velocity = Vector2.Zero;

				Projectile.rotation = SaveRotation;

				Projectile.frame = (int)Projectile.ai[2];

				if (Projectile.ai[2] > 5)
				{
					Projectile.Kill();
				}

				Projectile.localAI[1]++;
				if (Projectile.localAI[1] >= 60)
				{
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC Target = Projectile.OwnerMinionAttackTargetNPC;
						if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type])
						{
							Vector2 ShootSpeed = Target.Center - Projectile.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= 5f;
									
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, ShootSpeed, 
							ModContent.ProjectileType<CemeteryPoppyPetal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

							Projectile.ai[2]++;

							break;
						}

						NPC NPC = Main.npc[i];
						if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
						{
							Vector2 ShootSpeed = NPC.Center - Projectile.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= 6f;
									
							Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, ShootSpeed, 
							ModContent.ProjectileType<CemeteryPoppyPetal>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

							Projectile.ai[2]++;

							break;
						}
					}

					Projectile.localAI[1] = 0;
				}
			}
		}
	}
}