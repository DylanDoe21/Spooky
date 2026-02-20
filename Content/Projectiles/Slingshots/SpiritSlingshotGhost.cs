using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class SpiritSlingshotGhost : ModProjectile
    {
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 4;
        }

		public override void SetDefaults()
		{
			Projectile.width = 14;
            Projectile.height = 22;
			Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{   
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawTrailOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;

				Color color = Projectile.GetAlpha(Color.Lerp(Color.MediumPurple, Color.White, scale));

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawTrailOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override void PostDraw(Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			if (Projectile.ai[1] > 0)
			{
				SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);

				float maxAmount = 3;
				int currentAmount = 1;
				while (currentAmount <= maxAmount)
				{
					Vector2 velocity = new Vector2(Main.rand.NextFloat(3f, 6f), Main.rand.NextFloat(3f, 6f));
					Vector2 Bounds = new Vector2(Main.rand.NextFloat(3f, 6f), Main.rand.NextFloat(3f, 6f));
					float intensity = Main.rand.NextFloat(3f, 6f);

					Vector2 vector12 = Vector2.UnitX * 0f;
					vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
					vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

					Vector2 ShootVelocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

					Projectile.NewProjectile(target.GetSource_OnHurt(Main.player[Projectile.owner]), Projectile.Center + vector12, ShootVelocity, 
					ModContent.ProjectileType<SpiritSlingshotGhostBolt>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);

					currentAmount++;
				}
			}
        }

		public override void AI()
		{
			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

                Projectile.alpha = 0;

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			if (Projectile.alpha > 0 && Projectile.timeLeft > 60)
			{
				Projectile.alpha -= 5;
			}
			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}

			Projectile.ai[0]++;
            if (Projectile.ai[0] > 20)
            {
                int foundTarget = FindTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.velocity *= 0.975f;
                    Projectile.tileCollide = true;
                }
            }
		}

		private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 250;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

		public override void OnKill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 15; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
					
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}