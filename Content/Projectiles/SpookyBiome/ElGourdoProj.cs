using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class ElGourdoProj : ModProjectile
    {
        float RotateSpeed = 0.2f;
        float ScaleAmount = 0.05f;
        int ScaleTimerLimit = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("El Gourdo");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 85;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
        }

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] == 180 || Projectile.ai[0] == 210 || Projectile.ai[0] == 240 || Projectile.ai[0] == 270)
            {
                Projectile.frame++;
            }
        }

        public override void AI()
        {
            Projectile.rotation += RotateSpeed * Projectile.direction; 

            Projectile.ai[0]++;

            //slow down
            if (Projectile.ai[0] >= 75)
            {
                Projectile.velocity *= 0.98f;
            }

            if (Projectile.ai[0] == 210 || Projectile.ai[0] == 240 || Projectile.ai[0] == 270)
            {
                ScaleAmount += 0.05f;
                ScaleTimerLimit -= 3;
            }

            //scale up and down before it explodes
            if (Projectile.ai[0] >= 180)
            {
                if (RotateSpeed >= 0)
                {
                    RotateSpeed -= 0.01f;
                }
                else
                {
                    RotateSpeed = 0f;
                    Projectile.rotation = 0;
                }

                Projectile.ai[1]++;
                if (Projectile.ai[1] < ScaleTimerLimit)
                {
                    Projectile.scale -= ScaleAmount;
                }
                if (Projectile.ai[1] >= ScaleTimerLimit)
                {
                    Projectile.scale += ScaleAmount;
                }

                if (Projectile.ai[1] > ScaleTimerLimit * 2)
                {
                    Projectile.ai[1] = 0;
                    Projectile.scale = 1.2f;
                }
            }

            //explode and die
            if (Projectile.ai[0] >= 300)
            {
                Projectile.Kill();
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y = -Projectile.velocity.Y;

			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            for (int numDust = 0; numDust < 50; numDust++)
			{                                                                                  
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-8f, 8f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-8f, 8f);
                Main.dust[dustGore].scale = Main.rand.NextFloat(2f, 3f);
                Main.dust[dustGore].noGravity = true;
			}

            Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);

            int[] Type = new int[] { ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3 };

            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
            {
                Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));
                Vector2 Position = new Vector2(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-50, 50));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int greekFire = Projectile.NewProjectile(Projectile.GetSource_Death(), Position, speed, 
                    Main.rand.Next(Type), Projectile.damage / 2, 0f, Main.myPlayer, 0, 0);
                    Main.projectile[greekFire].DamageType = DamageClass.Melee;
                    Main.projectile[greekFire].friendly = true;
                    Main.projectile[greekFire].hostile = false;
                }
            }

            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}