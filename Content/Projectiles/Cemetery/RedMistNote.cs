using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class RedMistNote : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
		}

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(Projectile.alpha - 255, Projectile.alpha - 255, Projectile.alpha - 255, 0).MultiplyRGBA(Color.Red);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 3 * 6 + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 2) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool? CanDamage()
        {
			return Projectile.ai[0] >= 120;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            int foundTarget = HomeOnTarget();

            Projectile.frame = (int)Projectile.ai[2];

            if (Projectile.timeLeft <= 125)
            {
                Projectile.alpha += 2;
            }
            
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 120 || (Projectile.ai[0] >= 120 && foundTarget == -1))
            {
                Projectile.ai[1] += Projectile.frame == 0 ? -5 : 5;
                int distance = 85;
                double rad = Projectile.ai[1] * (Math.PI / 180);
                Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
            }
            else
            {
                NPC target = Main.npc[foundTarget];
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 22;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 550;

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