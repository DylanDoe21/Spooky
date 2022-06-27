using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class LeafTornado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Leaf Tornado");
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 250;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
           	Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
			
            return true;
        }
		
        public override void AI()
        {    
            if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}
            if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 9;
			}

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= 60)
            {   
                Projectile.velocity *= 1.02f;
            }
            else
            {
                Projectile.velocity *= 0.98f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                int[] Leaves = new int[] { ModContent.GoreType<Content.Gores.LeafGreenTreeFX>(),
                ModContent.GoreType<Content.Gores.LeafOrangeTreeFX>(), ModContent.GoreType<Content.Gores.LeafOrangeTreeFX>(), 
                ModContent.GoreType<Content.Gores.LeafRedTreeFX>(), ModContent.GoreType<Content.Gores.LeafRedTreeFX>() };

                int LeafGore = Gore.NewGore(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), Vector2.Zero, Leaves[i], 1f);
                Main.gore[LeafGore].rotation = 0f;
                Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
                Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.5f, 1.5f);
            }
        }
	}
}