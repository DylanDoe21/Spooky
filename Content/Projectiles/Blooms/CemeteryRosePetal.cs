using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Core;
using static Terraria.GameContent.PlayerEyeHelper;

namespace Spooky.Content.Projectiles.Blooms
{
    public class CemeteryRosePetal : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Lerp(Color.Black, Color.MediumPurple, oldPos / (float)Projectile.oldPos.Length)) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
			}

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool? CanDamage()
		{
			return Projectile.timeLeft <= 120;
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (Projectile.timeLeft < 60)
			{
				if (Projectile.scale > 0)
				{
					Projectile.scale -= 0.001f;
				}
				else
				{
					Projectile.Kill();
				}
			}

			float WaveIntensity = Main.rand.NextFloat(-5f, 5f);
			float Wave = Main.rand.NextFloat(-5f, 5f);

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

			int foundTarget = HomeOnTarget();
            if (foundTarget != -1 && Projectile.timeLeft <= 120)
            {
                Projectile.spriteDirection = Projectile.direction;

				NPC target = Main.npc[foundTarget];
				Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 15;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
			}
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 500;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
					if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance) && (target.IsTechnicallyBoss() || target.boss))
					{
						selectedTarget = i;
					}
					if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance) && !target.IsTechnicallyBoss() && !target.boss)
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }
    }
}