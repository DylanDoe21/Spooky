using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MortarArmorRocket : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 48;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPosOld = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPosOld, rectangle, color * 0.75f, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
			return Projectile.ai[0] > 15;
		}

        public override void AI()       
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);
			Dust dust = Dust.NewDustPerfect(position, 133, Vector2.Zero);
			dust.noGravity = true;
            dust.noLight = true;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 15)
            {
                int foundTarget = FindTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 45;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.velocity *= 0.975f;
                }
            }
        }

        private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 1200;

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

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f }, Projectile.Center);

            float maxAmount = 15;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f));
                float intensity = Main.rand.NextFloat(1f, 10f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int num104 = Dust.NewDust(Projectile.Center, 0, 0, 133, 0f, 0f, 100, default, 1.5f);
                Main.dust[num104].noGravity = true;
                Main.dust[num104].position = Projectile.Center + vector12;
                Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }
		}
    }
}