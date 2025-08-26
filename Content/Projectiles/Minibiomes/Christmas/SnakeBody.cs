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
    public class SnakeBody : ModProjectile
    {
        bool SpawnedSegment = false;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 8;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true; 
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.ai[2]++;
            if (!SpawnedSegment && Projectile.ai[2] >= 2)
            {
                int Type = Projectile.ai[1] > 5 ? ModContent.ProjectileType<SnakeTail>() : ModContent.ProjectileType<SnakeBody>();

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2)), 
                Vector2.Zero, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI, ai1: Projectile.ai[1] + 1);

                SpawnedSegment = true;
            }

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

        public override void OnKill(int timeLeft)
		{
            foreach (var Proj in Main.ActiveProjectiles)
			{
                if ((Proj.type == ModContent.ProjectileType<SnakeBody>() || Proj.type == ModContent.ProjectileType<SnakeTail>()) && Proj.ai[0] == Projectile.whoAmI)
                {
                    Proj.Kill();
                }
            }
        }
    }
}