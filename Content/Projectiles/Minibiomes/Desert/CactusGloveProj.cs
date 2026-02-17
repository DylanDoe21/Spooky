using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class CactusGloveProj : ModProjectile
	{
		public int TotalDuration = 8;

		public float CollisionWidth => 30f * Projectile.scale;

		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults() 
        {
			Projectile.width = 22;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
			Projectile.hide = true;
			Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
			Projectile.timeLeft = 360;
            Projectile.aiStyle = -1;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		
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
			Vector2 heldCenterOffset = Vector2.Normalize(Projectile.velocity) * 2f;

			Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false) + heldCenterOffset;
			Projectile.Center = playerCenter + Projectile.velocity * (Projectile.ai[0] - 1f);
		}

		public override bool ShouldUpdatePosition() 
        {
			return false;
		}
	}
}