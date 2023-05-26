using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SoulBolt : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";
		
        public override void SetDefaults()
        {
            Projectile.width = 10;                  			 
            Projectile.height = 10;         
			Projectile.friendly = true;              			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AI()
        {
            //fix Projectile direction
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            int NewDust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.UltraBrightTorch, 0f, -2f, 0, default(Color), 1.5f);
            Main.dust[NewDust].noGravity = true;
            Main.dust[NewDust].velocity *= 0;

            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] == 0)
            {
                Projectile.penetrate = -1;
                Projectile.timeLeft = 180;

                float goToX = player.Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                float goToY = player.Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

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

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SoulBolt>()] >= 5)
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

		public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 20; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.UltraBrightTorch, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
		}
    }
}
     
          






