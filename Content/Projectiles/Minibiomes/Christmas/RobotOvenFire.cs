using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
	public class RobotOvenFire : ModProjectile
	{
		public static int Lifetime => 96;
		public static int Fadetime => 80;

		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.ArmorPenetration = 10;
			Projectile.penetrate = 3;
			Projectile.MaxUpdates = 4;
			Projectile.timeLeft = 96;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			int size = (int)Utils.Remap(Projectile.ai[0], 0f, Fadetime, 10f, 45f);

			if (Projectile.ai[0] > Fadetime)
			{
				size = (int)Utils.Remap(Projectile.ai[0], Fadetime, Lifetime, 45f, 0f);
			}

			hitbox.Inflate(size, size);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Color color1 = Color.Yellow;
			Color color2 = color1;
			Color color3 = Color.Orange;
			Color color4 = Color.Red;
			
			float length = (Projectile.ai[0] > Fadetime - 10f) ? 0.1f : 0.15f;
			float vOffset = Math.Min(Projectile.ai[0], 20f);
			float timeRatio = Utils.GetLerpValue(0f, Lifetime, Projectile.ai[0]);
			float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

			if (timeRatio >= 1f)
			{
				return false;
			}

			for (float j = 1f; j >= 0f; j -= length)
			{
				Color fireColor = ((timeRatio < 0.1f) ? Color.Lerp(Color.Transparent, color1, Utils.GetLerpValue(0f, 0.1f, timeRatio)) :
				((timeRatio < 0.2f) ? Color.Lerp(color1, color2, Utils.GetLerpValue(0.1f, 0.2f, timeRatio)) :
				((timeRatio < 0.35f) ? color2 :
				((timeRatio < 0.7f) ? Color.Lerp(color2, color3, Utils.GetLerpValue(0.35f, 0.7f, timeRatio)) :
				((timeRatio < 0.85f) ? Color.Lerp(color3, color4, Utils.GetLerpValue(0.7f, 0.85f, timeRatio)) :
				Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(0.85f, 1f, timeRatio)))))));
				fireColor *= (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

				Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
				float mainRot = (-j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length) * Math.Sign(Projectile.velocity.X);
				float trailRot = MathHelper.PiOver4 - mainRot;

				Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, 49);
				Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

				Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
				Main.EntitySpriteDraw(ProjTexture.Value, firePos - trailOffset, rectangle, Projectile.GetAlpha(fireColor * 0.25f), trailRot, drawOrigin, fireSize, SpriteEffects.None);

				Main.EntitySpriteDraw(ProjTexture.Value, firePos, rectangle, Projectile.GetAlpha(fireColor), mainRot, drawOrigin, fireSize, SpriteEffects.None);
			}

			return false;
		}

		public override void AI()
        {
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 12)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 7)
			{
				Projectile.frame = 6;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] > Fadetime)
			{
				Projectile.velocity *= 0.95f;
			}

			if (Projectile.timeLeft < 20)
			{
				Projectile.alpha += 5;
			}
		}
	}
}