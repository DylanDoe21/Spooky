using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
    public class ZomboidTomatoHead : ModProjectile
    {
        public static readonly SoundStyle ExplodeSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            
            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
        }
        
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplodeSound, Projectile.Center);

            //spawn blood splatter
			for (int i = 0; i < 5; i++)
			{
				Vector2 Position = Projectile.Center + new Vector2(0, 15).RotatedByRandom(360);

                Vector2 ShootSpeed = Projectile.Center - Position;
                ShootSpeed.Normalize();
                ShootSpeed *= -Main.rand.NextFloat(5f, 9f);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Position, ShootSpeed, ModContent.ProjectileType<RedSplatter>(), 0, 0);
                }
			}

			//spawn blood explosion clouds
			for (int numExplosion = 0; numExplosion < 6; numExplosion++)
			{
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, 1f);
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].velocity *= 0.01f;
				Main.dust[DustGore].noGravity = true;

				if (Main.rand.NextBool(2))
				{
					Main.dust[DustGore].scale = 0.5f;
					Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
        }
    }
}