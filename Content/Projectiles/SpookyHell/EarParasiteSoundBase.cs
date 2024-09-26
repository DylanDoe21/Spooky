using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class EarParasiteSoundBase : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 45;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 8 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EarParasiteSound>(), Projectile.damage, 5f, Main.myPlayer);
            }
        }
    }
}