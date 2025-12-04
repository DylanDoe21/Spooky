using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class CorklidNuke : ModProjectile
    {
        float Opacity = 0f;
        float FlashOpacity = 0f;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;
        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> FlashTexture;

        public static readonly SoundStyle BeepSound = new("Spooky/Content/Sounds/CorklidBombCountdown", SoundType.Sound) { Volume = 0.7f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Trail");
            AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");
            FlashTexture ??= ModContent.Request<Texture2D>(Texture + "Flash");

            Color TrailColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.OrangeRed);
            Color AuraColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.DarkRed);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            if (Projectile.timeLeft <= 180)
            {
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, rectangle, AuraColor * 0.5f, Projectile.rotation - i, drawOrigin, Projectile.ai[1] / 37 + (Projectile.ai[1] < 600 ? time : time2), SpriteEffects.None, 0);
                }
            }

            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(TrailTexture.Value, vector + circular, rectangle, TrailColor * Opacity, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            if (FlashOpacity > 0f)
            {
                Main.EntitySpriteDraw(FlashTexture.Value, vector, rectangle, Color.White * FlashOpacity, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            if (Opacity < 1f)
            {
                Opacity += 0.005f;
            }

            if (FlashOpacity > 0f)
            {
                FlashOpacity -= 0.025f;
            }

            if (Projectile.timeLeft > 180)
            {
                if (Projectile.ai[0] >= 30 && Projectile.timeLeft % 60 == 0)
                {
                    SoundEngine.PlaySound(BeepSound, Projectile.Center);
                    FlashOpacity = 1f;
                }
            }
            else
            {
                if (Projectile.timeLeft % 20 == 0)
                {
                    SoundEngine.PlaySound(BeepSound, Projectile.Center);
                    FlashOpacity = 1f;
                }

                if (Projectile.ai[1] < 600)
                {
                    Projectile.ai[1] += 10;
                }
            }

            if (Projectile.timeLeft > 100)
            {
                Projectile.ai[0]++;
                if (Projectile.ai[0] >= 30)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 10;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = 1.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 1.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 1.2f, Pitch = -0.5f }, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 18f, 600f);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            int Damage = Main.masterMode ? 300 : Main.expertMode ? 200 : 100;

            foreach (var player in Main.ActivePlayers)
			{
				if (!player.dead && player.Distance(Projectile.Center) <= Projectile.ai[1] + time)
				{
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.CorklidNuke").ToNetworkText(player.name)), Damage + Main.rand.Next(-10, 30), 0);
                }
            }

            float maxAmount = 50;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 55f), Main.rand.NextFloat(2f, 55f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 55f), Main.rand.NextFloat(2f, 55f));
                float intensity = Main.rand.NextFloat(2f, 55f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                int Fire = Dust.NewDust(Projectile.Center, 0, 0, DustID.InfernoFork, 0f, 0f, 100, default, 5f);
                Main.dust[Fire].noGravity = true;
                Main.dust[Fire].position = Projectile.Center + vector12;
                Main.dust[Fire].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                if (currentAmount % 2 == 0)
                {
                    int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(2f, 5f));
                    Main.dust[Smoke].noGravity = true;
                    Main.dust[Smoke].position = Projectile.Center + vector12;
                    Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;
                }

                currentAmount++;
            }
		}
    }
}