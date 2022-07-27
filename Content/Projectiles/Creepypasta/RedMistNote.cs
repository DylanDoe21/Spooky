using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Creepypasta
{ 
    public class RedMistNote1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Note");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 12;                   			 
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Magic;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 45;
            Projectile.scale = 0.75f;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Brown) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, effects, 0);
            }

            return true;
        }
		
		public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction;
            
            //scale the projectile up and down
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 10)
            {
                Projectile.scale -= 0.05f;
            }
            if (Projectile.ai[0] >= 10)
            {
                Projectile.scale += 0.05f;
            }

            if (Projectile.ai[0] > 20)
            {
                Projectile.ai[0] = 0;
                Projectile.scale = 0.75f;
            }
		}

		public override void Kill(int timeLeft)
		{
            //SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, 
            ModContent.ProjectileType<RedMistCloud>(), Projectile.damage / 2, 0f, Main.myPlayer, 0f, 0f);
		}
    }

    public class RedMistNote2 : RedMistNote1
    {
        public override void SetDefaults()
        {
			Projectile.width = 26;                   			 
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Magic;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 45;
		}
    }
}
     
          






