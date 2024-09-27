using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class SnotBlessingOrbiter : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/NoseCult/Projectiles/NoseCultistMageSnot";

        private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 28;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
			Projectile.penetrate = 1;
            Projectile.alpha = 255;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
            Color glowColor = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

		public override bool? CanHitNPC(NPC target)
		{
			return Projectile.ai[2] > 0;
		}

		public override void AI()
        {
			NPC Target = Main.npc[(int)Projectile.ai[0]];

			if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
            }

            if (!Target.active)
            {
				Projectile.Kill();
            }

            Projectile.rotation += 0.25f * (float)Target.direction;

			if (Target.HasBuff(ModContent.BuffType<NoseBlessingDebuff>()) && Projectile.ai[2] == 0)
			{
				Projectile.ai[1] += 1f;
				int distance = 200;
				double rad = Projectile.ai[1] * (Math.PI / 180);
				Projectile.position.X = Target.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
				Projectile.position.Y = Target.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
			}
			else
			{
				if (Projectile.ai[2] == 0)
				{
					double Velocity = Math.Atan2(Target.Center.Y - Projectile.Center.Y, Target.Center.X - Projectile.Center.X);
					Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 35;

					Projectile.ai[2] = 1;
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.KryptonMoss, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
                {
				    Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
                }
			}
		}
    }
}
     
          






