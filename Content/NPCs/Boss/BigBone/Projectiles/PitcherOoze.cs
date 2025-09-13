using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class PitcherOoze1 : ModProjectile
	{
		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 12;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 300;
			Projectile.penetrate = 1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			if (Projectile.ai[0] == 0)
			{
				Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

				for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
				{
					float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
					Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(Color.Lerp(Color.Green, Color.YellowGreen, oldPos / (float)Projectile.oldPos.Length)) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
					Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
					Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
				}
			}

			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[0] == 0)
			{
				SoundEngine.PlaySound(SplatSound, Projectile.Center);

				Projectile.velocity.X *= 0;

				Projectile.ai[0]++;
			}

			return false;
		}

		public override void AI()
		{
			Projectile.frame = (int)Projectile.ai[0];
			
			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}

			if (Projectile.ai[0] == 0)
			{
				Projectile.rotation += 0.5f * (float)Projectile.direction;

				Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

				if (Projectile.Center.Y > target.Top.Y || Projectile.velocity.Y > 10f)
				{
					Projectile.ai[1] = 0;
				}
				else
				{
					Projectile.ai[1] = 1;
				}

				if (Projectile.velocity.X > -5f && Projectile.velocity.X < 5f)
				{
					Projectile.velocity.X += target.Center.X < Projectile.Center.X ? -0.12f : 0.12f;
				}

				if (Projectile.velocity.Y < 11f)
				{
					Projectile.velocity.Y += 0.2f;
				}

				Projectile.timeLeft = 600;
			}
			else
			{
				Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;

				Projectile.rotation = 0;

				if (Main.rand.NextBool(5))
				{
					Color color = Projectile.type == ModContent.ProjectileType<PitcherOoze2>() ? Color.Magenta : Color.YellowGreen;

					int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, color * 0.5f, Main.rand.NextFloat(0.1f, 0.2f));
					Main.dust[DustEffect].position.Y += -10 * 0.05f - 1.5f;
                    Main.dust[DustEffect].velocity.X *= 0.1f;
					Main.dust[DustEffect].velocity.Y = -1;
					Main.dust[DustEffect].alpha = 125;
				}
			}
		}
	}

	public class PitcherOoze2 : PitcherOoze1
	{
		private static Asset<Texture2D> ProjTexture;

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			if (Projectile.ai[0] == 0)
			{
				Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

				for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
				{
					float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
					Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(Color.Lerp(Color.Purple, Color.Magenta, oldPos / (float)Projectile.oldPos.Length)) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
					Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
					Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
				}
			}

			return true;
		}
	}
}