using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class OldHunterMagicBolt : ModProjectile
	{
		public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.aiStyle = -1;
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

				Color color = Color.Lerp(Color.Green, Color.Cyan, scale);

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

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.45f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
		{
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

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 30)
            {
				NPC target = Main.npc[(int)Projectile.ai[1]];
				Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 18;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
				Projectile.tileCollide = false;
            }
		}

        public override void OnKill(int timeLeft)
		{
        	for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
	}
}