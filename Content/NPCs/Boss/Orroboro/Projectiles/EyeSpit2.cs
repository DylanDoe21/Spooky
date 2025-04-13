using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class EyeSpit2 : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/Projectiles/EyeSpit";

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;          
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 300;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gold) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 11)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 35)
            {   
                Projectile.velocity.X = Projectile.velocity.X * 0.98f;

                if (Projectile.velocity.Y < 12)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
                }
            }
		}

		public override void OnKill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 12; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DesertWater2, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 0.85f;
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                Main.dust[dust].velocity = -Projectile.velocity;
			}
		}
    }
}
     
          






