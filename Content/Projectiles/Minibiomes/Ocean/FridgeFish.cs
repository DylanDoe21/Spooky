using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class FridgeFish : ModProjectile
    {
		int Bounces = 0;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			if (Projectile.ai[1] == 1)
			{
				target.AddBuff(ModContent.BuffType<CavefishStunned>(), 300);
			}
            if (Projectile.ai[1] == 2)
            {
                target.AddBuff(BuffID.Stinky, 600);
                target.AddBuff(BuffID.Poisoned, 600);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
			//golden fish do 25% more damage
			if (Projectile.ai[1] == 3)
			{
				modifiers.SourceDamage *= 1.25f;
			}
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[2] == 0)
			{
				Projectile.ai[2]++;
			}
			else
			{
				SoundEngine.PlaySound(SoundID.Item177 with { Pitch = 1f, Volume = 0.5f }, Projectile.Center);

				Bounces++;
				if (Bounces >= 3)
				{
					Projectile.Kill();
				}
			}

			return false;
		}

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

			if (Projectile.ai[2] == 0)
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				Projectile.rotation += 0f * (float)Projectile.direction;

				Projectile.frame = (int)Projectile.ai[1];

				if (Projectile.ai[1] == 2)
				{
					if (Main.rand.NextBool(5))
					{
						int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 188, 0f, -2f, 0, default, 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity = Projectile.velocity;
						Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
						Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
					}
				}

				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 15)
				{
					Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
				}
			}
			else
			{
				if (Projectile.velocity.Y == 0f)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Projectile.velocity.Y = (float)Main.rand.Next(-50, -20) * 0.1f;
						Projectile.velocity.X = (float)Main.rand.Next(-20, 20) * 0.1f;
						Projectile.netUpdate = true;
					}
				}
				Projectile.velocity.Y += 0.2f;
				if (Projectile.velocity.Y > 10f)
				{
					Projectile.velocity.Y = 10f;
				}

				Projectile.rotation = Projectile.velocity.Y * (Projectile.direction == 1 ? 0.07f : -0.07f) + (Projectile.direction == 1 ? MathHelper.PiOver2 : -MathHelper.PiOver2);
			}
        }

		public override void OnKill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 15; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-15, 15) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-15, 15) * 0.05f - 1.5f;
				Main.dust[dust].velocity = Projectile.velocity * 0.5f;
			}
		}
    }
}