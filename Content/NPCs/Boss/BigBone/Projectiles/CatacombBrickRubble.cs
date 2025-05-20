using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class CatacombBrickRubble : ModProjectile
    {
        private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 480;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 35)
            {
                TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/CatacombBrickRubbleTrail");
                
                Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, Projectile.height * 0.5f);

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1.2f;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.SaddleBrown) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                    Rectangle rectangle = new(0, (TrailTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, TrailTexture.Width(), TrailTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[1];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 35)
            {
                //max downward velocity, randomized for each individual projectile
                Projectile.ai[2] = Main.rand.Next(9, 17);

                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.t_Lihzahrd, new Vector2(0, Main.rand.Next(2, 16)));
                dust.color = Color.White;

                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] >= 35)
            {
                if (Projectile.velocity.Y < Projectile.ai[2])
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
                }
            }

            if (Projectile.ai[0] >= 60)
            {
                Projectile.tileCollide = true;
            }
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Tink with { Pitch = -0.5f }, Projectile.Center);

			//spawn temple brick dust
            for (int numDust = 0; numDust < 6; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Lihzahrd, 0f, 0f, 100, default, 1f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-1, 2);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-1, 2);

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
		}
    }
}
     
          






