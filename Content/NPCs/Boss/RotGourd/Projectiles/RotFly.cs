using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{
	public class RotFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fly");
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 500;
            Projectile.penetrate = -1;
            Projectile.scale = 0.9f;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.DarkOliveGreen);

                Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Color newColor = color;
                    newColor = Projectile.GetAlpha(newColor);
                    newColor *= 1f;
                    Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 4 * 6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.1f, effects, 0);
                }
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
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

            int index1 = (int)Projectile.ai[1];
            
            if (Main.npc[index1].active && (Main.npc[index1].type == ModContent.NPCType<RotGourd>()) )
            {
                Projectile.timeLeft = 500;

                switch ((int)Projectile.ai[0])
				{
					//home in on rot gourd constantly
					case 0:
					{
                        float goToX = Main.npc[index1].Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                        float goToY = Main.npc[index1].Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                        //if rot gourd is flying in his desperation phase, make them go to the 
                        if (Main.npc[index1].ai[0] == 5)
                        {
                            goToY = (Main.npc[index1].Center.Y - 100) - Projectile.Center.Y + Main.rand.Next(-20, 20);
                        }

                        float speed = 0.08f;

                        if (Vector2.Distance(Projectile.Center, Main.npc[index1].Center) >= 135)
                        {
                            speed = 5f;
                        }
                        else
                        {
                            speed = 2f;
                        }

                        if (speed >= 5f)
                        {
                            speed = 5f;
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

                        break;
                    }

                    //charge at the player
					case 1:
					{
                        Projectile.localAI[0]++;

                        if (Projectile.localAI[0] == 2)
                        {
                            double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, 
                            Main.player[Main.myPlayer].position.X - Projectile.position.X);

                            Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * Main.rand.Next(8, 10);
                        }

                        break;
                    }
                }
            }
            //fade away and die if rot gourd doesnt exist
            else
            {
                Projectile.alpha += 2;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }

            //prevent projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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