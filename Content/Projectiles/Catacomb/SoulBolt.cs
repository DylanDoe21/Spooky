using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SoulBolt : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;
		
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

        const int TrailLength = 5;

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
                return Color.Lerp(Color.White, Color.Cyan, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
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

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            if (Projectile.ai[0] == 0)
            {
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

		public override void OnKill(int timeLeft)
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
     
          






