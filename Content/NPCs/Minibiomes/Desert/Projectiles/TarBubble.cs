using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Desert.Projectiles
{
    public class TarBubble : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2000;
			Projectile.scale = 0.1f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, (ProjTexture.Height() / 3) * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
        {
			if (Collision.WetCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.velocity.Y = -0.5f;
			}
			else
			{
				Projectile.velocity = Vector2.Zero;

				if (Projectile.scale < 1)
				{
					Projectile.scale += 0.05f;
				}

				Projectile.ai[0]++;

				if (Projectile.ai[0] > 25)
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 7)
					{
						Projectile.frame++;
						Projectile.frameCounter = 0;
						if (Projectile.frame >= 3)
						{
							Projectile.frame = 0;
							Projectile.Kill();
						}
					}
				}
			}
        }

		public override void OnKill(int timeLeft)
		{
			if (Main.LocalPlayer.Distance(Projectile.Center) <= 200f)
			{
				float ActualVolume = Main.LocalPlayer.Distance(Projectile.Center);

				SoundEngine.PlaySound(SoundID.Item95.WithVolumeScale(1f).WithPitchOffset(1.2f), Projectile.Center);
			}
		}
	}
}