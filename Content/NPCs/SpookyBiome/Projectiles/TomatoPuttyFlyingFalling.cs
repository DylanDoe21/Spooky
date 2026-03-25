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
    public class TomatoPuttyFlyingFalling : ModProjectile
    {
        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 48;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.02f * (float)Projectile.direction;
            
            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
        }
        
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SplatSound, Projectile.Center);

            //spawn enemies
            for (int numNPCs = 0; numNPCs < 5; numNPCs++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int NewNPC = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<TomatoPuttyFlyingSmall>());
                    Main.npc[NewNPC].velocity = Main.rand.NextVector2Circular(3, 7);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                    }
                }
            }

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
			for (int numExplosion = 0; numExplosion < 8; numExplosion++)
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