using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class CoughCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.alpha = 255;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 60);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gold * 0.5f, Main.rand.NextFloat(0.2f, 0.4f));
                Main.dust[DustEffect].velocity *= 0;
                Main.dust[DustEffect].alpha = 100;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.velocity *= 0;

            return false;
        }
    }
}