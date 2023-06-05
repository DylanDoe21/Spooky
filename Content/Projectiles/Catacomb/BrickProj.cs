using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.NPCs.Boss.Daffodil;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BrickProj : ModProjectile
    {
        public static readonly SoundStyle BonkSound = new("Spooky/Content/Sounds/BrickBonk", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }

            for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<DaffodilBody>()) 
				{
                    if (Main.npc[k].Distance(Projectile.Center) <= 100f)
                    {
                        SoundEngine.PlaySound(BonkSound, Projectile.Center);
                        Main.npc[k].ai[0] = 1;
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void Kill(int timeLeft)
		{
            /*
            for (int numDust = 0; numDust < 10; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Brick, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    
				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
            */
        }
    }
}