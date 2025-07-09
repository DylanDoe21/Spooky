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
	public class EyeMinionBeam : ModProjectile
	{
		public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[60];
		Rectangle[] trailHitboxes = new Rectangle[60];

		private static Asset<Texture2D> TrailTexture;

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
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 120;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>(Texture);

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

                    for (int j = 0; j < 360; j += 90)
                    {
                        Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

						if (Projectile.timeLeft <= 90)
						{
							color *= (Projectile.timeLeft) / 90f;
						}

                        Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(j));

                        Main.EntitySpriteDraw(TrailTexture.Value, drawPos + circular, null, color, Projectile.rotation, drawOrigin, scale * 1.5f, SpriteEffects.None, 0f);
                    }
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			bool CollidingWithProj = false;

			if (!runOnce)
			{
				for (int i = 0; i < trailHitboxes.Length; i++)
				{
					if (trailHitboxes[i] != Rectangle.Empty && targetHitbox.Intersects(trailHitboxes[i]))
					{
						CollidingWithProj = true;
						break;
					}
					else
					{
						CollidingWithProj = false;
					}
				}
			}

			return targetHitbox.Intersects(projHitbox) || CollidingWithProj;
		}

		public override void AI()
		{
			Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					trailHitboxes[i] = Rectangle.Empty;
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
				trailHitboxes[i] = new Rectangle((int)current.X - 5, (int)current.Y - 5, 5, 5);

				current = previousPosition;
			}

			if (Projectile.timeLeft < 90)
			{
				Projectile.velocity = Vector2.Zero;
			}
		}

		public override void OnKill(int timeLeft)
		{
			//SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
		}
	}
}