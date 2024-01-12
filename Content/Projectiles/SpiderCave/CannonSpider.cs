using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
			Projectile.height = 22;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = 900;
			AIType = ProjectileID.BabySpider;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}
	}
}