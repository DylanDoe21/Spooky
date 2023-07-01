using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class FuzzBatPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
            .WithOffset(-20f, -12f).WithSpriteDirection(-1).WithCode(CharacterPreviewCustomization);
        }

		public static void CharacterPreviewCustomization(Projectile proj, bool walking)
		{
			DelegateMethods.CharacterPreview.Float(proj, walking);
		}

        public override void SetDefaults()
		{
			Projectile.width = 60;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().FuzzBatPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().FuzzBatPet)
            {
				Projectile.timeLeft = 2;
            }

			float num16 = 0.5f;
            Projectile.tileCollide = false;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
            float vertiPos = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;
            vertiPos += (float)Main.rand.Next(-8, 8);
            horiPos += (float)Main.rand.Next(-8, 8);
            horiPos += (float)(45 * -(float)player.direction);
            vertiPos -= 45f;
            float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
            float num21 = 18f;
            float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

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
                num16 = 0.02f;
            }
            else
            {
                if (playerDistance < 100f)
                {
                    num16 = 0.35f;
                }
                if (playerDistance > 300f)
                {
                    num16 = 1f;
                }
                
                playerDistance = num21 / playerDistance;
                horiPos *= playerDistance;
                vertiPos *= playerDistance;
            }

            if (Projectile.velocity.X <= horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X + num16;
                if (num16 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num16;
                }
            }

            if (Projectile.velocity.X > horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X - num16;
                if (num16 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num16;
                }
            }

            if (Projectile.velocity.Y <= vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num16;
                if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num16 * 2f;
                }
            }

            if (Projectile.velocity.Y > vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num16;
                if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num16 * 2f;
                }
            }

            if (Projectile.Center.X < Main.player[Projectile.owner].Center.X)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.Center.X > Main.player[Projectile.owner].Center.X)
            {
                Projectile.spriteDirection = 1;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
		}
	}
}