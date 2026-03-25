using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.OldHunter.Projectiles
{
    public class OldHunterHead : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 2;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
            
            return false;
        }

        public override void AI()
        {
			Projectile.timeLeft = 2;

            Projectile.rotation += Projectile.velocity.X * 0.1f;

			if (Projectile.ai[1] <= 0)
			{
				Projectile.ai[0] += 1f;
				if (Projectile.ai[0] > 5f)
				{
					Projectile.ai[0] = 5f;
					if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
					{
						Projectile.velocity.X *= 0.97f;
						if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
						{
							Projectile.velocity.X = 0f;
							Projectile.netUpdate = true;
						}
					}

					Projectile.velocity.Y += 0.2f;
				}
				
				if (Projectile.velocity.Y > 16f)
				{
					Projectile.velocity.Y = 16f;
				}
			}
			else
			{	
				Projectile.localAI[0]++;
				if (Projectile.localAI[0] >= 120)
				{
					NPC Parent = Main.npc[(int)Projectile.ai[2]];
					Parent.localAI[1] = 2;
					Projectile.Kill();
				}
			}
        }
    }
}