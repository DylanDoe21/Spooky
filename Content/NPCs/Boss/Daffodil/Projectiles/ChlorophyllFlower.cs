using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{ 
    public class ChlorophyllFlower : ModProjectile
    {
        int target;

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];

		private static Asset<Texture2D> TrailTexture;

		public override void SetDefaults()
		{
			Projectile.width = 20;                   			 
            Projectile.height = 24;     
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 180;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/FlowerTrail");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.Green, Color.Lime, scale);

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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0f);

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

            if (Projectile.ai[0] < 50)
            {
                if (Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                if (Projectile.velocity.X < 0)
                {
                    Projectile.rotation += -0.35f;
                }

                Projectile.velocity *= 0.99f;
            }

            if (Projectile.ai[0] >= 50 && Projectile.ai[0] < 90)
            {
                if (Projectile.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    target = -1;
                    float distance = 2000f;
                    for (int k = 0; k < 255; k++)
                    {
                        if (Main.player[k].active && !Main.player[k].dead)
                        {
                            Vector2 center = Main.player[k].Center;
                            float currentDistance = Vector2.Distance(center, Projectile.Center);
                            if (currentDistance < distance || target == -1)
                            {
                                distance = currentDistance;
                                target = k;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.ai[1] = 1;
                        Projectile.netUpdate = true;
                    }
                }
                else if (target >= 0 && target < Main.maxPlayers)
                {
                    Player targetPlayer = Main.player[target];
                    if (!targetPlayer.active || targetPlayer.dead)
                    {
                        target = -1;
                        Projectile.ai[1] = 0;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        float currentRot = Projectile.velocity.ToRotation();
                        Vector2 direction = targetPlayer.Center - Projectile.Center;
                        float targetAngle = direction.ToRotation();
                        if (direction == Vector2.Zero)
                        {
                            targetAngle = currentRot;
                        }

                        float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
                        Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot);
                    }
                }

                Projectile.velocity *= Main.rand.NextFloat(1.003f, 1.0085f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int numDust = 0; numDust < 10; numDust++)
            {                                                                                  
                int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default, 1f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-1f, 1f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-1f, 1f);
                Main.dust[dustGore].noGravity = true;
            }
        }
    }
}