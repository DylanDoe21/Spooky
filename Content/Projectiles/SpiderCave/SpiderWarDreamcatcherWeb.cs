using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SpiderWarDreamcatcherWeb : ModProjectile
    {
        float Scale = 0f;
        float fade = 1f;

        bool Switch = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 68;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            if (!Switch)
            {
                fade -= 0.0012f;
                if (fade <= 0.85f)
                {
                    Switch = true;
                }
            }
            if (Switch)
            {
                fade += 0.0012f;
                if (fade >= 1f)
                {
                    Switch = false;
                }
            }

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White * 0.75f), Projectile.rotation, drawOrigin, (Projectile.scale * 1.1f * fade) * Scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Main.rand.Next(0, 361);
                Projectile.ai[0]++;
            }

            if (Scale < 1f)
            {
                Scale += 0.02f;
                Projectile.rotation += 0.1f;

                if (Main.rand.NextBool(30))
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, Main.rand.Next(-4, -1)).RotatedBy(Projectile.rotation), ModContent.ProjectileType<OgreKingWebEffect>(), 0, 0f, Main.myPlayer);
                    }
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}