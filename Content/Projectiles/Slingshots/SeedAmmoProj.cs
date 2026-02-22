using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class SeedAmmoProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Slingshots/Ammo/SeedAmmo";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
		{
			ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
		}

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 70;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
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
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.YellowGreen;

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 45)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.98f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            }

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
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);

			float maxAmount = 20;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 5f), Main.rand.NextFloat(1f, 5f));
                float intensity = Main.rand.NextFloat(1f, 5f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int num104 = Dust.NewDust(Projectile.Center, 0, 0, 46, 0f, 0f, 100, default, 2f);
                Main.dust[num104].noGravity = true;
                Main.dust[num104].position = Projectile.Center + vector12;
                Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }

			maxAmount = 3;
            currentAmount = 0;

            float RandomRotation = MathHelper.ToRadians(Main.rand.NextFloat(0f, 360f));

            while (currentAmount < maxAmount)
            {
                Vector2 velocity = new Vector2(2f, 2f);
                Vector2 Bounds = new Vector2(1f, 1f);
                float intensity = 3f;

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                Vector2 ShootVelocity = (velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity).RotatedBy(RandomRotation);

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ShootVelocity, ModContent.ProjectileType<SeedAmmoSplit>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                currentAmount++;
            }
        }
    }
}