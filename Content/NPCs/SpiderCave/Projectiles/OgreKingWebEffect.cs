using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class OgreKingWebEffect : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

		Vector2 SavePosition;

		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 18;
            Projectile.height = 18;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
			Projectile.hide = true;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
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

				Color color = Color.LightGray * scale;
                color *= (Projectile.timeLeft) / 90f;

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

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.9f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
                int RandX = Main.rand.NextBool() ? -10 : 10;
                int RandY = Main.rand.NextBool() ? -10 : 10;
                SavePosition = new Vector2(Projectile.Center.X + RandX, Projectile.Center.Y + RandY);

                Projectile.ai[1] = Main.rand.NextFloat(4f, 15f);

                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] > 1)
            {
                double angle = Projectile.DirectionTo(SavePosition).ToRotation() - Projectile.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                float Angle = Math.Sign(angle);
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 5;

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(Projectile.ai[1]) * Angle);
            }

            if (Projectile.ai[0] > 75)
            {
                Projectile.scale *= 0.95f;
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
     
          






