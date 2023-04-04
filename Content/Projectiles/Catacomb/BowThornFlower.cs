using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class BowThornFlower : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thorn Flower");
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true; 
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			target.AddBuff(ModContent.BuffType<ThornMark>(), 180);
        }

		public override void AI()
        {   
            Lighting.AddLight(Projectile.Center, 0.85f, 0f, 0f);

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
			{
				Projectile.velocity *= 0.5f;
			}
			if (Projectile.ai[0] >= 60)
			{
				Projectile.velocity *= 0;

				if (Projectile.ai[1] == 0)
				{
					for (float numProjectiles = 0; numProjectiles < 6; numProjectiles++)
					{
						Vector2 projPos = Projectile.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / 6));

						Vector2 Direction = Projectile.Center - projPos;
						Direction.Normalize();

						Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0, 0,
						ModContent.ProjectileType<BowFlowerThorn>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
					}

					Projectile.ai[1] = 1;
				}
			}
        }

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
        
        	for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
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