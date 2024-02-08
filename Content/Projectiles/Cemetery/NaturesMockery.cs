using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class NaturesMockery : ModProjectile
    {
        public bool isAttacking = false;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
            Projectile.timeLeft = 420;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[2];

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
			}

            Projectile.ai[1]++;

            //lifesteal from enemies
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].Hitbox.Intersects(Projectile.Hitbox) && Projectile.ai[1] % 30 == 5)
                {
                    int damageDone = Main.rand.Next(5, 15); //Projectile.frame == 4 ? 80 : 10;

                    player.ApplyDamageToNPC(Main.npc[i], damageDone, 0, 0, false, null, false);
                    player.statLife += damageDone / 2;
                    player.HealEffect(damageDone / 2, true);
                }
            }
        }
    }
}