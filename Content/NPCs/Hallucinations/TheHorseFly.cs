using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Hallucinations
{
	public class TheHorseFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.scale = 0.9f;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
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

            return true;
        }

        public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            switch ((int)Projectile.ai[0])
            {
                //used to store if the fly should fade away
                case -1:
                {
                    Projectile.alpha += 5;

                    if (Projectile.alpha >= 255)
                    {
                        Projectile.netUpdate = true;
                        Projectile.Kill();
                    }

                    break;
                }

                //home in on rot gourd if it exists
                case 0:
                {
                    Projectile.timeLeft = 300;
                    
                    if (player.dead || !NPC.AnyNPCs(ModContent.NPCType<TheHorse>())) 
                    {
                        Projectile.ai[0] = -1;
                    }

                    float goToX = player.Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                    float goToY = player.Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                    float speedLimit = 5f;
                    float speed = 0.08f;

                    if (Vector2.Distance(Projectile.Center, player.Center) >= 135)
                    {
                        speed = speedLimit;
                    }
                    else
                    {
                        speed = 2f;
                    }

                    if (speed >= speedLimit)
                    {
                        speed = speedLimit;
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

                    Projectile.netUpdate = true;

                    break;
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
	}
}