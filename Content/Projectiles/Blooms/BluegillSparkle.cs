using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class BluegillSparkle : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

            //after images
            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                Vector2 afterImageVector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Main.EntitySpriteDraw(ProjTexture.Value, afterImageVector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White * 0.5f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 60;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                float maxAmount = 10;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                    float intensity = Main.rand.NextFloat(1f, 3f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                    int newDust = Dust.NewDust(Projectile.Center, 0, 0, 132, 0f, 0f, 100, default, 1f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].position = Projectile.Center + vector12;
                    Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                    currentAmount++;
                }
            }

            if (Projectile.ai[0] >= 60)
            {
                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 12;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 600;

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