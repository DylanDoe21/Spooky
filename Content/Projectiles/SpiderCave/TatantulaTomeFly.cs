using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class TatantulaTomeFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true; 
			Projectile.tileCollide = false;
			Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
		}

        public override bool? CanDamage()
		{
            return Projectile.ai[2] == 0;
        }

        public override bool? CanCutTiles()
        {
            return Projectile.ai[0] > 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 180;

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

            if (Projectile.ai[2] == 0)
            {
                Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == 1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (Main.mouseLeft && Projectile.ai[0] == 0 && Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 180;

                Projectile.tileCollide = false;

                float goToX = Main.MouseWorld.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                float goToY = Main.MouseWorld.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                float speed = 0.15f;

                if (Vector2.Distance(Projectile.Center, Main.MouseWorld) >= 140)
                {
                    speed = 0.3f;
                }
                else
                {
                    speed = 0.15f;
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

            Projectile.ai[1]++;

            if (Main.mouseLeftRelease && Projectile.ai[0] == 0 && Projectile.ai[1] >= 5)
            {
                if (player.ownedProjectileCounts[Type] >= 10)
                {
                    Projectile.tileCollide = true;

                    Vector2 ChargeDirection = Main.MouseWorld - Projectile.Center;
                    ChargeDirection.Normalize();

                    ChargeDirection *= -25;
                    Projectile.velocity = ChargeDirection;

                    Projectile.ai[0] = 1;
                }
                else
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[2] = 1;
                }
            }

            if (Projectile.ai[2] > 0)
            {
                Projectile.rotation += 0.25f * (float)Projectile.direction;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

                Projectile.alpha += 2;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }

            int foundTarget = HomeOnTarget();
            if (foundTarget != -1)
            {
                NPC target = Main.npc[foundTarget];
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 20;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                Projectile.tileCollide = false;
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 350;

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
    }
}