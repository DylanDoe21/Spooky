using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
    public class CursedFlamePillar : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[100];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 50;                   			 
            Projectile.height = 50;
            Projectile.friendly = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;        
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 100;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length + k) / (float)trailLength.Length;
				scale *= 1f;

                float scaleForLerp = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
                Color color = Color.Lerp(Color.DarkGreen, Color.Lime, scaleForLerp);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					for (int j = 0; j < 360; j += 90)
					{
						Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f)).RotatedBy(MathHelper.ToRadians(j));

						Main.spriteBatch.Draw(ProjTexture.Value, drawPos + circular, null, color * 0.25f, Projectile.rotation, drawOrigin, scale * 0.9f, SpriteEffects.None, 0f);
					}
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 3;

            target.AddBuff(BuffID.CursedInferno, 240);
        }

        public override void AI()
        {
            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

            Projectile.ai[1]++;

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity.Y = -5f;

                SpookyPlayer.ScreenShakeAmount = 2;

                //spawn dusts
                for (int numDust = 0; numDust < 50; numDust++)
				{                                                                                  
					int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, Color.Lime, 1.5f);
					Main.dust[dust].velocity.X *= Main.rand.NextFloat(-1f, 1f);
					Main.dust[dust].velocity.Y -= Main.rand.NextFloat(1f, 12f);
					Main.dust[dust].scale = 0.1f; 
					Main.dust[dust].noGravity = true;
				}

                Projectile.ai[0] = 1;
            }

            if (Projectile.ai[1] > 60)
            {
                Projectile.velocity *= 0;

                //trailWidth -= 2;
            }
        }
    }
}
     
          






