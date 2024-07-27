using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.SpookyHell.Projectiles
{ 
    public class NautilusSpit1 : ModProjectile
    {
        int target;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 18;
            Projectile.height = 18;      
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;             					
            Projectile.timeLeft = 240;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.White);
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }
		
		public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 11)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			//fix Projectile direction
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 20)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] < 60)
                {
                    if (Projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
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
                            Projectile.ai[0] = 1;
                            Projectile.netUpdate = true;
                        }
                    }
                    else if (target >= 0 && target < Main.maxPlayers)
                    {
                        Player targetPlayer = Main.player[target];
                        if (!targetPlayer.active || targetPlayer.dead)
                        {
                            target = -1;
                            Projectile.ai[0] = 0;
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

                    Projectile.velocity *= 1.008f;
                }
            }
		}
    }

    public class NautilusSpit2 : NautilusSpit1
    {
        private static Asset<Texture2D> ProjTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.White);
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
     
          






