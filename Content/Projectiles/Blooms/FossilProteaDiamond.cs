using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
    public class FossilProteaDiamond : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

        public override void AI()       
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

        /*
		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f }, Projectile.Center);

            for (int numDusts = 0; numDusts <= 5; numDusts++)
            {
                int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gold * 0.5f, Main.rand.NextFloat(0.5f, 0.75f));
                Main.dust[DustEffect].velocity *= 0;
                Main.dust[DustEffect].alpha = 100;
            }
		}
        */
    }
}