using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class CoughSmokeCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
			Projectile.localNPCHitCooldown = 60;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(5))
			{
				target.AddBuff(ModContent.BuffType<SmokerCoughDebuff>(), Main.rand.Next(300, 600));
			}
		}

		public override void AI()
		{
			int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CoughCloudDust>(), 0f, 0f, 100, Color.DarkGray * 0.5f, Main.rand.NextFloat(0.3f, 0.6f));
			Main.dust[DustEffect].velocity *= 0;
			Main.dust[DustEffect].alpha = 100;
        }
    }
}