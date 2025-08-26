using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class SnakeTail : ModProjectile
    {
        bool SpawnedSegment = false;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true; 
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile SegmentParent = Main.projectile[(int)Projectile.ai[0]];

            Projectile.alpha = SegmentParent.alpha;

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - Projectile.Center;

			if (SegmentParent.rotation != Projectile.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - Projectile.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			Projectile.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				Projectile.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 7f;
			}
        }
    }
}