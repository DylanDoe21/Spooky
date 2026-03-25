using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
    public class ZomboidBruteSlam : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
			Projectile.width = 45;
            Projectile.height = 100;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 30;
            Projectile.alpha = 255;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.velocity.Y = Main.rand.Next(-20, -9) * target.knockBackResist;
		}
		
		public override void AI()
        {
			Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
			Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
			_ = scanAreaStart.X / 2;
			_ = scanAreaEnd.X / 2;
			int explosionRange = Projectile.width;
			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1f)
			{
				Projectile.CreateImpactExplosion(2, Projectile.Bottom, ref scanAreaStart, ref scanAreaEnd, explosionRange, out var causedShockwaves);
			}
		}
    }
}