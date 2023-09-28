using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class PandoraRosaryHand : ModProjectile
    {
        float RotationSpeed = 2f;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Cyan);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                var effects = Projectile.Center.X < player.Center.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 3 * 6 + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, effects, 0);
            }

            return true;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = Projectile.Center.X < player.Center.X ? -1 : 1;

            Vector2 vector92 = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float num740 = player.Center.X - vector92.X;
            float num741 = player.Center.Y - vector92.Y;
            Projectile.rotation = (float)Math.Atan2((double)num741, (double)num740) + 4.71f;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }

            if (player.dead || !player.GetModPlayer<SpookyPlayer>().PandoraRosary)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.timeLeft = 300;

                Projectile.ai[2]++;

                if (Projectile.ai[2] >= 60)
                {
                    RotationSpeed = player.direction == -1 ? Main.rand.NextFloat(-3f, -0.5f) : Main.rand.NextFloat(0.5f, 3f);

                    Projectile.ai[2] = 0;
                }

                Projectile.ai[1] += RotationSpeed;
                int distance = 250;
                double rad = Projectile.ai[1] * (Math.PI / 180);
                Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
            }
            else
            {
                float goToX = player.Center.X - Projectile.Center.X;
                float goToY = player.Center.Y - Projectile.Center.Y;
                float speed = 0.35f;
                
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

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath6, Projectile.Center);

                    player.statLife += 15;
                    player.HealEffect(15, true);

                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.HallowSpray, 0f, 0f, 100, default, Main.rand.NextFloat(1f, 2f));
                Main.dust[dust].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
		}
    }
}