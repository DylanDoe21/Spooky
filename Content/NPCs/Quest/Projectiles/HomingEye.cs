using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class HomingEye : ModProjectile
    {
        int target;

        private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;                   			 
            Projectile.height = 20;         
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 360;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Crimson);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), SpriteEffects.None, 0);
            }

            return true;
        }

		public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 20;
                }
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] % 60 >= 50)
            {
                if (Projectile.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    target = -1;
                    float distance = 2000f;
                    for (int k = 0; k < 255; k++)
                    {
                        if (Main.player[k].active && !Main.player[k].dead)
                        {
                            Vector2 center = Main.player[k].Center;
                            float currentDistance = Vector2.Distance(center, Projectile.Center);
                            if (currentDistance < distance || target == -1)
                            {
                                distance = currentDistance;
                                target = k;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.ai[1] = 1;
                        Projectile.netUpdate = true;
                    }
                }
                else if (target >= 0 && target < Main.maxPlayers)
                {
                    Player targetPlayer = Main.player[target];
                    if (!targetPlayer.active || targetPlayer.dead)
                    {
                        target = -1;
                        Projectile.ai[1] = 0;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        float currentRot = Projectile.velocity.ToRotation();
                        Vector2 direction = targetPlayer.Center - Projectile.Center;
                        float targetAngle = direction.ToRotation();
                        if (direction == Vector2.Zero)
                        {
                            targetAngle = currentRot;
                        }

                        float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot);
                    }
                }
            }
        }
    }
}