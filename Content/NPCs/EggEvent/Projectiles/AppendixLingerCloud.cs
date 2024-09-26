using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class AppendixLingerCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Ichor, 180);
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CoughCloudDust>(), 0f, 0f, 100, Color.Gold * 0.5f, Main.rand.NextFloat(0.3f, 0.6f));
            Main.dust[DustEffect].velocity *= 0;
            Main.dust[DustEffect].alpha = 100;
        }
    }
}