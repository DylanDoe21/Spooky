using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{ 
    public class FlowerSpore : ModProjectile
    {
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];

		private static Asset<Texture2D> TrailTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 20;                   			 
            Projectile.height = 24;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, Color.Orange, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

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

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}

            Projectile.localAI[0]++;
            
            //make Projectile spin
            if (Projectile.localAI[0] > 30 && Projectile.localAI[0] < 75)
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

                Projectile.velocity *= 0.97f;
            }

            //make projectile charge at the player
            if (Projectile.localAI[0] == 75)
            {
                Projectile.tileCollide = true;

                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12; //12 is the "charge" speed
            }

            if (Projectile.localAI[0] > 75 || Projectile.localAI[0] < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;	
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
        
        	for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 1f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;

                if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}