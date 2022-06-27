using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class SpookyWisp : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spooky Wisp");
			Main.projFrames[Projectile.type] = 8;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
            Projectile.height = 24;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 999999999;
            Projectile.timeLeft *= 999999999;
            Projectile.penetrate = -1;
		}

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().SpookyWispPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().SpookyWispPet)
            {
				Projectile.timeLeft = 2;
            }

            Lighting.AddLight(Projectile.Center, 0.8f, 0.5f, 0f);

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) 
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 8) 
				{
					Projectile.frame = 0;
				}
			}

			if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 8f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 15f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.ai[1] = 3600f;
            Projectile.netUpdate = true;
            int num = 1;
            for (int k = 0; k < Projectile.whoAmI; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                {
                    num++;
                }
            }
            direction.X += (float)((10 + num * 40) * player.direction);
            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 9f)
            {
                speed = 9f;
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
            if (distanceTo > 48f)
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