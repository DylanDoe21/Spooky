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

namespace Spooky.Content.NPCs.Catacomb.Layer2.Projectiles
{
    public class CatacombCrusherDeath1 : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher1";

        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 78;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }
    
        public override void AI()
        {
            Projectile.rotation += 0.05f * (float)Projectile.direction;

            Projectile.velocity.Y += 0.45f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 10)
            {
                Projectile.tileCollide = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] >= 10)
            {
                Projectile.Kill();
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 5;

            //spawn crusher gores
            for (int numGores = 1; numGores <= 4; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/CatacombCrusher1Gore" + numGores).Type);
                }
            }

            //spawn temple brick dust
            for (int numDust = 0; numDust < 25; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Lihzahrd, 0f, 0f, 100, default, 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-6, 7);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-6, 7);

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }

    public class CatacombCrusherDeath2 : CatacombCrusherDeath1
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher2";

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 5;

            //spawn crusher gores
            for (int numGores = 1; numGores <= 4; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/CatacombCrusher2Gore" + numGores).Type);
                }
            }

            //spawn temple brick dust
            for (int numDust = 0; numDust < 25; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Lihzahrd, 0f, 0f, 100, default, 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-6, 7);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-6, 7);

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }

    public class CatacombCrusherDeath3 : CatacombCrusherDeath1
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher3";

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 5;

            //spawn crusher gores
            for (int numGores = 1; numGores <= 4; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/CatacombCrusher3Gore" + numGores).Type);
                }
            }

            //spawn temple brick dust
            for (int numDust = 0; numDust < 25; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.t_Lihzahrd, 0f, 0f, 100, default, 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-6, 7);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-6, 7);

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }

    public class CatacombCrusherDeath4 : CatacombCrusherDeath1
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher4";

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 5;

            //spawn crusher gores
            for (int numGores = 1; numGores <= 7; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/CatacombCrusher4Gore" + numGores).Type);
                }
            }

            //spawn temple brick dust
            for (int numDust = 0; numDust < 35; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Gold, 0f, 0f, 100, default, 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-6, 7);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-6, 7);

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}