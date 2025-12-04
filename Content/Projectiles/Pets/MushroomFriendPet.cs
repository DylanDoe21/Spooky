using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Pets
{
    public class MushroomFriendPet : ModProjectile
    {
        private int playerStill = 0;
        private bool playerFlying = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> CapTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 10;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 6, 5)
            .WithOffset(-10f, 0f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 66;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            Player player = Main.player[Projectile.owner];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			CapTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(CapTexture.Value, vector, rectangle, Projectile.GetAlpha(player.shirtColor.MultiplyRGBA(lightColor)), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            fallThrough = Projectile.Bottom.Y < player.Top.Y;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().MushroomFriendPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().MushroomFriendPet)
            {
				Projectile.timeLeft = 2;
            }

            float playerDistance = player.Distance(Projectile.Center);

            bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, player.position, player.width, player.height);

            if (!playerFlying)
            {
                //prevents the pet from getting stuck on sloped tiled
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

                Projectile.rotation = 0;

                if (Projectile.velocity.Y == 0 && Projectile.Bottom.Y > player.Top.Y && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -10f;
                }

                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (!lineOfSight)
                {
                    Projectile.ai[0]++;
                }

                if (playerDistance > 450f || Projectile.ai[0] >= 30)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }

                if (playerDistance > 75f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.12f;
                        if (Projectile.velocity.X > 6.5f)
                        {
                            Projectile.velocity.X = 6.5f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.12f;
                        if (Projectile.velocity.X < -6.5f)
                        {
                            Projectile.velocity.X = -6.5f;
                        }
                    }
                }

                if (playerDistance < 75f)
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

                if (playerDistance < 50f)
                {
                    Projectile.velocity.X *= 0.5f;
                }

                //set frames when idle
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                    Projectile.frameCounter = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 3;
                    Projectile.frameCounter = 0;
                }
                //moving animation
                else if (Projectile.velocity.X != 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 8 - (Projectile.velocity.X > 0 ? Projectile.velocity.X : -Projectile.velocity.X))
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 7)
                    {
                        Projectile.frame = 1;
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
                Projectile.tileCollide = false;

                Projectile.ai[0] = 0;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			    Projectile.rotation += 0f * (float)Projectile.direction;

                if (Main.rand.NextBool())
                {
                    Vector2 position = new Vector2(Projectile.Center.X + Main.rand.Next(-30, 31), Projectile.Center.Y + Main.rand.Next(-30, 31));
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<SmokeEffect>(), -Projectile.velocity / 3);
                    dust.color = player.shirtColor;
                    dust.scale = Main.rand.NextFloat(0.2f, 0.3f);
                }

                Vector2 Destination = player.Center - new Vector2(0, 30);
                bool DestinationLineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Destination, 2, 2);

                if (Projectile.Distance(Destination) > 50f)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(Destination) * 15;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
                else
                {
                    if (playerDistance < 100f)
                    {
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
                }

                if (playerDistance > 1200f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 3)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame < 7)
                {
                    Projectile.frame = 7;
                }
                if (Projectile.frame >= 10)
                {
                    Projectile.frame = 7;
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