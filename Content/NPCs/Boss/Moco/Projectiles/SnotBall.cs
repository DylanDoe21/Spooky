using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class SnotBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snot Ball");
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 26;                  			 
            Projectile.height = 26;          
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 400;
            Projectile.alpha = 255;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 11)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OgreSpit, 60, true);
        }

        public override void AI()
        {
			//fix Projectile direction
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1]++;

                if (Projectile.ai[1] > 75)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.99f;
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.18f;

                    int minTilePosX = (int)(Projectile.position.X / 16.0) - 1;
                    int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16.0) + 2;
                    int minTilePosY = (int)(Projectile.position.Y / 16.0) - 1;
                    int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16.0) + 2;
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
                                    Projectile.Kill();
                                }
                            }
                        }
                    }
                }
            }
		}

		public override void Kill(int timeLeft)
		{
            if (Projectile.ai[0] == 1)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, 
                ModContent.ProjectileType<LingeringSnot>(), Projectile.damage, 0, Main.myPlayer, 0f, 0f);
            }

            for (int numDust = 0; numDust < 20; numDust++)
            {                                                                                  
                int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.KryptonMoss, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[DustGore].noGravity = true;
                Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
                if (Main.dust[DustGore].position != Projectile.Center)
                {
                    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
            }
		}
    }
}
     
          






