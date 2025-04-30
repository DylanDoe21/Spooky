using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class FossilBlackPepperPebble : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			for (int i = 0; i < 360; i += 60)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

			return true;
        }

        public override bool? CanDamage()
		{
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[0] > 35 && Projectile.ai[1] == 0)
            {
                Projectile.velocity = Vector2.Zero;

                Projectile.ai[1] = 1;
            }

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.frame = (int)Projectile.ai[2];

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 35)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;

                if (Projectile.ai[1] > 0)
                {
                    if (Projectile.Distance(player.Center) <= 150f)
                    {
                        Vector2 desiredVelocity = Projectile.DirectionTo(player.MountedCenter) * 20;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    }

                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);

                        if (player.GetModPlayer<BloomBuffsPlayer>().FossilBlackPepperStacks < 10)
                        {
                            player.GetModPlayer<BloomBuffsPlayer>().FossilBlackPepperStacks++;
                        }

                        Projectile.Kill();
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