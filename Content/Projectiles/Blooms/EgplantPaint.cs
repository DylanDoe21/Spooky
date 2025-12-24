using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class EgplantPaint : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
		}

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[0];

            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation = Main.rand.Next(0, 360);

                Projectile.ai[1]++;
            }

            Projectile.alpha += 5;
        }
    }
}