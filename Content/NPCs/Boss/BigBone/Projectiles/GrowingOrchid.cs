using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class GrowingOrchid : ModProjectile
    {
		Vector2 SaveVelocity = Vector2.Zero;

		Color trailColor = Color.White;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 66;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Trail");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1.1f;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = trailColor * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			Projectile Parent = Main.projectile[(int)Projectile.ai[2]];

			if (!Parent.active || Parent.type != ModContent.ProjectileType<VineBase>())
			{
				Projectile.Kill();
			}

			if (Parent.velocity != Vector2.Zero && Parent.localAI[0] < 35)
			{
				SaveVelocity = Parent.velocity;
			}

			Projectile.rotation = SaveVelocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			Projectile.frame = (int)Projectile.ai[1];

			Projectile.Center = Parent.Center;

			if (Projectile.ai[0] == 0)
			{
				Color[] Colors = { Color.Gold, Color.White, Color.Magenta, Color.Crimson, Color.HotPink, Color.Orange, Color.ForestGreen };

				trailColor = Main.rand.Next(Colors);

				Projectile.ai[0]++;
			}

			if (Projectile.scale < 1f)
			{
				Projectile.scale += 0.1f;
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			if (Projectile.frame < 2)
			{
				for (int numGores = 1; numGores <= 5; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/OrchidPinkBigGore" + numGores).Type);
					}
				}
			}
			else
			{
				for (int numGores = 1; numGores <= 5; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/OrchidPurpleBigGore" + numGores).Type);
					}
				}
			}
		}
    }
}