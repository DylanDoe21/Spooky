using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Pumpkin.Projectiles
{
	public class RootTelegraph : ModProjectile
	{
		public override void SetStaticDefaults() 
        {
            DisplayName.SetDefault("Root");
        }

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
		
        public override void AI()
		{
			Projectile.ai[0]++;
			if (Projectile.ai[0] < 40) 
            {
                for (int i = 0; i < 5; i++)
                {                                                                                  
                    int num = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height + 20, DustID.Dirt, 0f, 0f, 0, default(Color), 1.5f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].velocity.Y -= Main.rand.Next(5, 10);
                    Main.dust[num].scale = 1.5f;
                }
            }

            if (Projectile.ai[0] >= 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y + 10, 0, -4, 
                    ModContent.ProjectileType<RootPillar>(), Projectile.damage, 1, Main.myPlayer, 0, 0);
                }

                Projectile.Kill();
            }
		}
	}
}