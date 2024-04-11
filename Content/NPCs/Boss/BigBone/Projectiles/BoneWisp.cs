using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class BoneWisp : ModProjectile
    {
        int Offset = Main.rand.Next(-100, 100);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
			Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[98].Value;
            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Color color1 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);
            Color color2 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Gold);

			for (int oldPos = 1; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Color newColor = Color.Lerp(color1, color2, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;

				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(tex, drawPos, null, newColor, Projectile.rotation + (float)Math.PI / 2f, tex.Size() / 2f, scale, SpriteEffects.None);
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
                    if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
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
                Projectile.tileCollide = true;
                
                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
        
        	if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/BoneWispGore").Type);
            }
		}
    }
}