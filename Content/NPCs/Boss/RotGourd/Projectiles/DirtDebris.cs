using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{ 
    public class DirtDebris : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dirt Debris");
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 16;                   			 
            Projectile.height = 16;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 240;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Brown);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }
		
		public override void AI()
        {
			//fix Projectile direction
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 60)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }	

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

			int newDust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.Dirt, 0f, 0f, 0, default(Color), 1f);
			Main.dust[newDust].position = position;
            Main.dust[newDust].scale = 1.2f;
			Main.dust[newDust].fadeIn = 0.5f;
			Main.dust[newDust].noGravity = true;
		}

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        
        	for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
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
     
          






