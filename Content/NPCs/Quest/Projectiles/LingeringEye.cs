using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class LingeringEye : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Indigo);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void AI()
        {
            Player Target = Main.player[(int)Projectile.ai[2]];

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 120 && Projectile.ai[0] < 180)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] < 3)
                {
                    Projectile.scale -= 0.2f;
                }
                if (Projectile.ai[1] >= 3)
                {
                    Projectile.scale += 0.2f;
                }
                
                if (Projectile.ai[1] > 6)
                {
                    Projectile.ai[1] = 0;
                    Projectile.scale = 1f;
                }
            }

            if (Projectile.ai[0] == 180)
            {
                Projectile.scale = 1f;

                Vector2 ShootSpeed = Target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 12f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
			    {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<LingeringEyeBolt>(), Projectile.damage, 0, Main.myPlayer);
                }

                Projectile.ai[0] = 0;
            }
        }
    }
}