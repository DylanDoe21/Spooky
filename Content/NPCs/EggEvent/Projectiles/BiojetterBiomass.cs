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
    public class BiojetterBiomass : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/EggEvent/GiantBiomassRed";

        private static Asset<Texture2D> AuraTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode2", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 70;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 20)
            {
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/GiantBiomassAura");

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Red);

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);

                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color * 0.2f, Projectile.rotation - i, drawOrigin, Projectile.localAI[1] / 37 + (Projectile.localAI[1] < 350 ? time : time2), SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 120)
            {
                Player player = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

                float goToX = player.Center.X - Projectile.Center.X;
                float goToY = player.Center.Y - Projectile.Center.Y;
                float speed = 0.2f;

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

            if (Projectile.ai[0] >= 120)
            {
                Projectile.velocity *= 0.98f;

                Projectile.localAI[0]++;

                if (Projectile.localAI[1] < 350)
                {
                    Projectile.localAI[1] += 10;
                }

                if (Projectile.localAI[0] >= 75)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 7f, 400f);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            int Damage = Main.masterMode ? 165 : Main.expertMode ? 110 : 55;

            for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.Distance(Projectile.Center) <= Projectile.localAI[1] + time)
				{
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.BiomassExplosion").ToNetworkText(player.name)), Damage + Main.rand.Next(-30, 30), 0);
                }
            }

            //spawn blood splatter
            int NumProjectiles = Main.rand.Next(8, 15);
            for (int i = 0; i < NumProjectiles; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-7, 8), Main.rand.Next(-7, 8), ModContent.ProjectileType<RedSplatter>(), 0, 0);
                }
            }

            for (int repeats = 0; repeats <= 1; repeats++)
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-8, 9), Main.rand.Next(-8, 9)), ModContent.Find<ModGore>("Spooky/BloodyBiomassGore" + numGores).Type);
                    }
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 8; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1f, 1.5f));
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}