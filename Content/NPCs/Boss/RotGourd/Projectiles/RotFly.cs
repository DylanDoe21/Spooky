using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{
	public class RotFly : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.scale = 0.9f;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 3)
            {
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);

                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Brown);

                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Color newColor = color;
                    newColor = Projectile.GetAlpha(newColor);
                    newColor *= 1f;
                    Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, effects, 0);
                }
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 3)
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
            
            if (Main.npc[index1].active && Main.npc[index1].type == ModContent.NPCType<RotGourd>())
            {
                Projectile.timeLeft = 300;
            }
            else
            {
                Projectile.ai[0] = -1;
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
                    if (Main.npc[index1].active && Main.npc[index1].type == ModContent.NPCType<RotGourd>())
                    {
                        float goToX = Main.npc[index1].Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                        float goToY = Main.npc[index1].Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                        //if rot gourd is flying in his desperation phase, make them go to the bottom of him
                        if (Main.npc[index1].ai[0] == 6)
                        {
                            goToX = Main.npc[index1].Center.X - Projectile.Center.X + Main.rand.Next(-100, 100);
                            goToY = Main.npc[index1].Center.Y - Projectile.Center.Y + Main.rand.Next(35, 100);
                        }

                        //if the flies are above the player during cripple phase, then fly above them
                        if (Main.npc[index1].ai[0] == 7 && Projectile.localAI[1] == 1)
                        {
                            goToX = Main.LocalPlayer.Center.X - Projectile.Center.X + Main.rand.Next(-350, 350);
                            goToY = Main.LocalPlayer.Center.Y - Projectile.Center.Y + Main.rand.Next(-500, -400);
                        }

                        if (Main.npc[index1].ai[0] == 7 && Projectile.localAI[1] == 2)
                        {
                            goToX = Main.npc[index1].Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                            goToY = Main.npc[index1].Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);
                        }

                        float speedLimit = Main.npc[index1].ai[0] == 6 ? 8f : 5f;
                        float speed = Main.npc[index1].ai[0] == 6 ? 0.1f : 0.08f;

                        if (Vector2.Distance(Projectile.Center, Main.npc[index1].Center) >= 135)
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
                    }

                    break;
                }

                //charge at the player
                case 1:
                {
                    Projectile.localAI[0]++;

                    if (Projectile.localAI[0] == 2)
                    {
                        SoundEngine.PlaySound(FlySound, Projectile.Center);

                        double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, 
                        Main.player[Main.myPlayer].position.X - Projectile.position.X);

                        Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * Main.rand.Next(8, 12);

                        Projectile.netUpdate = true;
                    }

                    if (Projectile.localAI[0] == 120)
                    {
                        Projectile.ai[0] = 0;

                        Projectile.netUpdate = true;
                    }

                    break;
                }

                //for when his flies drop him in his desperation phase
                case 2:
                {
                    Projectile.velocity *= 0.95f;

                    Projectile.netUpdate = true;

                    break;
                }

                //go above the player in a massive swarm, then charge
                case 3:
                {
                    Projectile.localAI[0]++;

                    if (Projectile.localAI[0] == 2)
                    {
                        SoundEngine.PlaySound(FlySound, Projectile.Center);

                        double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, 
                        Main.player[Main.myPlayer].position.X - Projectile.position.X);

                        Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * Main.rand.Next(12, 15);

                        Projectile.netUpdate = true;
                    }

                    if (Projectile.localAI[0] >= 60)
                    {
                        Projectile.ai[0] = 0;
                        Projectile.localAI[1] = 2;

                        Projectile.netUpdate = true;
                    }

                    break;
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