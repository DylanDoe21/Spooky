using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
    public class TomatoSauce : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 4;
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 240;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Color.Red * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.velocity = Vector2.Zero;

			return false;
		}

        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
		}

		public override void OnKill(int timeLeft)
		{
            for (int numDusts = 0; numDusts < 20; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
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
     
          






