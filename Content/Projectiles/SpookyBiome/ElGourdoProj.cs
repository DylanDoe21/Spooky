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
        int ScaleTimerLimit = 10;
        float ScaleAmount = 0.05f;
        float SaveRotation;
        bool Shake = false;

        public override void SetStaticDefaults()
        {
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }

			return false;
		}

        public override void AI()
        {
            Projectile.ai[0]++;

            //set projectile rotation randomly for variance
            if (Projectile.ai[0] == 1)
            {   
                Projectile.rotation = Main.rand.Next(0, 360);
            }

            //dont allow projectile to collide with tiles if you are close to the ground
            if (Projectile.ai[0] < 5)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            //slow down before getting ready to explode
            if (Projectile.ai[0] >= 75)
            {
                Projectile.velocity *= 0.98f;
            }

            //rotate projectile based on the direction it is moving in
            if (Projectile.ai[0] <= 180)
            {
                Projectile.rotation += 0.2f * Projectile.direction;
            }

            //save rotation before exploding
            if (Projectile.ai[0] == 180)
            {
                SaveRotation = Projectile.rotation;
            }

            //scale up and down before it explodes
            if (Projectile.ai[0] >= 180)
            {
                //shake the projectile back and fourth based on its last saved rotation before beginning to detonate
                if (Shake)
                {
                    Projectile.rotation += 0.1f;
                    if (Projectile.rotation > SaveRotation + 0.2f)
                    {
                        Shake = false;
                    }
                }
                else
                {
                    Projectile.rotation -= 0.1f;
                    if (Projectile.rotation < SaveRotation - 0.2f)
                    {
                        Shake = true;
                    }
                }

                //make projectile scale up and down really quickly
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

            //increase the rate and size at which the projectile scales for the above
            if (Projectile.ai[0] == 210 || Projectile.ai[0] == 240 || Projectile.ai[0] == 270)
            {
                ScaleAmount += 0.05f;
                ScaleTimerLimit -= 3;
            }

            //explode
            if (Projectile.ai[0] >= 300)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
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

            int[] Types = new int[] { ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3 };

            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
            {
                Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);
                Vector2 realSpeed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));
                Vector2 Position = new Vector2(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-50, 50));

                int GreekFire = Projectile.NewProjectile(Projectile.GetSource_Death(), Position, realSpeed, 
                Main.rand.Next(Types), Projectile.damage / 2, 0f, Main.myPlayer, 0, 0);
                Main.projectile[GreekFire].DamageType = DamageClass.Melee;
                Main.projectile[GreekFire].friendly = true;
                Main.projectile[GreekFire].hostile = false;
            }

            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool())
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}