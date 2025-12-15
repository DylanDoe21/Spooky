using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class RustMiteSpike : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 60;
            Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
            
            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Orange);

            Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, Projectile.GetAlpha(Color.White * 0.2f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[1];

            if (Projectile.timeLeft <= 60) 
            {
				Projectile.alpha += 5;
			}
			else 
            {
				Projectile.alpha -= 15;
			}

            if (Projectile.alpha >= 255)
            {
                Projectile.alpha = 255;
            }

            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 7)
            {
				Projectile.velocity = Vector2.Zero;
			}
        }
    }
}