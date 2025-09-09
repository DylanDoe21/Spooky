using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class ZombieEelBody : ModProjectile
    {
        public int segmentIndex = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.DamageType = DamageClass.Summon;
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

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<ZombieEelTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<ZombieEelTail>().segmentIndex;
                    projectile.ModProjectile<ZombieEelTail>().segmentIndex++;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[0];

			if (player.channel && player.active && !player.dead && !player.noItems && !player.CCed)
            {
                Projectile.timeLeft = 2;
            }
        }

        public void SegmentMove()
        {
            var live = false;

            Projectile nextSegment = new Projectile();
            ZombieEelHead head = new ZombieEelHead();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var projectile = Main.projectile[i];
                if (projectile.type == Type && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (projectile.ModProjectile<ZombieEelBody>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<ZombieEelHead>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    
                    head = projectile.ModProjectile<ZombieEelHead>();
                    Projectile.direction = projectile.direction;
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
                destinationOffset = destinationOffset.RotatedBy(angle * 0.35f);
            }

            Projectile.rotation = destinationOffset.ToRotation();

            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center + nextSegment.velocity - destinationOffset.SafeNormalize(Vector2.Zero) * 42f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ZombieEelHead>()] > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    var projectile = Main.projectile[i];
                    if (projectile.type == ModContent.ProjectileType<ZombieEelHead>() && projectile.owner == Projectile.owner && projectile.active)
                    {
                        projectile.Kill();
                    }
                }
            }
        }
    }
}
