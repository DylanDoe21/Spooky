using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class OrroboroChargeTelegraph : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Telegraph");
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 3600;
            Projectile.alpha = 255;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            
            if (Projectile.ai[0] < 30)
            {
                if (Projectile.alpha <= 255) 
                { 
                    Projectile.alpha -= 25;

                    if (Projectile.alpha <= 155)
                    {
                        Projectile.alpha = 155;
                    }
                }   
            }         
            
            if (Projectile.ai[0] >= 30)
            {
                Projectile.alpha += 25;

                if (Projectile.alpha >= 255) 
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {      
            Main.EntitySpriteDraw((Texture2D)TextureAssets.MagicPixel, Projectile.position - new Vector2(3, 4000) - Main.screenPosition, null, new Color(193, 97, 18) * (1 - ((float)Projectile.alpha /255f)), Projectile.rotation, Vector2.Zero, new Vector2(6, 4), SpriteEffects.None, 0);
            
            return false;
        }
    }
}