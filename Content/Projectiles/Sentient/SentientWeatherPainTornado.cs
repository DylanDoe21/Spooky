using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientWeatherPainTornado : ModProjectile
	{
		public float shift = 0;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> EyeTexture;

		public override void SetDefaults()
		{
			Projectile.width = 50;
			Projectile.height = 200;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 360;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		//this is literally just decompile vanilla code from the tome of wisdom tornado projectile visual effect
        //I tried to translate the variable names as best as I could, probably still isnt the best though
		public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			float ProjectileMaxTime = 360f;
			float AI0 = Projectile.ai[0];

            //seems to be used for color fading when the projectile is about to die?
			float ColorScale = MathHelper.Clamp(AI0 / 30f, 0f, 1f);
			if (AI0 > ProjectileMaxTime - 60f)
			{
				ColorScale = MathHelper.Lerp(1f, 0f, (AI0 - (ProjectileMaxTime - 60f)) / 60f);
			}

			Rectangle TornadoFrame = ProjTexture.Frame(1, 1, 0, 0);
            Vector2 TornadoOrigin = TornadoFrame.Size() / 2f;

			float FloatMultiplier = 0.2f; //this is defined here in the decompiled code, its purpose just seems to be a multiplier for other variables
			Vector2 top = Projectile.Top;
			Vector2 bottom = Projectile.Bottom;
			Vector2.Lerp(top, bottom, 0.5f);
			Vector2 BottomToTop = new Vector2(0f, bottom.Y - top.Y);
			BottomToTop.X = BottomToTop.Y * FloatMultiplier;
			new Vector2(top.X - BottomToTop.X / 2f, top.Y);

			float ProjRotationValue = 0.1f * AI0 * (float)((Projectile.velocity.X > 0f) ? -1 : 1);

            double AIMultipliedValue = (double)(AI0 * 0.14f);
            Vector2 center = default;
            Vector2 VectorUnitY = Vector2.UnitY;
			Vector2 vector50 = VectorUnitY.RotatedBy(AIMultipliedValue, center);

			float num264 = 0f;
			float TornadoLoopIncrement = 10f;

            bool ProjMovingRight = Projectile.velocity.X > 0f;

            SpriteEffects effects = (Projectile.velocity.X > 0f) ? SpriteEffects.FlipVertically : SpriteEffects.None;

			Color Color = Color.White;

			for (float num267 = (int)bottom.Y; num267 > (int)top.Y; num267 -= TornadoLoopIncrement)
			{
				num264 += TornadoLoopIncrement;
				float num268 = num264 / BottomToTop.Y;
				float num269 = num264 * 6.28318548f / -20f;

				switch (Projectile.ai[1])
				{
					case 0:
					{
						Color = Color.Lerp(Color.RoyalBlue, Color.Red, num268);
						break;
					}
					case 1:
					{
						Color = Color.Lerp(Color.DarkOrchid, Color.RoyalBlue, num268);
						break;
					}
					case 2:
					{
						Color = Color.Lerp(Color.Red, Color.Lime, num268);
						break;
					}
					case 3:
					{
						Color = Color.Lerp(Color.Lime, Color.DarkOrchid, num268);
						break;
					}
				}

				if (ProjMovingRight)
				{
					num269 *= -1f;
				}
				float num270 = num268 - (0.35f / num267 * 10);
				Vector2 arg_CDC3_0 = vector50;
				double arg_CDC3_1 = (double)num269;
				center = default(Vector2);
				Vector2 vector51 = arg_CDC3_0.RotatedBy(arg_CDC3_1, center);
				Vector2 vector52 = new Vector2(0f, num268 + 1f);
				vector52.X = vector52.Y * FloatMultiplier;
				Color color51 = Color.Lerp(Color.Transparent, Color, num268 * 2f);
				color51.A = (byte)((float)color51.A * 0.5f);
				color51 *= ColorScale;
				vector51 *= vector52 * 100f;
				vector51.Y = 0f;
				vector51.X = 0f;
				vector51 += new Vector2(bottom.X, num267) - Main.screenPosition;

				shift += 5;
				float offset = (float)Math.Sin(shift + (num264 / 10) / 0.5) * 5f;
				Vector2 drawPos = vector51;
				drawPos.X += offset;

				//the top few segments of the tornado should not draw with sin offset, so it doesnt look too odd
				if (num267 <= (int)top.Y + 30)
				{
					Main.spriteBatch.Draw(ProjTexture.Value, vector51, new Rectangle?(TornadoFrame), color51 * 0.5f, ProjRotationValue + num269, TornadoOrigin, num270 * 0.52f, effects, 0f);
				}
				//rest of the tornado should draw with sin offset
				else
				{
					if (num267 <= (int)bottom.Y - 30)
					{
						Main.spriteBatch.Draw(ProjTexture.Value, drawPos, new Rectangle?(TornadoFrame), color51 * 0.5f, ProjRotationValue + num269, TornadoOrigin, num270 * 0.5f, effects, 0f);
					}
				}
			}

			return false;
		}

		public override void AI()
		{
			float num = 360f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (Projectile.localAI[0] >= 16f && Projectile.ai[0] < num - 15f)
			{
				Projectile.ai[0] = num - 15f;
			}
			Projectile.ai[0] += 1f;
			if (Projectile.ai[0] >= num)
			{
				Projectile.Kill();
			}
			Vector2 top = Projectile.Top;
			Vector2 bottom = Projectile.Bottom;
			Vector2 value = Vector2.Lerp(top, bottom, 0.5f);
			Vector2 vector = new Vector2(0f, bottom.Y - top.Y);

			if (Projectile.ai[0] < num - 30f)
			{
				for (int j = 0; j < 1; j++)
				{
					float value2 = -1f;
					float value3 = 0.9f;
					float amount = Main.rand.NextFloat();
					Vector2 vector2 = new Vector2(MathHelper.Lerp(0.1f, 1f, Main.rand.NextFloat()), MathHelper.Lerp(value2, value3, amount));
					vector2.X *= MathHelper.Lerp(2.2f, 0.6f, amount);
					vector2.X *= -1f;
					Vector2 value4 = new Vector2(6f, 10f);
					Vector2 position2 = value + vector * vector2 * 0.5f + value4;
				}
			}

			Projectile.velocity *= 0.95f;

			foreach (NPC npc in Main.ActiveNPCs)
			{
				if (npc.active && npc.CanBeChasedBy(this) && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type] && npc.Distance(Projectile.Center) <= 250f)
				{
					if (!npc.boss && !npc.IsTechnicallyBoss())
					{
                        if (npc.Distance(Projectile.Center) <= 150 && npc.Distance(Projectile.Center) > 60f)
                        {
                            Vector2 ChargeDirection = Projectile.Center - npc.Center;
                            ChargeDirection.Normalize();

                            ChargeDirection *= 0.15f / (Vector2.Distance(npc.Center, Projectile.Center) / 400);
                            npc.velocity.X += ChargeDirection.X;
                        }

                        if (npc.Hitbox.Intersects(Projectile.Hitbox))
                        {
                            npc.velocity.Y -= 0.5f;
                        }
                    }
				}
			}
		}
	}
}