using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class HomingFlower : ModProjectile
    {
		int homingTarget;
		
		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 52;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BouncingFlowerTrail");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color1 = Color.Gold;
			Color color2 = Color.Brown;

			switch (Projectile.frame)
			{
				case 0:
				{
					color1 = new Color(163, 205, 65);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 1:
				{
					color1 = new Color(106, 92, 208);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 2:
				{
					color1 = new Color(251, 46, 114);
			 		color2 = color1 * 0.5f;
					break;
				}
				case 3:
				{
					color1 = new Color(250, 122, 70);
			 		color2 = color1 * 0.5f;
					break;
				}
			}

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1.1f;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Color.Lerp(color1, color2, oldPos / (float)Projectile.oldPos.Length) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, Projectile.GetAlpha(color), Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

			if (Projectile.ai[2] == 0)
			{
				double Velocity = Math.Atan2(target.position.Y - Projectile.position.Y, target.position.X - Projectile.position.X);
				Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 16;

				Projectile.ai[2] = 1;
			}

			Projectile.ai[0]++;
			if (Projectile.ai[0] <= 15)
			{
				if (Projectile.scale < 1f && Projectile.ai[0] > 10)
				{
					Projectile.scale += 0.1f;
				}
			}

			if (Projectile.timeLeft < 60)
			{
				Projectile.alpha += 5;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

			SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

			double Velocity = Math.Atan2(target.position.Y - Projectile.position.Y, target.position.X - Projectile.position.X);
			Vector2 actualSpeed = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 14;

			Projectile.velocity = actualSpeed;

			return false;
		}
    }
}