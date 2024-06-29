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
    public class VesicatorDeath : ModProjectile
    {
        private static Asset<Texture2D> AuraTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/VesicatorExplosion", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 132;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/VesicatorExplosionAura");

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.Red);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(AuraTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.localAI[0] / 35 + (Projectile.localAI[0] < 450 ? time : time2), SpriteEffects.None, 0);

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    
        public override void AI()
        {
            Projectile.rotation += 0.25f * (float)Projectile.direction;
            
            if (Projectile.localAI[0] < 450)
            {
                Projectile.localAI[0] += 10;
            }

            if (Projectile.localAI[0] >= 225)
            {
                Projectile.velocity.Y += 0.25f;
            }

            if (Projectile.localAI[0] >= 450 && Projectile.velocity.Y >= 15)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] >= 450 && Projectile.velocity.Y >= 15)
            {
                Projectile.Kill();
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 15;

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    if (player.Distance(Projectile.Center) <= Projectile.localAI[0] + time)
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " " + Language.GetTextValue("Mods.Spooky.DeathReasons.VesicatorExplosion")), (Projectile.damage * 2) + Main.rand.Next(-10, 30), 0);
                    }
                }
            }

            //spawn vesicator gores
            for (int numGores = 1; numGores <= 10; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-22, 22), Main.rand.Next(-15, -2)), ModContent.Find<ModGore>("Spooky/VesicatorGore" + numGores).Type);
                }
            }

            //spawn blood splatter
            int NumProjectiles = Main.rand.Next(15, 25);
            for (int i = 0; i < NumProjectiles; i++)
            {
                //chance to shoot them directly up
                if (Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 4),
                    Main.rand.Next(-8, -3), ModContent.ProjectileType<BloodSplatter>(), 0, 0, 0, 0, 0);
                }
                else
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-12, 14),
                    Main.rand.Next(-8, -1), ModContent.ProjectileType<BloodSplatter>(), 0, 0, 0, 0, 0);
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1.8f, 2.5f));
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