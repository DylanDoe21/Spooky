using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class PufferfishFlailShard : ModProjectile
    {
        int Bounces = 0;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
                SoundEngine.PlaySound(SoundID.NPCHit2 with { Pitch = 1.25f, Volume = 0.35f }, Projectile.Center);

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.75f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
                }
            }

			return false;
		}
		
		public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
		}

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit2 with { Pitch = 1.25f, Volume = 0.35f }, Projectile.Center);

			for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0f, -2f, 0, default, 1f);

				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
        }
    }
}