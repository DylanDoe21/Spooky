using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class MassiveFlameBallBolt : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];

		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(258);
			Projectile.width = 14;
            Projectile.height = 14;
			Projectile.hostile = true;
            Projectile.friendly = false;                              			  		
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.Red, Color.White, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
		{
			target.AddBuff(BuffID.OnFire, 300);
		}

		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0f);

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