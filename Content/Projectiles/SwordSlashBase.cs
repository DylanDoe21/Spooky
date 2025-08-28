using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles
{
    public class SwordSlashBase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.usesOwnerMeleeHitCD = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 2;
            Projectile.aiStyle = -1;
        }

        private void UpdateEnchantmentVisuals() 
        {
			if (Projectile.npcProj) 
            {
				return;
			}

			for (float num = -(float)Math.PI / 4f; num <= (float)Math.PI / 4f; num += (float)Math.PI / 2f) 
            {
				Rectangle r = Utils.CenteredRectangle(Projectile.Center + (Projectile.rotation + num).ToRotationVector2() * 70f * Projectile.scale, new Vector2(60f * Projectile.scale, 60f * Projectile.scale));
				Projectile.EmitEnchantmentVisualsAt(r.TopLeft(), r.Width, r.Height);
			}
		}

        public override void AI()
        {
            Projectile.localAI[0]++;
            Player player = Main.player[Projectile.owner];
            float num = Projectile.localAI[0] / Projectile.ai[1];
            float num2 = Projectile.ai[0];
            float Fade = Projectile.velocity.ToRotation();
            Projectile.rotation = (float)Math.PI * num2 * num + Fade + num2 * (float)Math.PI + player.fullRotation;

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;

            /*
            float scaleMulti = 0.2f;
			float scaleAdder = 1f;
            Projectile.scale = scaleAdder + num * scaleMulti;
            */

            float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
            Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 vector3 = (num8 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();

            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.Kill();
            }

            if (!Projectile.noEnchantmentVisuals) 
            {
				UpdateEnchantmentVisuals();
			}
        }

        public override bool? CanCutTiles() 
        {
            return true;
        }
    }
}