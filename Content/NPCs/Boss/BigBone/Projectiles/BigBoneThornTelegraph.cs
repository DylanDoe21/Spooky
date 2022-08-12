using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class BigBoneThornTelegraph : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph");
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;  
            Projectile.aiStyle = -1;
        }
       
        public override Color? GetAlpha(Color lightColor)
		{
			Color[] ColorList = new Color[]
            {
                new Color(182, 62, 59),
                new Color(138, 31, 40)
            };

            float fade = Main.GameUpdateCount % 20 / 20f;
			int index = (int)(Main.GameUpdateCount / 20 % 2);
			return Color.Lerp(ColorList[index], ColorList[(index + 1) % 2], fade);
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 30)
            {
                Projectile.alpha -= 10;

                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
            
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.scale += 0.1f;
            }
            if (Projectile.ai[1] >= 10)
            {
                Projectile.scale -= 0.1f;
            }

            if (Projectile.ai[1] > 20)
            {
                Projectile.ai[1] = 0;
                Projectile.scale = 1f;
            }
        }
    }
}