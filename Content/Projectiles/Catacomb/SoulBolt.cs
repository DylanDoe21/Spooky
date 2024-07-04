using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SoulBolt : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

                Color color = Color.Lerp(Color.White, Color.Cyan, scale);
                color *= (Projectile.timeLeft) / 90f;

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

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-1, 2) * scale;
					float y = Main.rand.Next(-1, 2) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale * 0.6f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override bool? CanDamage()
        {
            return Projectile.ai[0] == 1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

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

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] >= 600)
                {
                    Projectile.Kill();
                }

                Projectile.penetrate = -1;
                Projectile.timeLeft = 180;

                float goToX = player.Center.X - Projectile.Center.X + Main.rand.Next(-100, 100);
                float goToY = player.Center.Y - Projectile.Center.Y + Main.rand.Next(-100, 100);

                float speed = 0.08f;

                if (Vector2.Distance(Projectile.Center, player.Center) >= 135)
                {
                    speed = 3f;
                }
                else
                {
                    speed = 2f; //was 0.08
                }

                if (Projectile.velocity.X > speed)
                {
                    Projectile.velocity.X *= 0.98f;
                }
                if (Projectile.velocity.Y > speed)
                {
                    Projectile.velocity.Y *= 0.98f;
                }

                if (Projectile.velocity.X < goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                    if (Projectile.velocity.X < 0f && goToX > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                    }
                }
                else if (Projectile.velocity.X > goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                    if (Projectile.velocity.X > 0f && goToX < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                    }
                }
                if (Projectile.velocity.Y < goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    if (Projectile.velocity.Y < 0f && goToY > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        return;
                    }
                }
                else if (Projectile.velocity.Y > goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    if (Projectile.velocity.Y > 0f && goToY < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        return;
                    }
                }

                //prevent projectiles clumping together
                for (int num = 0; num < Main.projectile.Length; num++)
                {
                    Projectile other = Main.projectile[num];
                    if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                    {
                        const float pushAway = 0.08f;
                        if (Projectile.position.X < other.position.X)
                        {
                            Projectile.velocity.X -= pushAway;
                        }
                        else
                        {
                            Projectile.velocity.X += pushAway;
                        }
                        if (Projectile.position.Y < other.position.Y)
                        {
                            Projectile.velocity.Y -= pushAway;
                        }
                        else
                        {
                            Projectile.velocity.Y += pushAway;
                        }
                    }
                }
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.penetrate = 1;

                Projectile.ai[1]++;

                if (Projectile.ai[1] == 1)
                {
                    Projectile.damage *= 2;

                    Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                    Vector2 newVelocity = Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));
                    Projectile.velocity = newVelocity;
                }

                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 8;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SoulBolt>()] >= 10)
            {
                Projectile.ai[0] = 1;
            }
        }

        private int HomeOnTarget()
        {
            const bool homingCanAimAtWetEnemies = true;
            const float homingMaximumRangeInPixels = 500;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile) && (!target.wet || homingCanAimAtWetEnemies))
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
    }
}
     
          






