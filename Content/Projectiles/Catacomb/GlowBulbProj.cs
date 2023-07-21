using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class GlowBulbProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.ai[1] = 1;

			Projectile.velocity.X *= 0;

			return false;
		}

        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
			    Projectile.rotation += 0.25f * (float)Projectile.direction;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 5)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int numDust = 0; numDust < 12; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dust].velocity.Y *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 1.25f);
                Main.dust[dust].noGravity = true;
			}

            Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);

            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
            {
                Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                float damageDivide = 1.5f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, 
                    ModContent.ProjectileType<GlowBulbThorn>(), Projectile.damage / (int)damageDivide, 0f, Main.myPlayer, 0, 0);
                }
            }
        }

        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 10; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenMoss, 0f, -2f, 0, default, 1.5f);
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