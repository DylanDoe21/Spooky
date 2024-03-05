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
    public class ManholeBiomass : ModProjectile
    {
        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/EnemyDeath2", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] >= 60)
            {
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color glowColor = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/RedAura").Value;

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Color newColor = glowColor;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.localAI[1] / 37 + (Projectile.localAI[1] < 135 ? time : time2), SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y;
            }

			return false;
		}
    
        public override void AI()
        {
            Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;

            Player player = Main.player[Projectile.owner];
            float goToX = player.Center.X - Projectile.Center.X;
            float speed = 0.12f;

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

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 60)
            {
                Projectile.velocity *= 0.98f;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 20)
                {
                    if (Projectile.localAI[1] < 135)
                    {
                        Projectile.localAI[1] += 5;
                    }
                }

                if (Projectile.localAI[0] >= 130)
                {
                    Projectile.Kill();
                }
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
                    if (Main.player[i].Distance(Projectile.Center) <= Projectile.localAI[1] + time)
                    {
                        Main.player[i].Hurt(PlayerDeathReason.ByCustomReason(Main.player[i].name + " " + Language.GetTextValue("Mods.Spooky.DeathReasons.BiomassExplosion")), (Projectile.damage * 2) + Main.rand.Next(-10, 30), 0);
                    }
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 5; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1f, 1.2f));
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(3))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}