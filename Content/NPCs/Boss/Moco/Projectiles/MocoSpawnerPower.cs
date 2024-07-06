using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class MocoSpawnerPower : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
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

				Color color = Color.Lerp(Color.Brown, Color.Lime, scale);

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
            Projectile.timeLeft = 5;

            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            if (!Parent.active || Parent.type != ModContent.NPCType<MocoIntro>())
            {
                Projectile.Kill();
            }

            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 15)
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
            }

            if (Projectile.ai[1] >= 60)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(Parent.Center) * 20;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                Projectile.tileCollide = false;

                if (Projectile.Hitbox.Intersects(Parent.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
    }
}