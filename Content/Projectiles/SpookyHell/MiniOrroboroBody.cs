using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class MiniBoroBody : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody";

        public int segmentIndex = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = false;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
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

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<MiniBoroTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<MiniBoroTail>().segmentIndex;
                    projectile.ModProjectile<MiniBoroTail>().segmentIndex++;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorMouth)
			{
				Projectile.timeLeft = 2;
			}
        }

        public void SegmentMove()
        {
            var live = false;

            Projectile nextSegment = new Projectile();
            MiniBoroHead head = new MiniBoroHead();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var projectile = Main.projectile[i];
                if (projectile.type == Type && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (projectile.ModProjectile<MiniBoroBody>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<MiniBoroHead>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    
                    head = projectile.ModProjectile<MiniBoroHead>();
                }
            }
            
            if (!live) 
            {
                Projectile.Kill();
            }

            Vector2 destinationOffset = nextSegment.Center+nextSegment.velocity - Projectile.Center;

            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
            }

            Projectile.rotation = destinationOffset.ToRotation();

            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center+nextSegment.velocity - destinationOffset.SafeNormalize(Vector2.Zero) * 20f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MiniBoroHead>()] > 0)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    var projectile = Main.projectile[i];
                    if (projectile.type == ModContent.ProjectileType<MiniBoroHead>() && projectile.owner == Projectile.owner && projectile.active)
                    {
                        projectile.Kill();
                    }
                }
            }
        }
    }

    public class MiniOrroBody : MiniBoroBody
    {
        public override string Texture => "Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody";

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<MiniOrroTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<MiniOrroTail>().segmentIndex;
                    projectile.ModProjectile<MiniOrroTail>().segmentIndex++;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorEye)
			{
				Projectile.timeLeft = 2;
			}
        }

        public void SegmentMove()
        {
            Player player = Main.player[Projectile.owner];
            var live = false;

            Projectile nextSegment = new Projectile();
            MiniOrroHead head = new MiniOrroHead();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var projectile = Main.projectile[i];
                if (projectile.type == Type && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (projectile.ModProjectile<MiniOrroBody>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<MiniOrroHead>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    
                    head = projectile.ModProjectile<MiniOrroHead>();
                }
            }
            
            if (!live) 
            {
                Projectile.Kill();
            }

            Vector2 destinationOffset = nextSegment.Center+nextSegment.velocity - Projectile.Center;

            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
            }

            Projectile.rotation = destinationOffset.ToRotation();

            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center+nextSegment.velocity - destinationOffset.SafeNormalize(Vector2.Zero) * 20f;
            }
        }
    }
}
