using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class BouncingBone : ModProjectile
    {
        int bounces = 0;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

        private static Asset<Texture2D> TrailTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (runOnce)
			{
				return false;
			}

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

            Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Orange);

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.45f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

            Color AuraColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Gold);

            Vector2 AuraDrawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, AuraColor, Projectile.rotation, AuraDrawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt with { Pitch = 0.5f, Volume = 0.4f }, Projectile.Center);

            if (bounces > 4)
            {
                Projectile.Kill();
            }
            else
            {
                bounces++;

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.75f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.65f;
                }
            }

			return false;
		}

        public override void AI()
        {
            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 60)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            }

            if (Projectile.ai[0] == 1)
            {
                float maxAmount = 10;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(5f, 5f);
                    Vector2 Bounds = new Vector2(3f, 3f);
                    float intensity = 5f;

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                    int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame, 0f, 0f, 100, default, 1f);
                    Main.dust[num104].noGravity = true;
                    Main.dust[num104].position = Projectile.Center + vector12;
                    Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                    currentAmount++;
                }
            }
        }
    }
}