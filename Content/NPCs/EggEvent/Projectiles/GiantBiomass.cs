using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;
using Spooky.Content.Events;
using Spooky.Content.NPCs.EggEvent;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class GiantBiomass : ModProjectile
    {
        int ScaleTimerLimit = 10;
        float RotateSpeed = 0.2f;
        float ScaleAmount = 0.05f;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath3", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 72;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.rotation += RotateSpeed * Projectile.direction; 

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 90 || Projectile.ai[0] == 110 || Projectile.ai[0] == 130)
            {
                ScaleAmount += 0.025f;
                ScaleTimerLimit -= 2;
            }

            if (Projectile.ai[0] >= 70)
            {
                Projectile.velocity *= 0.95f;

                if (RotateSpeed >= 0)
                {
                    RotateSpeed -= 0.01f;
                }
                else
                {
                    RotateSpeed = 0f;
                }

                Projectile.ai[1]++;
                if (Projectile.ai[1] < ScaleTimerLimit)
                {
                    Projectile.scale -= ScaleAmount;
                }
                if (Projectile.ai[1] >= ScaleTimerLimit)
                {
                    Projectile.scale += ScaleAmount;
                }

                if (Projectile.ai[1] > ScaleTimerLimit * 2)
                {
                    Projectile.ai[1] = 0;
                    Projectile.scale = 1.5f;
                }
            }

            if (Projectile.ai[0] >= 150)
            {
                //spawn enemy depending on what ai[2] is set to
                switch ((int)Projectile.ai[2])
                {
                    //Glutinous
                    case 0:
                    {
                        int Enemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Glutinous>());

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Enemy);
                        }

                        break;
                    }

                    //Vigilante
                    case 1:
                    {
                        int Enemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Vigilante>());

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Enemy);
                        }

                        break;
                    }

                    //Ventricle
                    case 2:
                    {
                        int Enemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Ventricle>());

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Enemy);
                        }

                        break;
                    }

                    //Crux
                    case 3:
                    {
                        int Enemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Crux>());

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Enemy);
                        }

                        break;
                    }

                    //Vesicator
                    case 4:
                    {
                        int Enemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<Vesicator>());

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Enemy);
                        }

                        break;
                    }
                }

                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
		{
            EggEventWorld.hasSpawnedBiomass = true;

            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            //spawn gores
            for (int numGores = 1; numGores <= 5; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, -2)), ModContent.Find<ModGore>("Spooky/BiomassGore" + numGores).Type);
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 8; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1f, 1.2f));
                Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            //spawn vanilla blood dust
            for (int numDust = 0; numDust < 75; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Blood, 0f, 0f, 100, default(Color), 1f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                Main.dust[newDust].scale *= Main.rand.NextFloat(1.8f, 2.5f);
                Main.dust[newDust].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
		}
    }
}