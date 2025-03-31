using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class BigBoneFlySmall : ModProjectile
    {
        int Offset = Main.rand.Next(-100, 100);

        private static Asset<Texture2D> AfterImageTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
			Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            AfterImageTexture ??= TextureAssets.Extra[98];
            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Color color1 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Gray);
            Color color2 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Black);

            float TrailRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			TrailRotation += 0f * Projectile.direction;

			for (int oldPos = 2; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Color newColor = Color.Lerp(color1, color2, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;

				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 0.72f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                for (int repeats = 0; repeats < 2; repeats++)
                {
                    Main.EntitySpriteDraw(AfterImageTexture.Value, drawPos, null, newColor, TrailRotation, AfterImageTexture.Size() / 2f, scale, SpriteEffects.None);
                }
            }

            return true;
        }

        public override void AI()
        {
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

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

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
                NPC Parent = Main.npc[(int)Projectile.ai[1]];

                if (Parent.active && Parent.type == ModContent.NPCType<BigBone>())
                {
                    float goToX = Parent.Center.X + Offset - Projectile.Center.X;
                    float goToY = Parent.Center.Y + Offset - Projectile.Center.Y;
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

            if (Projectile.ai[0] == 75)
            {
                Projectile.tileCollide = true;
                
                if (Projectile.ai[2] == 0)
                {
                    double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                    Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;
                }
                else
                {
                    Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                    Vector2 newVelocity = Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));
                    Projectile.velocity = newVelocity;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath47 with { Volume = 0.25f }, Projectile.Center);
		}
    }
}