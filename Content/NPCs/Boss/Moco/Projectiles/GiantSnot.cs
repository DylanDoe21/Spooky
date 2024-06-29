using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class GiantSnot : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 38;                   			 
            Projectile.height = 54;          
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = true;
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
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.DarkGreen) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, Projectile.GetAlpha(Color.Lime * 0.5f), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);

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
        }

		public override void OnKill(int timeLeft)
		{
            if (Projectile.ai[0] > 0)
            {
                //spawn projectiles here

                //debug text for now
                Main.NewText("BOOOM!!!", Color.Green);
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
     
          






