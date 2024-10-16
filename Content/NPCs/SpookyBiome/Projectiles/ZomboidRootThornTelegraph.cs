using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
    public class ZomboidRootThornTelegraph : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

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

            if (Projectile.ai[0] < 45)
            {
                int dirtDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
                Main.dust[dirtDust].noGravity = true;
                Main.dust[dirtDust].velocity.Y *= Main.rand.Next(2, 3);
            }

            if (Projectile.ai[0] == 10)
            {
                Vector2 lineDirection = new Vector2(0, 16);
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + 13, Projectile.Center.Y - 16, 0, 0,
                    ModContent.ProjectileType<ZomboidRootThorn>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                }
            }

            if (Projectile.ai[0] > 45)
            {
                Projectile.Kill();
            }
        }
    }

    public class ZomboidRootThornTelegraphFire : ZomboidRootThornTelegraph
    {
        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] < 45)
            {
                int[] DustIDs = new int[] { DustID.Dirt, DustID.Torch };

                int dirtDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(DustIDs), 0f, -2f, 0, default, 1.5f);
                Main.dust[dirtDust].noGravity = true;
                Main.dust[dirtDust].velocity.Y *= Main.rand.Next(2, 3);
            }

            if (Projectile.ai[0] == 5)
            {
                Vector2 lineDirection = new Vector2(0, 16);
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + 13, Projectile.Center.Y - 16, 0, 0,
                    ModContent.ProjectileType<ZomboidRootThornFire>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                }
            }

            if (Projectile.ai[0] > 45)
            {
                Projectile.Kill();
            }
        }
    }
}