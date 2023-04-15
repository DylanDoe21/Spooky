using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SkullTotem : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul Totem");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 5; numEffect++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 5 * 6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(-1, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SkullTotem>()] > 1)
            {
                Projectile.Kill();
            }

            if (Projectile.localAI[0] == 0)
            {
                Projectile.ai[0] = Projectile.position.Y;
                Projectile.localAI[0]++;
            }

            Projectile.ai[1]++;
            Projectile.position.Y = Projectile.ai[0] + (float)Math.Sin(Projectile.ai[1] / 30) * 30;

            //create dust ring
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * 500);
                offset.Y += (float)(Math.Cos(angle) * 500);
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center + offset - new Vector2(4, 4), 0, 0, DustID.GreenTorch, 0, 0, 100, Color.White, 1f)];
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.scale = 2.5f;
            }

            if (player.active && !player.dead)
            {
                float distance = player.Distance(Projectile.Center);
                
                if (distance <= 500)
                {
                    player.AddBuff(ModContent.BuffType<SkullTotemBuff>(), 2);
                }
            }
        }
        
        public override void Kill(int timeLeft)
		{
            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[newDust].position != Projectile.Center)
                {
				    Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
                }
			}
        }
    }
}