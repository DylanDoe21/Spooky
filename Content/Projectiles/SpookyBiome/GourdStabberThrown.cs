using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{ 
    public class GourdStabberThrown : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gourd Stabber");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 64;                   			 
            Projectile.height = 64;
            Projectile.DamageType = DamageClass.Melee;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 500;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Brown) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
		
		public override void AI()
        {
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.80f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 2)
            {
                Projectile.damage *= 2;
            }

            if (Projectile.ai[0] <= 12)
            {
                Projectile.tileCollide = false;
            }

            if (Projectile.ai[0] > 12)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] >= 45)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
            }
		}

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			for (int numDust = 0; numDust < 25; numDust++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
                
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}
     
          






