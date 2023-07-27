using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.SpookyHell.Projectiles
{
    public class NautilusBiomass : ModProjectile
    {
        int ScaleTimerLimit = 10;
        float RotateSpeed = 0.2f;
        float ScaleAmount = 0.05f;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath3", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 74;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += RotateSpeed * Projectile.direction; 

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 65 || Projectile.ai[0] == 85 || Projectile.ai[0] == 105)
            {
                ScaleAmount += 0.025f;
                ScaleTimerLimit -= 2;
            }

            if (Projectile.ai[0] >= 45)
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

            if (Projectile.ai[0] >= 125)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            //spawn squids
            for (int numEnemies = 1; numEnemies <= 2; numEnemies++)
            {
                int SpawnedNPC = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<ValleySquidClone>());

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);
                }
            }

            //spawn gores
            for (int repeats = 1; repeats <= 2; repeats++)
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, -2)), ModContent.Find<ModGore>("Spooky/BloodyBiomassGore" + numGores).Type);
                    }
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1.8f, 2.5f));
                Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
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
                int newDust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Blood, 0f, 0f, 100, default, 1f);
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