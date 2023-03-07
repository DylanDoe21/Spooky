using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class ThornTelegraph : ModProjectile
    {
        public static readonly SoundStyle BoneSnap = new SoundStyle("Spooky/Content/Sounds/Orroboro/BoneSnap", SoundType.Sound);

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
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3600;  
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 20)
            {
                if (Projectile.alpha <= 255) 
                { 
                    Projectile.alpha -= 12;

                    if (Projectile.alpha <= 155)
                    {
                        Projectile.alpha = 155;
                    }
                }            
            }

            else if (Projectile.ai[0] == 60)
            {
                Vector2 lineDirection = new Vector2(0, 16);

                SoundEngine.PlaySound(BoneSnap, Projectile.position);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 20, Vector2.Zero.X, Vector2.Zero.Y,
                    ModContent.ProjectileType<ThornPillar>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                }
            }
            else if (Projectile.ai[0] >= 60)
            {
                Projectile.alpha += 10;

                if (Projectile.alpha >= 255) 
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {      
            Main.EntitySpriteDraw((Texture2D)TextureAssets.MagicPixel, Projectile.position - new Vector2(3, 4000) - Main.screenPosition, null, new Color(140, 99, 201) * (1 - ((float)Projectile.alpha /255f)), Projectile.rotation, Vector2.Zero, new Vector2(6, 4), SpriteEffects.None, 0);
            return false;
        }
    }
}