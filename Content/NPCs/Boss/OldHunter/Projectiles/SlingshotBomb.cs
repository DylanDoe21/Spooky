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

namespace Spooky.Content.NPCs.Boss.OldHunter.Projectiles
{
    public class SlingshotBomb : ModProjectile
    {
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/Aura");
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
                float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Gray;

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, scale * 1.3f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			Color AuraColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.DarkRed);

			Vector2 drawOriginSprite = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            if (Projectile.timeLeft <= 90)
            {
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

				Rectangle AuraFrame = new Rectangle(0, 0, 64, 64);

                Vector2 drawOriginAura = new Vector2(AuraTexture.Width() * 0.5f, AuraTexture.Height() * 0.5f);

                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, AuraFrame, AuraColor * 0.5f, Projectile.rotation - i, drawOriginAura, Projectile.ai[1] / 37 + (Projectile.ai[1] < 225 ? time : time2), SpriteEffects.None, 0);
                }
            }

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOriginSprite, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
            
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.velocity.Y += 0.5f;
            }
			
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			if (Projectile.timeLeft <= 90)
            {
                if (Projectile.ai[1] < 225)
                {
                    Projectile.ai[1] += 5;
                }
            }

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
        }

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Volume = 1.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact with { Volume = 1.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item74 with { Volume = 1.2f, Pitch = -0.5f }, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 8f, 500f);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            int Damage = Main.masterMode ? 160 : Main.expertMode ? 125 : 70;

            foreach (var player in Main.ActivePlayers)
			{
				if (!player.dead && player.Distance(Projectile.Center) <= Projectile.ai[1] + time)
				{
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.SeaMineExplosion").ToNetworkText(player.name)), Damage + Main.rand.Next(-10, 30), 0);
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
                    int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(1f, 3f));
                    Main.dust[Smoke].noGravity = true;
                    Main.dust[Smoke].position = Projectile.Center + vector12;
                    Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;
                }

                currentAmount++;
            }
		}
    }
}