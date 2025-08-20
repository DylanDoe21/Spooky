using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{ 
    public class Knife : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 10;
            Projectile.height = 34;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
		}
		
		public override void AI()
        {
            Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            Projectile.ai[0]++;
            
            //make it spin
            if (Projectile.ai[0] < 60)
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

            //make it charge at the player
            if (Projectile.ai[0] == 60)
            {
                double Velocity = Math.Atan2(target.Center.Y - Projectile.Center.Y, target.Center.X - Projectile.Center.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 10;
            }

            if (Projectile.ai[0] > 60)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;
            }
		}

		public override void OnKill(int timeLeft)
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

    public class KnifeCleaver : Knife
    {
    }
}