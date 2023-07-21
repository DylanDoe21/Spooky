using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class GourdFlailFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true; 
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
            Projectile.scale = 0.85f;
            Projectile.aiStyle = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 500;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            if (!player.active || player.dead || Projectile.ai[0] > 0)
            {
                Projectile.alpha += 2;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.alpha -= 1;

                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }

            if (Main.mouseLeft && Projectile.ai[0] == 0)
            {
                float goToX = player.Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                float goToY = player.Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                float speed = 0.08f;

                if (Vector2.Distance(Projectile.Center, player.Center) >= 300)
                {
                    Projectile.position = player.Center;
                }

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

            if (Main.mouseLeftRelease && Projectile.ai[0] == 0)
            {
                Vector2 ChargeDirection = Main.MouseWorld - Projectile.Center;
                ChargeDirection.Normalize();

                ChargeDirection *= 25;
                Projectile.velocity = ChargeDirection;

                Projectile.ai[0] = 1;
            }
        }
    }
}