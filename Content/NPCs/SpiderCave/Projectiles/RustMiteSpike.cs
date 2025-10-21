using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class RustMiteSpike : ModProjectile
    {
        public bool isAttacking = false;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[1];

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

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 7)
            {
				Projectile.velocity = Vector2.Zero;
			}
        }
    }
}