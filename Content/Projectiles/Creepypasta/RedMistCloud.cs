using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Creepypasta
{ 
    public class RedMistCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Mist");
            Main.projFrames[Projectile.type] = 5;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 26;                   			 
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Magic;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false; 
            Projectile.penetrate = -1;               					
            Projectile.timeLeft = 300;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = Color.Lerp(Color.Lerp(Color.Transparent, new Color(220, 20, 60, 0), fade), Color.Transparent, fade);


            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 77);
            Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(tex, vector, rectangle, color, Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            return true;
        }
		
		public override void AI()
        {
            Projectile.velocity *= 0;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
		}
    }
}
     
          






