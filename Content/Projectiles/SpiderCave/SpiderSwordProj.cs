using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class SpiderSwordProj : ModProjectile
	{
		public const int TotalDuration = 16;

		public float CollisionWidth => 30f * Projectile.scale;

		public override void SetDefaults() 
        {
			Projectile.width = 82;
            Projectile.height = 82;
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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			if (Main.rand.NextBool(4))
			{
            	target.AddBuff(BuffID.Poisoned, 300);
			}
        }

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];

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

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
			Projectile.Center = playerCenter + Projectile.velocity * (Projectile.ai[0] - 1f);

			Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;

			SetVisualOffsets();
		}

		private void SetVisualOffsets() 
        {
			const int HalfSpriteWidth = 82 / 2;
			const int HalfSpriteHeight = 82 / 2;

			int HalfProjWidth = Projectile.width / 2;
			int HalfProjHeight = Projectile.height / 2;

			DrawOriginOffsetX = 0;
			DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
			DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
		}

		public override bool ShouldUpdatePosition() 
        {
			return false;
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
			Vector2 end = start + Projectile.velocity * 20f;
			float collisionPoint = 0f;

			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
		}
	}
}