using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
	public class IrisPetalLockOn : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 68;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
			return false;
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            Projectile.position = new Vector2(Main.MouseWorld.X - (Projectile.width / 2), Main.MouseWorld.Y - (Projectile.height / 2));

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SpringIris)
            {
                Projectile.Kill();
            }
        }
    }
}