using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class GlowBulbProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glow Bulb");
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
            Projectile.velocity.Y *= 0;

			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            for (int numDust = 0; numDust < 15; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                
				if (Main.dust[DustGore].position != Projectile.Center)
                {
				    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
			}

            Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);

            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
            {
                Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, 
                    ModContent.ProjectileType<GlowBulbThorn>(), Projectile.damage, 0f, Main.myPlayer, 0, 0);
                }
            }
        }

        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 10; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default(Color), 1.5f);
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