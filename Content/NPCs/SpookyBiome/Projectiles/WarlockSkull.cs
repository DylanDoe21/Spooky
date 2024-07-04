using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{ 
    public class WarlockSkull : ModProjectile
    {
        int Offset = Main.rand.Next(-20, 20);

        private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;                   			 
            Projectile.height = 16;         
			Projectile.hostile = true;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Indigo);

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

            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0f);

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 75)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (Projectile.ai[0] < 75)
            {
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].active && (Main.npc[k].type == ModContent.NPCType<ZomboidWarlock>()))
                    {
                        float goToX = Main.npc[k].Center.X + Offset - Projectile.Center.X;
                        float goToY = Main.npc[k].Center.Y + Offset - Projectile.Center.Y;
                        float speed = 0.12f;

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
                }
            }

            if (Projectile.ai[0] == 75)
            {
                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 8;
            }

            if (Projectile.ai[0] > 100)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }
        }
    }
}