using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomBomb : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> EyeTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/Projectiles/PhantomBombGlow");

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Indigo);

            if (Flags.RaveyardHappening)
            {
                color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(new Color(18, 148, 0));
            }

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(EyeTexture.Value, drawPos, rectangle, Color.White * 0.5f, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.35f, 0.7f);

            Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            Projectile.velocity.X = Projectile.velocity.X * 0.99f;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - 25, 0, 0, 
                ModContent.ProjectileType<PhantomExplosion>(), Projectile.damage, 0, Main.myPlayer, 0, 0);
            }
        }
    }
}