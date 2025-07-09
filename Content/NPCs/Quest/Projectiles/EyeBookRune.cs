using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
	public class EyeBookRune : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                writer.WriteVector2(trailLength[i]);
            }

            //bools
            writer.Write(runOnce);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                trailLength[i] = reader.ReadVector2();
            }

            //bools
            runOnce = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
			Projectile.width = 22;
			Projectile.height = 22;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);

					if (Projectile.timeLeft <= 90)
					{
						color *= (Projectile.timeLeft) / 90f;
					}

					Main.EntitySpriteDraw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override void PostDraw(Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Color color = Color.Lime;

			if (Projectile.timeLeft <= 45)
			{
				color *= (Projectile.timeLeft) / 45f;
			}

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
		}

		public override void AI()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

			Projectile.frame = (int)Projectile.ai[1];

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

				runOnce = false;

				Projectile.netUpdate = true;
			}

			//save previous positions, rotations, and direction
			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;

				current = previousPosition;
			}

			Projectile.ai[0]++;
            if (Projectile.ai[0] <= 30)
            {
                Projectile.velocity *= 0.95f;
            }
           	if (Projectile.ai[0] > 30 && Projectile.ai[0] <= 100)
            {
                Projectile.velocity *= 1.075f;
            }
		}

		public override void OnKill(int timeLeft)
		{
			//SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
		}
	}
}