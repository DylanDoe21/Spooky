using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class TarCannonBlob : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Vector2 trailDrawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Gray * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(ProjTexture.Value, trailDrawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale - oldPos / (float)Projectile.oldPos.Length / 3, SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slow, 300);
        }

        public override void AI()       
        {
            Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.12f;
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item126, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, 0f, -2f, 0, default, 0.85f);
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
			}
		}
    }
}