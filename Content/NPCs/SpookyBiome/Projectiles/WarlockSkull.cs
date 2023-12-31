using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{ 
    public class WarlockSkull : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        int target;

        int Offset = Main.rand.Next(-20, 20);

		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;                   			 
            Projectile.height = 16;         
			Projectile.hostile = true;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 4 * factor, factor =>
            {
                return Color.Lerp(Color.Purple, Color.OrangeRed, factor.X) * factor.X;
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
                    if (Main.npc[k].active && (Main.npc[k].type == ModContent.NPCType<ZomboidWarlock>()))
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

            if (Projectile.ai[0] > 75 && Projectile.ai[0] < 110)
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

                Projectile.velocity *= 1.055f;
            }

            if (Projectile.ai[0] > 110)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }
        }
    }
}