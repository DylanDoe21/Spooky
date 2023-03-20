using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Projectiles
{
	public class SentientTransform : ModProjectile
	{
        int ItemType = 0;

        Vector2 SaveProjectilePosition;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sentient Item Transform");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;                   			 
            Projectile.height = 32;  
            Projectile.friendly = true;       
			Projectile.hostile = false;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Yellow) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            //run down of ai

            //fade in and grow, while spawning particles around it that get sucked in


            switch ((int)Projectile.ai[1])
            {
                case 0:
                {
                    ItemType = ModContent.ItemType<SentientFleshAxe>();
                    break;
                }
                case 1:
                {
                    ItemType = ModContent.ItemType<SentientFleshBow>();
                    break;
                }
                case 2:
                {
                    ItemType = ModContent.ItemType<SentientFleshStaff>();
                    break;
                }
                case 3:
                {
                    ItemType = ModContent.ItemType<SentientFleshWhip>();
                    break;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
			/*
			//replace with the cool sparkly dust
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 100, default, 2f);
                Main.dust[DustGore].velocity *= 5f;
                Main.dust[DustGore].noGravity = false;
            }
			*/
		}
    }
}