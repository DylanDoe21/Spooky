using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class MortarMine : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
            
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Color.White * fade, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 2f, 250f);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - 25, 0, 0, 
                ModContent.ProjectileType<MortarMineExplosion>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
            }

            Projectile.Kill();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            //constantly call stepup collision so it doesnt get stuck on blocks
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.velocity.Y += 0.2f;
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f;
			
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}
        }
    }
}