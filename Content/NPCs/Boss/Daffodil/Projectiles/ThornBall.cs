using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class ThornBall : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lime) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            //add light for visibility
            Lighting.AddLight(Projectile.Center, 0.2f, 0.35f, 0f);

            if (Projectile.ai[1] < 180)
			{
			    Projectile.rotation += 0.12f * (float)Projectile.direction;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20 && Projectile.ai[1] < 180)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }

            Projectile.ai[1]++;
			if (Projectile.ai[1] >= 180)
			{
				Projectile.velocity *= 0.15f;

				if (Projectile.ai[2] == 0)
				{
                    Projectile.velocity *= 0;

					for (float numProjectiles = 0; numProjectiles < 7; numProjectiles++)
					{
						Vector2 projPos = Projectile.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / 7));

						Vector2 Direction = Projectile.Center - projPos;
						Direction.Normalize();

						Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0, 0,
						ModContent.ProjectileType<ThornBallSpike>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
					}

					Projectile.ai[2] = 1;
				}
			}

            if (Projectile.ai[1] >= 320)
			{
                SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

                Projectile.Kill();
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.ai[0] = 0;

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }

			return false;
		}
    }
}