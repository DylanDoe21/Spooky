using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.Events
{
    public class EggEventHandler : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        bool SpawnedEnemies = false;
        bool EventEnemiesExist = true;

        public static readonly SoundStyle EventEndSound = new("Spooky/Content/Sounds/EggEvent/EggEventEnd", SoundType.Sound) { Volume = 2f };

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20; 
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var center = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            float intensity = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;
            DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/EggShieldNoise").Value, center,
            new Rectangle(0, 0, 500, 420), Color.Indigo, 0, new Vector2(250f, 250f), Projectile.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + intensity * 0.5f));
            GameShaders.Misc["ForceField"].Apply(drawData);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public void SwitchToNextWave()
        {
            if (EggEventWorld.hasSpawnedBiomass && !EventEnemiesExist)
            {
                if (EggEventWorld.Wave < 11)
                {
                    CombatText.NewText(Projectile.getRect(), Color.Magenta, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventWaveComplete"), true);
                }
                else
                {
                    SoundEngine.PlaySound(EventEndSound, Projectile.Center);
                }

                Projectile.ai[1] = 300;
                EggEventWorld.Wave++;
                SpawnedEnemies = false;
                EggEventWorld.hasSpawnedBiomass = false;
            }
        }

        public void SpawnEnemies()
        {
            switch (EggEventWorld.Wave)
            {
                //wave enemies: Glutinous x3, Vigilante x1
                //post defeat: Vigilante x1, Ventricle x1
                case 0:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {
                            //Vigilante
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);

                            //Ventricle
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x4, Vigilante x2
                //post defeat: Ventricle x2
                case 1:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 4; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {
                            //Ventricle
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                            }
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x3, Vigilante x1, Ventricle x1
                //post defeat: Vigilante x1, Crux x1
                case 2:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);

                        //Ventricle
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {
                            //Vigilante
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);

                            //Crux
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x2, Vigilante x2, Ventricle x2
                //post defeat: Crux x1
                case 3:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Ventricle
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {
                            //Crux
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Vigilante x2, Crux x1
                //post defeat: Glutinous x2, crux x1
                case 4:
                {
                    if (!SpawnedEnemies)
                    {
                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Crux
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {
                            //Glutinous
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                            }
                            
                            //Crux
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x3, Vigilante x2, Crux x1
                //post defeat: Vesicator x1
                case 5:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Crux
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Vesicator
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Vigilante x2, Ventricle x2, Crux x1
                //post defeat: Vigilante x4
                case 6:
                {
                    if (!SpawnedEnemies)
                    {
                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Ventricle
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        //Crux
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Vigilante
                            for (int numEnemy = 1; numEnemy <= 4; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                            }
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x5, Vesicator x1, 6 enemies total
                //post defeat: Ventricle x2
                case 7:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 5; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vesicator
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Ventricle
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                            }
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x5, Crux x3
                //post defeat: Vigilante x2, Ventricle x1, Vesicator x1
                case 8:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 5; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Crux
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Vigilante
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                            }

                            //Ventricle
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);

                            //Vesicator
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x2, Ventricle x3, Crux x2
                //post defeat: Vigilante x1, Vesicator x1
                case 9:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Ventricle
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        //Crux
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Vigilante
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);

                            //Vesicator
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Vigilante x4, Ventricle x3, Vesicator x1
                //post defeat: Glutinous x5, Crux x1
                case 10:
                {
                    if (!SpawnedEnemies)
                    {
                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 4; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Ventricle
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        //Vesicator
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Glutinous
                            for (int numEnemy = 1; numEnemy <= 5; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                            }

                            //Crux
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }

                //wave enemies: Glutinous x3, Vigilante x2, Ventricle x2, Crux x2, Vesicator x1
                //post defeat: Glutinous x3, Vigilante x2, Ventricle x1, Crux x1
                case 11:
                {
                    if (!SpawnedEnemies)
                    {
                        //Glutinous
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                        }

                        //Vigilante
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                        }

                        //Ventricle
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);
                        }

                        //Crux
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        //Vesicator
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                        Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 4);

                        //after defeating the event once, spawn some extra enemies
                        if (Flags.downedEggEvent)
                        {   
                            //Glutinous
                            for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 0);
                            }

                            //Vigilante
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 1);
                            }

                            //Ventricle
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 2);

                            //Crux
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-8f, 8f),
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, 3);
                        }

                        SpawnedEnemies = true;
                    }

                    break;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            VignettePlayer vignettePlayer = player.GetModPlayer<VignettePlayer>();
            vignettePlayer.SetVignette(0f, 1700f, 1f, Color.Black, new Vector2(Projectile.Center.X, Projectile.Center.Y - 85));

            //push players so they cannot leave the area during the event
            for (int i = 0; i <= Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    float distance = Main.player[i].Distance(new Vector2(Projectile.Center.X, Projectile.Center.Y - 85));
                    if (distance > 1600 && Main.player[i].InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
                    {
                        Vector2 movement = new Vector2(Projectile.Center.X, Projectile.Center.Y - 85) - Main.player[i].Center;
                        float difference = movement.Length() - 600;
                        movement.Normalize();
                        movement *= difference < 25f ? difference : 25f;
                        Main.player[i].position += movement;
                    }
                }
            }

            Projectile.ai[0]++;

            //spawn dust particles that get sucked towards this projectile, which is technically the egg
            if (Projectile.ai[0] % 20 == 0)
            {
                int MaxDusts = Main.rand.Next(10, 15);
                float distance = 50f;

                for (int numDust = 0; numDust < MaxDusts; numDust++)
                {
                    Vector2 dustPos = (Vector2.One * new Vector2((float)Projectile.width / 3f, (float)Projectile.height / 3f) * distance).RotatedBy((double)((float)(numDust - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + Projectile.Center;
                    Vector2 velocity = dustPos - Projectile.Center;

                    if (Main.rand.NextBool(2))
                    {
                        int dust = Dust.NewDust(dustPos + velocity, 0, 0, DustID.DemonTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                        Main.dust[dust].scale = 5f;
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = false;
                        Main.dust[dust].velocity = Vector2.Normalize(velocity) * (-50f);
                        Main.dust[dust].fadeIn = 1.2f;
                    }
                }

                Projectile.ai[0] = 2;
            }

            //egg event enemy spawning and wave handling
            if (EggEventWorld.EggEventActive)
            {
                Projectile.timeLeft = 20;

                if (NPC.AnyNPCs(ModContent.NPCType<Crux>()) || NPC.AnyNPCs(ModContent.NPCType<Glutinous>()) || NPC.AnyNPCs(ModContent.NPCType<Ventricle>()) ||
                NPC.AnyNPCs(ModContent.NPCType<Vesicator>()) || NPC.AnyNPCs(ModContent.NPCType<Vigilante>()))
                {
                    EventEnemiesExist = true;
                }
                else
                {
                    EventEnemiesExist = false;
                }

                //cooldown before switching to the next wave
                if (Projectile.ai[1] > 0)
                {
                    Projectile.ai[1]--;
                }
                
                SwitchToNextWave();

                if (Projectile.ai[1] <= 0)
                {
                    SpawnEnemies();
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}