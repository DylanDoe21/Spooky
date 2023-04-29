using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class GlowBulbProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Glow Bulb");
        }
        
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

        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
			    Projectile.rotation += 0.25f * (float)Projectile.direction;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.ai[1] = 1;

			Projectile.velocity.X *= 0;

			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int numDust = 0; numDust < 12; numDust++)
			{                                                                                  
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dustGore].scale = Main.rand.NextFloat(1f, 1.25f);
                Main.dust[dustGore].noGravity = true;
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
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenMoss, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[DustGore].position != Projectile.Center)
                {
				    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
			}
        }
    }
}