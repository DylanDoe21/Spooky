using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class LingerThorn : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;                   			 
            Projectile.height = 16;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 600;
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Poisoned, 120, true);
            target.AddBuff(BuffID.Bleeding, 120, true);

            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Red);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 3 * 6 + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(-1, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;	
            
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

            for (int i = minTilePosX; i < maxTilePosX; i++)
            {
                for (int j = minTilePosY; j < maxTilePosY; j++)
                {
                    if (Main.tile[i, j] != null && !Main.tile[i, j].Get<TileWallWireStateData>().IsActuated && Main.tile[i, j].HasTile && 
                    Main.tileSolid[Main.tile[i, j].TileType] && !Main.tileSolid[Main.tile[i, j - 1].TileType] && 
                    Main.tile[i, j].TileType != ModContent.TileType<OldWoodPlatform>())
                    {
                        Vector2 vector2;
                        vector2.X = (i * 16);
                        vector2.Y = (j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            Projectile.velocity *= 0;
                        }
                    }
                }
            }
		}

		public override void Kill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 8; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
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
     
          






