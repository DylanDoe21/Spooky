using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class GhostPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 16;
            Projectile.height = 22;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().GhostPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().GhostPet)
            {
				Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) 
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 2) 
				{
					Projectile.frame = 0;
				}
			}
            
            Lighting.AddLight(Projectile.Center, 0.85f, 0.5f, 0f);

			if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 6f;

            if (Projectile.ai[0] == 1f)
            {
                speed = 9f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.ai[1] = 3600f;
            Projectile.netUpdate = true;
            direction.X += (20 * player.direction);
            direction.Y -= 50f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 12f)
            {
                speed = 12f;
            }
            if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 30f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
		}
	}
}