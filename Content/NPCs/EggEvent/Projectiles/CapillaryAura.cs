using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class CapillaryAura : ModProjectile
    {
        public static List<int> BuffableNPCs = new List<int>() 
        {
            ModContent.NPCType<Crux>(),
            ModContent.NPCType<Distended>(),
            ModContent.NPCType<DistendedBrute>(),
            ModContent.NPCType<Vesicator>(),
            ModContent.NPCType<Vigilante>(),
            ModContent.NPCType<Visitant>()
        };

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/SpookyHell/EnemyDeath2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Capillary Aura");
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] >= 60)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color glowColor = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Purple);

                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/CruxAuraDraw").Value;

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Color newColor = glowColor;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.localAI[1] / 35 + (Projectile.localAI[1] < 350 ? fade : fade2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    
        public override void AI()
        {
            Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.localAI[0]++;   

            if (Projectile.localAI[0] >= 60)
            {
                if (Projectile.localAI[1] < 350)
                {
                    Projectile.localAI[1] += 15;
                }

                for (int i = 0; i < 20; i++)
                {
                    float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                    float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                    Vector2 offset = new();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * Projectile.localAI[1] + (Projectile.localAI[1] < 100 ? fade : fade2));
                    offset.Y += (float)(Math.Cos(angle) * Projectile.localAI[1] + (Projectile.localAI[1] < 100 ? fade : fade2));
                    Dust dust = Main.dust[Dust.NewDust(Projectile.Center + offset - new Vector2(4, 4), 0, 0, DustID.GemRuby, 0, 0, 100, Color.White, 1f)];
                    dust.velocity *= 0;
                    dust.noGravity = true;
                    dust.scale = 0.5f;
                }
            }

            if (Projectile.localAI[0] >= 150)
            {
                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            //inflict player with debuffs
            if (Main.LocalPlayer.Distance(Projectile.Center) <= Projectile.localAI[1] + fade2)
            {
                Main.LocalPlayer.AddBuff(BuffID.WitheredArmor, 300);
                Main.LocalPlayer.AddBuff(BuffID.WitheredWeapon, 300);
            }

            //buff enemies
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC buffTarget = Main.npc[i];
                if (buffTarget.active)
                {
                    if (BuffableNPCs.Contains(buffTarget.type) && Vector2.Distance(Projectile.Center, buffTarget.Center) < Projectile.localAI[1] + fade2 && buffTarget.type != ModContent.NPCType<Capillary>())
                    {
                        buffTarget.AddBuff(ModContent.BuffType<EggEventEnemyBuff>(), 600);
                    }
                }
            }

            for (int numDust = 0; numDust < 45; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<SpookyHellPurpleDust>(), 0f, 0f, 100, default(Color), 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                Main.dust[newDust].scale *= 1.5f;
                Main.dust[newDust].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}