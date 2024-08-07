using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class AmuletSeed : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/SpookySpirit/Projectiles/PhantomSeed";

		private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 14;                   			 
            Projectile.height = 20;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Purple);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] > 80;
        }

        public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                for (int numDust = 0; numDust < 10; numDust++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].scale = 1.5f;
                    Main.dust[dust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;

                    if (Main.dust[dust].position != Projectile.Center)
                    {
                        Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                    }
                }
            }

            if (Projectile.ai[0] < 80)
			{
				Projectile.velocity *= 0.98f;
			}
			
			if (Projectile.ai[0] > 80 && Projectile.ai[0] < 120)
			{
				int foundTarget = HomeOnTarget();
				if (foundTarget != -1)
				{
					NPC target = Main.npc[foundTarget];
					Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 50;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
					Projectile.tileCollide = false;
				}
				else
				{
					Projectile.tileCollide = true;
				}			
			}
		}

		private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 350;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
            for (int numDust = 0; numDust < 15; numDust++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}
     
          






