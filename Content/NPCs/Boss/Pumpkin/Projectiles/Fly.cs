using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.NPCs.Boss.Pumpkin;

namespace Spooky.Content.NPCs.Boss.Pumpkin.Projectiles
{
	public class Fly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fly");
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 999999999;
            Projectile.timeLeft *= 999999999;
            Projectile.alpha = 100;
            Projectile.scale = 0.85f;
            Projectile.aiStyle = -1;
		}

		public override void AI()
		{
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            int index1 = (int)Projectile.ai[1];
            
            if (Main.npc[index1].active && (Main.npc[index1].type == ModContent.NPCType<SpookyPumpkin>() || Main.npc[index1].type == ModContent.NPCType<SpookyPumpkinP2>())) 
            {
                float goToX = Main.npc[index1].Center.X - Projectile.Center.X + Main.rand.Next(-200, 200);
                float goToY = Main.npc[index1].Center.Y - Projectile.Center.Y + Main.rand.Next(-200, 200);

                float speed = 0.08f;

                if (Vector2.Distance(Projectile.Center, Main.npc[index1].Center) >= 135)
                {
                    speed = 3f;
                }
                else
                {
                    speed = 2f; //was 0.08
                }
                
                if (Projectile.velocity.X > speed)
                {
                    Projectile.velocity.X *= 0.98f;
                }
                if (Projectile.velocity.Y > speed)
                {
                    Projectile.velocity.Y *= 0.98f;
                }

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
                Projectile.alpha++;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.08f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }
		}
	}
}