using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class CursedExplosion : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 176;
            Projectile.height = 180;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(Color.DarkGreen, Color.Lime, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 8f), Main.rand.NextFloat(1f, 8f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 5)
                {
                    Projectile.Kill();
                }
            }
			
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch);
            dust.noGravity = true;
            dust.scale = 1.6f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1 && Projectile.scale > 0.8f)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 0.5f, Pitch = -0.5f }, Projectile.Center);

                int newExplosion = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 75), 
                Vector2.Zero, ModContent.ProjectileType<CursedExplosion>(), Projectile.damage, 0f, Projectile.owner);
                Main.projectile[newExplosion].scale = Projectile.scale - 0.05f;
            }

            if (Projectile.ai[0] == 5)
            {
                Vector2 RandomPosition = new Vector2(Main.rand.Next(-(Projectile.width / 2), (Projectile.width / 2)), 0);

                //spawn smaller explosion
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + RandomPosition, 
                Vector2.Zero, ModContent.ProjectileType<CursedExplosionSmall>(), Projectile.damage / 2, 0f, Projectile.owner);

                //spawn fireball
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + RandomPosition, new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-35, -24)),
                ModContent.ProjectileType<CursedExplosionBall>(), Projectile.damage, 0f, Projectile.owner);
            }
        }
    }
}
