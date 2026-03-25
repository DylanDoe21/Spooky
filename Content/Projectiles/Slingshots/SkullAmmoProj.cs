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
    public class SkullAmmoProj : ModProjectile
    {
		public override string Texture => "Spooky/Content/Items/Slingshots/Ammo/SkullAmmo";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
		{
			ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
		}
		
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
			Projectile.penetrate = -1;
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

				Color color = Color.Gold;

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

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.velocity.X = -Projectile.velocity.X;
			Projectile.velocity.Y = -Projectile.velocity.Y;
		}

        public override void AI()       
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * (float)Projectile.direction;

            Projectile.ai[0]++;
			if (Projectile.ai[0] >= 60)
			{
				Projectile.velocity *= 0.95f;
			}

			if (Projectile.ai[0] >= 180 && Projectile.timeLeft > 70)
			{
				if (Projectile.ai[0] % 60 == 0)
				{
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -5).RotatedByRandom(360), 
					ModContent.ProjectileType<SkullAmmoThornVine>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: Projectile.whoAmI);
				}
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
    }
}