using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientRulerProj : ModProjectile
	{
		public int TotalDuration = 7;

		public float CollisionWidth => 30f * Projectile.scale;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> EffectTexture;

		public override void SetDefaults() 
        {
			Projectile.width = 72;
            Projectile.height = 200;
            Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
			Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
			Projectile.timeLeft = 360;
            Projectile.aiStyle = -1;
		}

		public override bool PreDraw(ref Color lightColor)
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            EffectTexture ??= ModContent.Request<Texture2D>(Texture + "Effect");

			float SlashAlpha = 1f - Math.Abs(Projectile.ai[0] / TotalDuration);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(EffectTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Color.White * SlashAlpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(EffectTexture.Value, Projectile.Center + new Vector2(-10, 10).RotatedBy(Projectile.rotation) - Main.screenPosition, rectangle, Color.White * SlashAlpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(EffectTexture.Value, Projectile.Center + new Vector2(10, 10).RotatedBy(Projectile.rotation) - Main.screenPosition, rectangle, Color.White * SlashAlpha, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];

			if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

			Projectile.spriteDirection = player.direction;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= TotalDuration) 
            {
				Projectile.Kill();
				return;
			}
			else
            {
				player.heldProj = Projectile.whoAmI;
			}

			//offset because the texture is big
			Vector2 heldCenterOffset = Vector2.Normalize(Projectile.velocity) * 80f;

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false) + heldCenterOffset;
			Projectile.Center = playerCenter + Projectile.velocity * (Projectile.ai[0] - 1f);
		}

		public override bool ShouldUpdatePosition() 
        {
			return false;
		}

		public override void CutTiles() 
        {
			Player player = Main.player[Projectile.owner];

			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 start = player.Center;
			Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 32f;
			Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			Player player = Main.player[Projectile.owner];

			Vector2 start = player.Center;
			Vector2 end = start + Projectile.velocity * 42f;
			float collisionPoint = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}
	}
}