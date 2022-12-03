using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class ControllableEyeBigExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.width = 240;
            Projectile.height = 240;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 1;
        }

        public override void AI()
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-18f, 18f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-18f, 18f);
                Main.dust[dustGore].scale = Main.rand.NextFloat(1f, 2f);
                Main.dust[dustGore].noGravity = true;
			}
        }
    }
}