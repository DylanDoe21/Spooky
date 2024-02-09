using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;

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
                if (Main.npc[i].Hitbox.Intersects(Projectile.Hitbox) && Projectile.ai[1] % 20 == 5)
                {
                    int damageDone = player.GetModPlayer<SpookyPlayer>().AnalogHorrorTape ? Main.rand.Next(80, 100) : Main.rand.Next(10, 15);
                    int lifeStealDivider = player.GetModPlayer<SpookyPlayer>().AnalogHorrorTape ? 5 : 2;

                    player.ApplyDamageToNPC(Main.npc[i], damageDone, 0, 0, false, null, false);
                    player.statLife += damageDone / lifeStealDivider;
                    player.HealEffect(damageDone / lifeStealDivider, true);
                }
            }
        }
    }
}