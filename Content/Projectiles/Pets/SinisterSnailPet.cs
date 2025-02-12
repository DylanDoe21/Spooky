using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
    public class SinisterSnailPet : ModProjectile
    {
        private int playerStill = 0;
        private bool playerFlying = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 11;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 4, 8)
            .WithOffset(-25f, 0f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 450f;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().SinisterSnailPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().SinisterSnailPet)
            {
				Projectile.timeLeft = 2;
            }

            if (!playerFlying)
            {
                Projectile.rotation = 0;
                Vector2 center2 = Projectile.Center;
                Vector2 vector48 = player.Center - center2;
                float playerDistance = Projectile.Distance(player.Center);

                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -7f;
                }

                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (playerDistance > 450f)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }

                if (playerDistance > 90f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.12f;
                        if (Projectile.velocity.X > 5.5f)
                        {
                            Projectile.velocity.X = 5.5f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.12f;
                        if (Projectile.velocity.X < -5.5f)
                        {
                            Projectile.velocity.X = -5.5f;
                        }
                    }
                }

                if (playerDistance < 90f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.8f)
                        {
                            Projectile.velocity.X -= 0.25f;
                        }
                        else if (Projectile.velocity.X < -0.8f)
                        {
                            Projectile.velocity.X += 0.25f;
                        }
                        else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }

                if (playerDistance < 80f)
                {
                    Projectile.velocity.X *= 0.95f;
                }

                //set frames when idle
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 8)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 5;
                    Projectile.frameCounter = 0;
                }
                //moving animation
                else if (Projectile.velocity.X != 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 2)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 8)
                    {
                        Projectile.frame = 4;
                    }
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }
            }
            else if (playerFlying)
            {
                float num16 = 0.5f;
                Projectile.tileCollide = false;
                Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
                float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)(60 * -(float)player.direction);
                vertiPos -= 60f;
                float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
                float num21 = 18f;
                float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }

                if (playerDistance < 100f)
                {
                    num16 = 0.5f;
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

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 5)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 11)
                {
                    Projectile.frame = 8;
                }
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}