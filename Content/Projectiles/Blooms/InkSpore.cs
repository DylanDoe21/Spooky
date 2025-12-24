using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Blooms
{
    public class InkSpore : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

			return false;
		}

        public override bool? CanDamage()
		{
			return Projectile.ai[1] >= 30;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.velocity = Vector2.Zero;
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[1]++;
            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation = Main.rand.Next(0, 360);
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f }, Projectile.Center);

            float maxAmount = 10;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                float intensity = Main.rand.NextFloat(1f, 3f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int newDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Asphalt, 0f, 0f, 100, default, 1f);
                Main.dust[newDust].noGravity = true;
                Main.dust[newDust].position = Projectile.Center + vector12;
                Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }
		}
    }
}