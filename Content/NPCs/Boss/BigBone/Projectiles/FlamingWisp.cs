using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class FlamingWisp : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        int Offset = Main.rand.Next(-100, 100);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Wisp");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
			Projectile.height = 36;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/FireTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        const int TrailLength = 25;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            //using (factor => 12 * factor) makes the trail get smaller the further from the projectile, the number (12 in this case) affects how thick it is
            //just using (factor => 12) makes the trail the same size, where again the number (12 in this case) is the constant thickness
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 15 * factor, factor =>
            {
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.Red, Color.OrangeRed, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0f);

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 75)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (Projectile.ai[0] < 75)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
                    {
                        float goToX = Main.npc[k].Center.X + Offset - Projectile.Center.X;
                        float goToY = Main.npc[k].Center.Y + Offset - Projectile.Center.Y;
                        float speed = 0.12f;

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
                    }
                }
            }

            if (Projectile.ai[0] == 75)
            {
                Projectile.tileCollide = true;
                
                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12; //10 is the "charge" speed
            }
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.Center);
        
        	for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}