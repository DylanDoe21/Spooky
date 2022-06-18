using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class Toothball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
         	DisplayName.SetDefault("Toothball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;      
            Projectile.height = 22;   
			Projectile.friendly = false;  
            Projectile.hostile = true;         
            Projectile.tileCollide = false; 
			Projectile.ignoreWater = true;     
            Projectile.penetrate = -1;   
            Projectile.timeLeft = 600;
        }

        public override void AI()       
        {
			Projectile.spriteDirection = Projectile.velocity.X  > 0 ? 1 : -1;
			
			if (Projectile.spriteDirection == 1)
			{
				Projectile.rotation += 0.05f;
			}
			
			if (Projectile.spriteDirection == -1)
			{
				Projectile.rotation += -0.05f;
			}

			Projectile.localAI[0]++;

			if (Projectile.localAI[0] < 80)
			{
				Projectile.tileCollide = false;
			}

			if (Projectile.localAI[0] >= 80)
			{
				Projectile.aiStyle = 2;
			}

			if (Projectile.localAI[0] >= 190)
			{
				Projectile.tileCollide = true;
			}
        }
		
		int Bounces = 5;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces--;
			if (Bounces <= 0)
			{
				Projectile.Kill();
			}
			else
			{
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.velocity.X = -oldVelocity.X * 0.95f;
				}
				if (Projectile.velocity.Y != oldVelocity.Y)
				{
					Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
				}
			}
			return false;
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{                                                                                  
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 75, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				
				if (Main.dust[num].position != Projectile.Center)
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 2f;
				}
            }

			SoundEngine.PlaySound(SoundID.NPCDeath12, Projectile.Center);
		}
    }
}