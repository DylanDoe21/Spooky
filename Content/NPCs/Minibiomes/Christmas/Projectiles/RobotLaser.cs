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

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
	public class RobotLaser : ModProjectile
	{
		public override string Texture => "Spooky/Content/Projectiles/Blank";

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[22];
		Rectangle[] trailHitboxes = new Rectangle[22];

		float SaveRotation;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SendExtraAI(BinaryWriter writer)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                writer.WriteVector2(trailLength[i]);
            }

            //bools
            writer.Write(runOnce);

            //floats
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
			writer.Write(Projectile.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			for (int i = 0; i < trailLength.Length; i++)
            {
                trailLength[i] = reader.ReadVector2();
            }

            //bools
            runOnce = reader.ReadBoolean();

            //floats
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
			Projectile.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 180;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
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

                    Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

					if (Projectile.timeLeft <= 90)
					{
						color *= (Projectile.timeLeft) / 90f;
					}

					Main.EntitySpriteDraw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0f);
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

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
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
				trailHitboxes[i] = new Rectangle((int)current.X - 3, (int)current.Y - 3, 6, 6);

				current = previousPosition;
			}

			if (trailHitboxes[0].X == trailHitboxes[trailLength.Length - 1].X && trailHitboxes[0].Y == trailHitboxes[trailLength.Length - 1].Y)
			{
				Projectile.Kill();
			}
		}
	}
}