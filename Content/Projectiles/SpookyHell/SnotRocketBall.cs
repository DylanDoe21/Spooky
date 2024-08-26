using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class SnotRocketBall : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Moco/Projectiles/SnotBall";

        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 26;                  			 
            Projectile.height = 26;          
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 400;
            Projectile.alpha = 255;
		}

        public override bool? CanDamage()
		{
            return Projectile.velocity.Y > 0;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 RealDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightGreen) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, Projectile.GetAlpha(Color.Lime), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
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

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }

            int minTilePosX = (int)(Projectile.position.X / 16) - 1;
            int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16) + 2;
            int minTilePosY = (int)(Projectile.position.Y / 16) - 1;
            int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    if (Main.tile[i, j] != null && (Main.tile[i, j].HasTile && (Main.tileSolid[(int)Main.tile[i, j].TileType])))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - (Projectile.height / 2), 0, 0, ModContent.ProjectileType<SnotRocketLinger>(), Projectile.damage, 0, Main.myPlayer);

                            Projectile.Kill();
                        }
                    }
                }
            }
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            for (int numDusts = 0; numDusts < 20; numDusts++)
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
     
          






