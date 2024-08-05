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

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class CruxAura : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private static Asset<Texture2D> AuraTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode1", SoundType.Sound);

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
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/Aura");

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Purple);

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
      
                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);
                
                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color * 0.2f, Projectile.rotation - i, drawOrigin, Projectile.ai[2] / 35 + (Projectile.ai[2] < 250 ? time : time2), SpriteEffects.None, 0);
                }
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

            if (Parent.active && Parent.type == ModContent.NPCType<CruxBat>())
            {
                Projectile.position = Parent.Center - Projectile.Size / 2;
            }
            else
            {
                Projectile.Kill();
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

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    if (player.Distance(Projectile.Center) <= Projectile.ai[2] + time)
                    {
                        player.AddBuff(BuffID.WitheredArmor, 300);
                        player.AddBuff(BuffID.WitheredWeapon, 300);
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