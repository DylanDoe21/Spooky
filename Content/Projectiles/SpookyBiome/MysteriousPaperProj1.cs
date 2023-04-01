/*
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.PlayerDrawLayer;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class MysteriousPaperProj1 : ModProjectile
    {

        public override void SetStaticDefaults()
        {

            DisplayName.SetDefault("Purple Paper");

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {

            Projectile.width = 27;

            Projectile.height = 25;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.aiStyle = -1;

            Projectile.tileCollide = true;

            Projectile.friendly = true;
        }

        private bool rotChanged = false;
        public override void AI()
        {
            if (!rotChanged)
            {
                Projectile.rotation = Projectile.DirectionTo(Main.MouseWorld).ToRotation() - MathHelper.PiOver2;
                rotChanged = true;
            }
            Projectile.velocity.X *= 0.982f;
            Projectile.velocity.Y += 0.809f;
        }
    }
}
*/