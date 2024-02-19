using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class DiscoPartySkull : ModProjectile
    {
        int Offset = Main.rand.Next(-100, 100);

        Color[] ColorList = new Color[]
        {
            Color.Red, Color.OrangeRed, Color.Gold, Color.Lime, Color.Cyan, Color.Blue, Color.Indigo, Color.Fuchsia
        };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(ColorList[(int)Projectile.ai[1]]);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;

                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale + (fade / 2), effects, 0);
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Projectile.alpha, Projectile.alpha, Projectile.alpha, 0).MultiplyRGBA(ColorList[(int)Projectile.ai[1]]);
        }

        public override bool? CanDamage()
		{
            return Projectile.ai[0] > 120;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 120)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (Projectile.ai[0] < 120)
            {
                float goToX = player.Center.X + Offset - Projectile.Center.X;
                float goToY = player.Center.Y + Offset - Projectile.Center.Y;
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
            else
            {
                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    Projectile.tileCollide = false;

                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 15;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 500;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }
    }
}