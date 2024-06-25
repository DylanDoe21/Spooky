using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class LingeringSnotBall : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/TomatoSplat", SoundType.Sound);

        public override void SetStaticDefaults()
        {
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
                if (Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Color glowColor = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Color newColor = glowColor;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OgreSpit, 60, true);
        }

        public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 20)
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
                                Projectile.Kill();
                            }
                        }
                    }
                }
            }
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<LingeringSnot>(), Projectile.damage, 0, Main.myPlayer);

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
     
          






