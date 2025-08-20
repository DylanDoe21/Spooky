using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
    public class SnowCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {   
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 75;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }
        
        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.08f;

            if (Main.rand.NextBool(5))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SmokeEffect>(), Projectile.velocity * 0.1f);
                dust.scale = Main.rand.NextFloat(0.25f, 0.45f);
                dust.color = Color.White * 0.5f;
                dust.alpha = 150;
            }
        }
    }
}