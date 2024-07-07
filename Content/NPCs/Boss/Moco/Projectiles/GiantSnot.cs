using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class GiantSnot : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 38;                   			 
            Projectile.height = 54;          
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 260;
            Projectile.alpha = 255;
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OgreSpit, 180, true);
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 RealDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = 1f * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.DarkGreen) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, Projectile.GetAlpha(Color.Lime * 0.5f), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.NPCHit8, Projectile.Center);

            //set where the it should be jumping towards
            Vector2 JumpTo = new(player.Center.X, player.Center.Y - Main.rand.Next(300, 450));

            //set velocity and speed
            Vector2 velocity = JumpTo - Projectile.Center;
            velocity.Normalize();

            float speed = MathHelper.Clamp(velocity.Length() / 5, 10, 45);

            velocity.X *= Projectile.ai[0] > 0 ? 1.5f : 1.2f;
            velocity.Y -= Main.rand.NextFloat(0.5f, 1f);

			Projectile.velocity = velocity * speed;

			return false;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 11)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }   

            if (!IsColliding())
            {
                Projectile.ai[2]++;
            }
            if (Projectile.ai[2] > 20)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] > 0 && Projectile.timeLeft <= 85)
            {
                //make the mocling scale up and down rapidly
                Projectile.ai[1]++;
                if (Projectile.ai[1] < 2)
                {
                    Projectile.scale -= 0.5f;
                }
                if (Projectile.ai[1] >= 2)
                {
                    Projectile.scale += 0.5f;
                }
                
                if (Projectile.ai[1] > 4)
                {
                    Projectile.ai[1] = 0;
                    Projectile.scale = 1f;
                }
            }
        }

        public bool IsColliding()
        {
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
                            return true;
                        }
                    }
                }
            }

            return false;
        }

		public override void OnKill(int timeLeft)
		{
            if (Projectile.ai[0] > 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

                SpookyPlayer.ScreenShakeAmount = 8;

                for (int numProjectiles = 0; numProjectiles < 10; numProjectiles++)
			    {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-7, -3)), ModContent.ProjectileType<LingeringSnotBall>(), Projectile.damage, 0, Main.myPlayer);
                }
            }

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
     
          






