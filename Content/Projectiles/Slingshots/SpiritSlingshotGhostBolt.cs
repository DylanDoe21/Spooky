using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class SpiritSlingshotGhostBolt : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

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

			TrailTexture ??= ModContent.Request<Texture2D>(Texture);

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

		public override bool? CanDamage()
		{
			return Projectile.ai[0] > 20;
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
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 10;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
		}

		private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 200;

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
			for (int numDusts = 0; numDusts < 7; numDusts++)
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