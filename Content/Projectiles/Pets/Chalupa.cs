using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
    public class Chalupa : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 3)
            .WithOffset(-3f, -15f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().ChalupoPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().ChalupoPet)
            {
				Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 0;
            }

            if (Projectile.Center.X < player.Center.X)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.Center.X > player.Center.X)
            {
                Projectile.spriteDirection = 1;
            }

            float Speed = 0.35f;
            float horiPos = player.Center.X - Projectile.Center.X;
            float vertiPos = player.Center.Y - Projectile.Center.Y;
            vertiPos += (float)Main.rand.Next(-10, 15);
            horiPos += (float)Main.rand.Next(-3, 4);
            horiPos += (float)(60 * -(float)player.direction);
            vertiPos -= 25f;

            float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

            if (playerDistance > 1200f)
            {
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }

            if (playerDistance < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }

                Speed = 0.02f;
            }
            else
            {
                if (playerDistance < 150f)
                {
                    Speed = 0.1f;
                }
                if (playerDistance > 400f)
                {
                    Speed = 0.25f;
                }
                
                playerDistance = 18f / playerDistance;
                horiPos *= playerDistance / 2;
                vertiPos *= playerDistance / 2;
            }

            if (Projectile.velocity.X <= horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X + Speed;
                if (Speed > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + Speed;
                }
            }

            if (Projectile.velocity.X > horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X - Speed;
                if (Speed > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - Speed;
                }
            }

            if (Projectile.velocity.Y <= vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + Speed;
                if (Speed > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + Speed * 2f;
                }
            }

            if (Projectile.velocity.Y > vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - Speed;
                if (Speed > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - Speed * 2f;
                }
            }
        }
    }
}