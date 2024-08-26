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
    public class BillyPhantomPortal : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 60;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.White);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, -Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
        {
            Projectile.ai[0]++;

            Projectile.rotation += 0.1f;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;

                if (Projectile.timeLeft == 60)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BillyPhantom>(), Projectile.damage, Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                }
            }
        }
    }
}