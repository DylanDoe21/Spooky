using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class CoughSmokeCloudSmall : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
			Projectile.localNPCHitCooldown = 80;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

		public override void AI()
		{
			int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CoughCloudDust>(), 0f, 0f, 100, Color.DarkGray * 0.5f, Main.rand.NextFloat(0.3f, 0.6f));
			Main.dust[DustEffect].velocity *= 0;
			Main.dust[DustEffect].alpha = 100;
        }
    }
}