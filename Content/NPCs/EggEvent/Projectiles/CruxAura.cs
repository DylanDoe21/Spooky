using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class CruxAura : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";
        
        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath2", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] >= 60)
            {
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color glowColor = new Color(127, 127, 127, 0).MultiplyRGBA(Color.Purple);

                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/PurpleAura").Value;

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Color newColor = glowColor;
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.ai[2] / 35 + (Projectile.ai[2] < 250 ? time : time2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    
        public override void AI()
        {
            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            if (Parent.active && Parent.type == ModContent.NPCType<Crux>())
            {
                Projectile.position = Parent.Center - Projectile.Size / 2;
            }

            Projectile.ai[1]++;

            if (Projectile.ai[1] >= 60)
            {
                if (Projectile.ai[2] < 250)
                {
                    Projectile.ai[2] += 5;
                }
            }

            if (Projectile.ai[1] >= 150)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            for (int i = 0; i <= Main.maxPlayers; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead)
                {
                    if (Main.player[i].Distance(Projectile.Center) <= Projectile.ai[2] + time)
                    {
                        Main.player[i].AddBuff(BuffID.WitheredArmor, 300);
                        Main.player[i].AddBuff(BuffID.WitheredWeapon, 300);
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

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}