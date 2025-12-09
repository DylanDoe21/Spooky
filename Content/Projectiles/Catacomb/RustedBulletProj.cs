using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class RustedBulletProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Catacomb/RustedBullet";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

		private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 2;
			Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scaleForLerp = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scaleForLerp *= 1f;

				Color color = Color.Lerp(Color.Transparent, Color.Brown, scaleForLerp / 1.5f);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / 4;

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()       
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.5f }, Projectile.Center);

            Vector2 Speed = new Vector2(2f, -12f).RotatedByRandom(2 * Math.PI);

            for (int numProjectiles = 0; numProjectiles <= 2; numProjectiles++)
            {
                Vector2 speed = -(Projectile.velocity / 8) + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, ModContent.ProjectileType<RustedBulletShrapnel>(), Projectile.damage / 2, 0f, Main.myPlayer);
                }
            }
        }
    }
}