using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class SpiritHand : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
		}

		public override void SetDefaults()
		{
            Projectile.width = 20;
			Projectile.height = 34;
			Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
			Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
		{
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			if (Projectile.timeLeft <= 60) 
            {
				Projectile.alpha += 5;
			}
			else 
            {
				Projectile.alpha -= 15;
			}

            if (Projectile.alpha >= 255)
            {
                Projectile.alpha = 255;
            }

            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }

			if (Projectile.ai[0] > 7) 
            {
				Projectile.velocity = Vector2.Zero;
			}
			else 
            {
				Projectile.ai[0]++;
				Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			}
		}
	}
}