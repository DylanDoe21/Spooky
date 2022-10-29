using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class CatacombKey1Proj : ModProjectile
	{
        Vector2 SaveProjectilePosition;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Key");
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;                   			 
            Projectile.height = 10;  
            Projectile.friendly = true;       
			Projectile.hostile = false;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
		}

		public override void AI()
		{
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] <= 60)
			{
                Projectile.velocity.X *= 1.005f;
            }

            if (Projectile.ai[0] == 62)
			{
				SaveProjectilePosition = Projectile.Center;
			}

			if (Projectile.ai[0] > 62 && Projectile.ai[0] < 180)
			{
				Projectile.Center = new Vector2(SaveProjectilePosition.X, SaveProjectilePosition.Y);
				Projectile.Center += Main.rand.NextVector2Square(-4, 4);
			}
		}
	}
}