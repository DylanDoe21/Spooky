using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Sentient
{
    public class BillyFeather : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 22;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool? CanDamage()
		{
			return Projectile.ai[0] > 50;
        }

		public override void AI()
        {
            Projectile.ai[0]++;
            
            //make projectile spin
            if (Projectile.ai[0] < 50)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                }

                Projectile.spriteDirection = Projectile.velocity.X  > 0 ? 1 : -1;
                
                if (Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                if (Projectile.velocity.X < 0)
                {
                    Projectile.rotation += -0.35f;
                }

                Projectile.velocity *= 0.97f;
            }

            if (Projectile.ai[0] > 50)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;

                if (Projectile.ai[1] < 25)
                {
                    Projectile.ai[1] += 0.5f;
                }

                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * Projectile.ai[1];
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 500;

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
    }
}