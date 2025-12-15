using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SpiderWarRemoteEgg : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 RealDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, lightColor, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            float RotateDirection = Projectile.velocity.ToRotation();
            float RotateSpeed = 0.05f;

            Projectile.rotation = Projectile.rotation.AngleTowards(RotateDirection - (Projectile.ai[1] == 1 ? (float)Math.PI : 0), RotateSpeed);

            Projectile.frame = (int)Projectile.ai[1];

            Projectile.tileCollide = Projectile.Center.Y > player.Center.Y;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
		}

		public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -1.5f }, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 5f, 300f);
            
            player.AddBuff(ModContent.BuffType<SpiderWarRemoteFlyBuff>(), 2);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpiderWarRemoteFly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            float maxAmount = 25;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 15f), Main.rand.NextFloat(2f, 15f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 15f), Main.rand.NextFloat(2f, 15f));
                float intensity = Main.rand.NextFloat(2f, 15f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                int WebDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Web, 0f, 0f, 100, default, 2f);
                Main.dust[WebDust].noGravity = true;
                Main.dust[WebDust].position = Projectile.Center + vector12;
                Main.dust[WebDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                if (currentAmount % 2 == 0)
                {
                    int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(200, 200, 200) * 0.5f, Main.rand.NextFloat(0.5f, 1.5f));
                    Main.dust[Smoke].noGravity = true;
                    Main.dust[Smoke].position = Projectile.Center + vector12;
                    Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;
                }

                currentAmount++;
            }
		}
    }
}
     
          






