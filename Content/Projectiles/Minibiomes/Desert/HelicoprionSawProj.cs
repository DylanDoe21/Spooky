using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class HelicoprionSawProj : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
            Projectile.width = 38;
			Projectile.height = 38;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
            Projectile.hide = true;
			Projectile.aiStyle = 20;
			Projectile.penetrate = -1;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }
        }
	}
}