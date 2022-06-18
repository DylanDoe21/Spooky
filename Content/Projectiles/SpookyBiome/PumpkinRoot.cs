using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class PumpkinRoot : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Root Spike");
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			//Projectile.hide = true;
            Projectile.timeLeft = 180;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;	
			Projectile.alpha = 255;
		}

		/*
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
		*/

		public override void AI()
		{
			//decrease alpha when timeleft is low
            if (Projectile.timeLeft <= 60) 
            {
				Projectile.alpha += 15;

				if (Projectile.alpha >= 255)
				{
					Projectile.alpha = 255;
				}
			}
			else 
            {
				Projectile.alpha -= 50;

				if (Projectile.alpha <= 0)
				{
					Projectile.alpha = 0;
				}
			}
            
			//other ai
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