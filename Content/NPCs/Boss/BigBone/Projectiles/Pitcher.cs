using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class Pitcher1 : ModProjectile
    {
		float SaveVelocity = 0;

		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 38;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = 1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Projectile Parent = Main.projectile[(int)Projectile.ai[1]];

			if (!Parent.active || Parent.type != ModContent.ProjectileType<VineBase>())
			{
				Projectile.Kill();
			}

			Projectile.spriteDirection = SaveVelocity < 0 ? -1 : 1;

			Projectile.Center = Parent.Center + new Vector2(Projectile.spriteDirection == -1 ? -8 : 8, -15);

			Projectile.timeLeft = 5;

			if (Parent.localAI[0] <= 35)
			{
				SaveVelocity = Parent.velocity.X;

				if (Projectile.scale < 1f && Parent.localAI[0] > 10)
				{
					Projectile.scale += 0.1f;
				}
			}
			else
			{
				Projectile.ai[0]++;
				if (Projectile.ai[0] > 30 && Projectile.ai[0] < 60)
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 4 && Projectile.frame < 2)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;
					}
				}

				if (Projectile.ai[0] == 60)
				{
					SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

					for (int numProj = 0; numProj < 2; numProj++)
					{
						int ShootSpeedX = Main.rand.Next(-6, 7);
						int ShootSpeedY = Main.rand.Next(-10, -7);

						int ProjType = Main.rand.NextBool() ? ModContent.ProjectileType<PitcherOoze1>() : ModContent.ProjectileType<PitcherOoze2>();

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Top, new Vector2(ShootSpeedX, ShootSpeedY), ProjType, Projectile.damage, Projectile.knockBack);
					}
				}

				if (Projectile.ai[0] > 75)
				{
					Projectile.frameCounter++;
					if (Projectile.frameCounter >= 4 && Projectile.frame > 0)
					{
						Projectile.frameCounter = 0;
						Projectile.frame--;
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			if (Main.netMode != NetmodeID.Server) 
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/PitcherGore1").Type);
			}
        }
	}

	public class Pitcher2 : Pitcher1
	{
		private static Asset<Texture2D> ProjTexture;

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			if (Main.netMode != NetmodeID.Server) 
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/PitcherGore2").Type);
			}
        }
	}
}