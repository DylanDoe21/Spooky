using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class OldWoodStaffProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyBiome/OldWoodStaff";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 52;
            Projectile.height = 58;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
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

            if (player.itemAnimation == player.itemAnimationMax / 3)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 muzzleOffset = Vector2.Normalize(new Vector2(Projectile.velocity.X, Projectile.velocity.Y)) * 45f;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), playerRelativePoint + Projectile.velocity * 0.8f + muzzleOffset,
                    Projectile.velocity * 1.5f, ModContent.ProjectileType<OldWoodStaffBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
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