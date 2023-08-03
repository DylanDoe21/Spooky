using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Sentient
{
    public class BloodTooth : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 25;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            //make a trail of blood dust
            Vector2 dustPosition = Projectile.Center;
            dustPosition -= Projectile.velocity * 0.25f;
            int dust = Dust.NewDust(dustPosition, 1, 1, DustID.Blood, 0f, 0f, 0, default, 1f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].position = dustPosition;
            Main.dust[dust].velocity *= 0.2f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 2)
            {
                Projectile.alpha = 255;
            }
            else
            {
                Projectile.alpha = 0;
            }

            if (Projectile.ai[0] < 20)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y = -22;
            }

            if (Projectile.ai[0] > 20 && Projectile.ai[0] < 25)
            {
                Projectile.velocity *= 0.98f;
            }

            if (Projectile.ai[0] == 25)
            {
                Vector2 ChargeDirection = (Main.MouseWorld + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50))) - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection *= 50;
                Projectile.velocity = ChargeDirection;
            }

            if (Projectile.ai[0] >= 25 && Projectile.position.Y >= player.Center.Y - 35)
            {
                Projectile.tileCollide = true;
            }
        }

        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 12; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale *= 0.5f;
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
			}
		}
    }
}