using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class AcidBreath : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] > 30)
            {
                if (Main.rand.NextBool(3))
                {
                    int DustEffect = Dust.NewDust(Projectile.Center, Projectile.width / 5, Projectile.height / 5, 
                    ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.5f, Main.rand.NextFloat(0.35f, 0.65f));
                    Main.dust[DustEffect].velocity *= 0;
                    Main.dust[DustEffect].alpha = 100;
                }
            }
        }
    }
}