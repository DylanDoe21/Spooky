using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class WormySegment : ModProjectile
    {
        public int segmentIndex = 1;

        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 5;
		}	

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override bool? CanDamage()
		{
			return false;
		}

		public override void OnSpawn(IEntitySource source)
        {
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<WormyTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<WormyTail>().segmentIndex;
                    projectile.ModProjectile<WormyTail>().segmentIndex++;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.GetModPlayer<BloomBuffsPlayer>().Wormy)
			{
				Projectile.timeLeft = 2;
			}
        }

        public void SegmentMove()
        {
            var live = false;

            Projectile nextSegment = new Projectile();
            WormyHead head = new WormyHead();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var projectile = Main.projectile[i];
                if (projectile.type == Type && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (projectile.ModProjectile<WormySegment>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<WormyHead>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    
                    head = projectile.ModProjectile<WormyHead>();
                }
            }
            
            if (!live) 
            {
                Projectile.Kill();
            }

            Vector2 destinationOffset = nextSegment.Center + nextSegment.velocity - Projectile.Center;

            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
            }

            Projectile.rotation = destinationOffset.ToRotation();

			//how far each segment should be from each other
            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center + nextSegment.velocity - destinationOffset.SafeNormalize(Vector2.Zero) * 10f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<WormyHead>()] > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    var projectile = Main.projectile[i];
                    if (projectile.type == ModContent.ProjectileType<WormyHead>() && projectile.owner == Projectile.owner && projectile.active)
                    {
                        projectile.Kill();
                    }
                }
            }
        }
    }
}
