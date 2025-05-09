using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class Columboo : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 7)
            .WithOffset(-10f, 0f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
		}

        public override void SetDefaults()
		{
			Projectile.width = 34;
            Projectile.height = 44;
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
				player.GetModPlayer<SpookyPlayer>().ColumbooPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().ColumbooPet)
            {
				Projectile.timeLeft = 2;
            }

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 7)
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 4)
				{
					Projectile.frame = 0;
				}
			}

            Projectile.direction = player.direction;

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
            direction.X -= (2 + num * 20) * player.direction;
            direction.Y -= 45f;
            float distanceTo = direction.Length();

            if (distanceTo > 150f && speed < 15f)
            {
                speed = 15f;
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

                Projectile.ai[2]++;
                if (Projectile.ai[2] < 60)
                {
                    Projectile.rotation = Projectile.velocity.X * 0.05f;
                }
                else
                {
                    Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.05f * (float)Projectile.direction;
                }
            }
            else
            {
                Projectile.ai[2] = 0;

                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);

                Projectile.rotation = Projectile.rotation.AngleTowards(0, 0.1f);
            }

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
		}

        public override void OnKill(int timeLeft)
		{
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ColumbooStare>(), 0, 0f, Main.myPlayer);
        }
	}
}