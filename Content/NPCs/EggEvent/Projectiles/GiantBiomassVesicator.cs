using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class GiantBiomassVesicator : ModProjectile
    {
        private static Asset<Texture2D> AuraTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath3", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 70;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1 && Projectile.localAI[1] >= 60)
            {
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/GiantBiomassVesicatorAura");

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.Red);

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(AuraTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.localAI[2] / 37 + (Projectile.localAI[2] < 320 ? time : time2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return Projectile.ai[0] == 0;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f * Projectile.direction;

            //different behaviors of this projectile
            switch ((int)Projectile.ai[0])
            {
                //fall to the ground and spawn a random enemy
                case 0:
                {
                    if (Projectile.localAI[0] < 450)
                    {
                        Projectile.localAI[0] += 10;
                    }

                    if (Projectile.localAI[0] >= 225)
                    {
                        Projectile.velocity.Y += 0.25f;
                    }

                    if (Projectile.localAI[0] >= 450)
                    {
                        Projectile.tileCollide = true;
                    }
                    else
                    {
                        Projectile.tileCollide = false;
                    }

                    break;
                }

                //chase the player, create massive damage aura, then explode
                case 1:
                {
                    Projectile.localAI[0]++;

                    if (Projectile.localAI[0] < 200)
                    {
                        Player player = Main.LocalPlayer;
                        float goToX = player.Center.X - Projectile.Center.X;
                        float goToY = player.Center.Y - Projectile.Center.Y;
                        float speed = 0.45f;

                        if (Projectile.velocity.X < goToX)
                        {
                            Projectile.velocity.X = Projectile.velocity.X + speed;
                            if (Projectile.velocity.X < 0f && goToX > 0f)
                            {
                                Projectile.velocity.X = Projectile.velocity.X + speed;
                            }
                        }
                        else if (Projectile.velocity.X > goToX)
                        {
                            Projectile.velocity.X = Projectile.velocity.X - speed;
                            if (Projectile.velocity.X > 0f && goToX < 0f)
                            {
                                Projectile.velocity.X = Projectile.velocity.X - speed;
                            }
                        }
                        if (Projectile.velocity.Y < goToY)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y + speed;
                            if (Projectile.velocity.Y < 0f && goToY > 0f)
                            {
                                Projectile.velocity.Y = Projectile.velocity.Y + speed;
                                return;
                            }
                        }
                        else if (Projectile.velocity.Y > goToY)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y - speed;
                            if (Projectile.velocity.Y > 0f && goToY < 0f)
                            {
                                Projectile.velocity.Y = Projectile.velocity.Y - speed;
                                return;
                            }
                        }
                    }

                    if (Projectile.localAI[0] >= 120)
                    {
                        Projectile.velocity *= 0.98f;

                        Projectile.localAI[1]++;

                        if (Projectile.localAI[1] >= 60)
                        {
                            if (Projectile.localAI[2] < 320)
                            {
                                Projectile.localAI[2] += 10;
                            }
                        }

                        if (Projectile.localAI[1] >= 160)
                        {
                            Projectile.Kill();
                        }
                    }

                    break;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            //spawn a random enemy if it is meant to
            if (Projectile.ai[0] == 0)
            {
                int[] SpawnableEnemies = new int[] { ModContent.NPCType<Crux>(), ModContent.NPCType<Vigilante>(), ModContent.NPCType<Ventricle>() };

                int SpawnedNPC = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, Main.rand.Next(SpawnableEnemies));

                NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);
            }

            //explode with aura if it is mean to
            if (Projectile.ai[0] == 1)
            {   
                SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

                SpookyPlayer.ScreenShakeAmount = 5;

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                for (int i = 0; i <= Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        if (Main.player[i].Distance(Projectile.Center) <= Projectile.localAI[2] + time)
                        {
                            Main.player[i].Hurt(PlayerDeathReason.ByCustomReason(Main.player[i].name + " " + Language.GetTextValue("Mods.Spooky.DeathReasons.BiomassExplosion")), (Projectile.damage * 2) + Main.rand.Next(-10, 30), 0);
                        }
                    }
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