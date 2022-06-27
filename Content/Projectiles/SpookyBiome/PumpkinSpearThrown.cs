using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{ 
    public class PumpkinSpearThrown : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Poker");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 58;                   			 
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Melee;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 500;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Brown) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
		
		public override void AI()
        {
			Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.80f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 12)
            {
                Projectile.tileCollide = false;
            }

            if (Projectile.ai[0] > 12)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] >= 45)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
            }
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) 
        {
			Projectile.damage = (int)(damage * 2f);
		}

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

			for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
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
     
          






