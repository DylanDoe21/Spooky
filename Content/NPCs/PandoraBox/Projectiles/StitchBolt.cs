using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.PandoraBox.Projectiles
{
    public class StitchBolt : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 14;                   			 
            Projectile.height = 14;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 180;
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
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

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

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, Color.Cyan, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}
		
		public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 10)
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
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 20)
            {   
                Projectile.velocity *= 0.97f;
            }
            else
            {
                Projectile.velocity *= 1.05f;
            }
		}
    }
}
     
          






