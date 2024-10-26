using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Pets
{
    public class PuttyPet1 : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];

            fallThrough = Projectile.position.Y < player.Center.Y - Projectile.height;

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().PuttyPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().PuttyPet)
            {
				Projectile.timeLeft = 2;
            }

            if (!playerFlying)
            {
                //set frames when idle
                if (Projectile.velocity.Y == 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 6)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 2)
                    {
                        Projectile.frame = 0;
                    }
                }
                //up frame
                else if (Projectile.velocity.Y < 0)
                {
                    Projectile.frame = 1;
                    Projectile.frameCounter = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0)
                {
                    Projectile.frame = 2;
                    Projectile.frameCounter = 0;
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.rotation = 0;

                Projectile.velocity.Y += 0.35f;

                Projectile.tileCollide = true;

                //slow down a bit while falling after jumping
                if (Projectile.velocity.Y >= 0)
                {
                    Projectile.velocity.X *= 0.98f;
                }
                
                //slow down quickly while on the ground
                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) < 120f)
                {
                    Projectile.velocity.X *= 0.8f;
                }

                if (Projectile.velocity.Y == 0.35f && Projectile.Distance(player.Center) >= 120f)
                {
                    JumpTo(null, player);
                }

                if (Projectile.Distance(player.Center) >= 450f)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }
            }
            else
            {
                Projectile.frame = 2;
                Projectile.frameCounter = 0;

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                Projectile.tileCollide = false;

                float Speed = 0.5f;
                float horiPos = player.Center.X - Projectile.Center.X;
                float vertiPos = player.Center.Y - Projectile.Center.Y;
                vertiPos += (float)Main.rand.Next(-10, 15);
                horiPos += (float)Main.rand.Next(-10, 15);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;

                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

                if (playerDistance < 100f)
                {
                    Speed = 0.5f;
                    if (player.velocity.Y == 0f)
                    {
                        playerStill++;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        playerFlying = false;
                        Projectile.velocity *= 0.2f;
                        Projectile.tileCollide = true;
                    }
                }

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
                    horiPos *= playerDistance;
                    vertiPos *= playerDistance;
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

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                int[] Putties = new int[] { ModContent.ProjectileType<PuttyPet1>(), ModContent.ProjectileType<PuttyPet2>(), ModContent.ProjectileType<PuttyPet3>() };

                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && Putties.Contains(other.type) && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.15f;
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
		}

        public void JumpTo(NPC target, Player player)
        {
            Vector2 JumpTo = target == null ? new Vector2(player.Center.X, player.Center.Y - 100) : new Vector2(target.Center.X, target.Center.Y - 200);

            Vector2 velocity = JumpTo - Projectile.Center;

            float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 15);
            velocity.Normalize();
            velocity.Y -= 0.12f;
            velocity.X *= target == null ? 0.5f : 0.75f;
            Projectile.velocity = velocity * speed * 1.1f;
        }
    }

    public class PuttyPet2 : PuttyPet1
    {
    }

    public class PuttyPet3 : PuttyPet1
    {
    }
}