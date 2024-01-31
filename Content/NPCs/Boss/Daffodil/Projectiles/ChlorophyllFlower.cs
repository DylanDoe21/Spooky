using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{ 
    public class ChlorophyllFlower : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        int target;

		public override void SetDefaults()
		{
			Projectile.width = 20;                   			 
            Projectile.height = 24;     
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 180;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return true;
        }

        const int TrailLength = 22;

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new RoundedTip(4), factor => 3 * factor, factor =>
            {
                return Color.Lerp(Color.Green, Color.Lime, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0f);

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 50)
            {
                if (Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                if (Projectile.velocity.X < 0)
                {
                    Projectile.rotation += -0.35f;
                }

                Projectile.velocity *= 0.99f;
            }

            if (Projectile.ai[0] >= 50 && Projectile.ai[0] < 90)
            {
                if (Projectile.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    target = -1;
                    float distance = 2000f;
                    for (int k = 0; k < 255; k++)
                    {
                        if (Main.player[k].active && !Main.player[k].dead)
                        {
                            Vector2 center = Main.player[k].Center;
                            float currentDistance = Vector2.Distance(center, Projectile.Center);
                            if (currentDistance < distance || target == -1)
                            {
                                distance = currentDistance;
                                target = k;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.ai[1] = 1;
                        Projectile.netUpdate = true;
                    }
                }
                else if (target >= 0 && target < Main.maxPlayers)
                {
                    Player targetPlayer = Main.player[target];
                    if (!targetPlayer.active || targetPlayer.dead)
                    {
                        target = -1;
                        Projectile.ai[1] = 0;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        float currentRot = Projectile.velocity.ToRotation();
                        Vector2 direction = targetPlayer.Center - Projectile.Center;
                        float targetAngle = direction.ToRotation();
                        if (direction == Vector2.Zero)
                        {
                            targetAngle = currentRot;
                        }

                        float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot);
                    }
                }

                Projectile.velocity *= Main.rand.NextFloat(1.003f, 1.0085f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int numDust = 0; numDust < 10; numDust++)
            {                                                                                  
                int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 1f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-1f, 1f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-1f, 1f);
                Main.dust[dustGore].noGravity = true;
            }
        }
    }
}