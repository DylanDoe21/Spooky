using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.NPCs.Boss.RotGourd;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{
	public class MoldSpore : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mold Spore");
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 500;
            Projectile.alpha = 255;
		}

		public override void AI()
		{
            Player player = Main.LocalPlayer;

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 5;
            }

			Projectile.rotation = Projectile.velocity.X * 0.1f;

			Projectile.ai[0]++;
            if (Projectile.ai[0] < 180)
            {
                float goToX = (player.Center.X + Main.rand.Next(-15, 15)) - Projectile.Center.X;
                float goToY = (player.Center.Y + Main.rand.Next(-15, 15)) - Projectile.Center.Y;
                float speed = 0.085f;

                if (Projectile.velocity.X < goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                    if (Projectile.velocity.X < 0f && goToX > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                    }
                }
                else if (Projectile.velocity.X > goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                    if (Projectile.velocity.X > 0f && goToX < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                    }
                }
                if (Projectile.velocity.Y < goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    if (Projectile.velocity.Y < 0f && goToY > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        return;
                    }
                }
                else if (Projectile.velocity.Y > goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    if (Projectile.velocity.Y > 0f && goToY < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        return;
                    }
                }
            }
            else
            {
                Projectile.velocity *= .97f;
            }
		}
	}
}