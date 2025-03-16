using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SharkronFireball : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.snowMoon)
            {
                ProjTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/FrostMoonTextures/SharkronFireball");
            }
            else
            {
                ProjTexture = ModContent.Request<Texture2D>(Texture);
            }

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
			Projectile.rotation += 0.5f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 1f;

            for (int numDust = 0; numDust < 2; numDust++)
			{
                Vector2 dustPosition = Projectile.Center;
                dustPosition -= Projectile.velocity * ((float)numDust * 0.25f);
                int dustType = Main.snowMoon ? DustID.IceTorch : DustID.Torch;
                int dust = Dust.NewDust(dustPosition, 1, 1, dustType, 0f, 0f, 0, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = dustPosition;
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            for (int numDust = 0; numDust < 35; numDust++)
			{                      
                int dustType = Main.snowMoon ? DustID.IceTorch : DustID.Torch;
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			}
		}
    }
}