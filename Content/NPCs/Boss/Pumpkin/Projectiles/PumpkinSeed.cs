using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Pumpkin.Projectiles
{ 
    public class PumpkinSeed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Seed");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 18;                   			 
            Projectile.height = 18;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Pumpkin/Projectiles/PumpkinSeedTrail").Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Brown) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
		
		public override void AI()
        {
            
            
            if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}

            Projectile.localAI[0]++;
            
            //make Projectile spin
            if (Projectile.localAI[0] < 60)
            {
                Projectile.spriteDirection = Projectile.velocity.X  > 0 ? 1 : -1;
                
                if (Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                if (Projectile.velocity.X < 0)
                {
                    Projectile.rotation += -0.35f;
                }

                Projectile.velocity *= 0.98f;
            }

            //make Projectile charge at the player
            if (Projectile.localAI[0] == 60)
            {
                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 10; //10 is the "charge" speed
            }

            if (Projectile.localAI[0] > 60)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;	
            }
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 262, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}