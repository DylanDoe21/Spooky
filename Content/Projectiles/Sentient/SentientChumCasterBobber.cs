using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientChumCasterBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Bobber;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }
    }
}
