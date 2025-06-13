using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class SpearfishLanceMetalProj : ModProjectile
    {
        public float CollisionWidth => 30f * Projectile.scale;

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void CutTiles() 
        {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			Vector2 start = Projectile.Center;
			Vector2 end = start + Projectile.velocity * 3.75f;
			float collisionPoint = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.direction = player.direction;
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = playerRelativePoint;

            if (!player.active || player.dead || player.noItems || player.CCed)
            {
                Projectile.Kill();
            }

            Projectile.spriteDirection = Projectile.direction = player.direction;

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 127;
                
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            if (Projectile.localAI[0] > 0f)
            {
                Projectile.localAI[0] -= 1f;
            }

            float inverseAnimationCompletion = 1f - (player.itemAnimation / (float)player.itemAnimationMax);
            float originalVelocityDirection = Projectile.velocity.ToRotation();
            float originalVelocitySpeed = Projectile.velocity.Length();

            Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + inverseAnimationCompletion * MathHelper.TwoPi) * new Vector2(originalVelocitySpeed, Projectile.ai[0]);

            Projectile.position += flatVelocity.RotatedBy(originalVelocityDirection) + new Vector2(originalVelocitySpeed + 12f, 0f).RotatedBy(originalVelocityDirection);

            Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(originalVelocityDirection) + originalVelocityDirection.ToRotationVector2() * (originalVelocitySpeed + 12f + 75f);
            Projectile.rotation = player.AngleTo(destination) + MathHelper.PiOver4 * player.direction;

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            if (player.itemAnimation == 1)
            {
                Projectile.Kill();
                player.reuseDelay = 1;
            }
        }
    }
}