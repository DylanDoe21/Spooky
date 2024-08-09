using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class CannonSpider : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.BabySpider);
            Projectile.width = 30;
			Projectile.height = 20;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 3;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 900;
			AIType = ProjectileID.BabySpider;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

		public override void AI()
		{
			if (Projectile.velocity.X == 0)
			{
				Projectile.frame = 0;
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            for (int numGores = 1; numGores <= 2; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/CannonSpiderGore" + numGores).Type);
				}
			}
        }
	}
}