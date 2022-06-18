using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class LeanProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {   
            DisplayName.SetDefault("Lean");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Throwing;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 60;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Magenta) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(tex, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
		
        public override void AI()
		{
            Projectile.rotation += 0.35f * (float)Projectile.direction;

            if (Main.rand.NextBool()) 
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 65);
				dust.noGravity = true;
				dust.scale = 1.6f;
			}   
        }

        public override void Kill(int timeleft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            for (int i = 0; i < 4; i++)
			{ 
				int NewProjectile = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20), Main.rand.NextFloat(-5.2f, 5.2f), 
                Main.rand.NextFloat(-5.2f, 5.2f), ProjectileID.DD2DrakinShot, Projectile.damage, 1, Main.myPlayer, 0, 0);

                Main.projectile[NewProjectile].friendly = true;
                Main.projectile[NewProjectile].hostile = false;
			}
        }
    }
}