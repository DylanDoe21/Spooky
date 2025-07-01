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

namespace Spooky.Content.NPCs.SpookyHell.Projectiles
{
    public class NautilusBubble1 : ModProjectile
    {
        private static Asset<Texture2D> AuraTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode1", SoundType.Sound);

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
            if (Projectile.ai[0] >= 20)
            {
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/Aura");

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.Red);

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);

                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color * 0.2f, Projectile.rotation - i, drawOrigin, Projectile.localAI[1] / 37 + (Projectile.localAI[1] < 135 ? time : time2), SpriteEffects.None, 0);
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
			if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }

			return false;
		}
    
        public override void AI()
        {
            Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity *= 0.98f;

                Projectile.localAI[0]++;

                if (Projectile.localAI[1] < 135)
                {
                    Projectile.localAI[1] += 9;
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

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 2f, 450f);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;


            int Damage = Main.masterMode ? 140 : Main.expertMode ? 90 : 50;

            for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.Distance(Projectile.Center) <= Projectile.localAI[1] + time)
				{
                    player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.BubbleExplosion").ToNetworkText(player.name)), Damage + Main.rand.Next(-30, 30), 0);
                }
            }

            for (int numDusts = 0; numDusts < 35; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, Color.Red, 1f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}
        }
    }

    public class NautilusBubble2 : NautilusBubble1
    {
        private static Asset<Texture2D> AuraTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= 20)
            {
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/Aura");

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Color color = new Color(127, 127, 127, 0).MultiplyRGBA(Color.Purple);

                Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

                Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);

                for (int i = 0; i < 360; i += 90)
                {
                    Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color * 0.2f, Projectile.rotation - i, drawOrigin, Projectile.localAI[1] / 37 + (Projectile.localAI[1] < 135 ? time : time2), SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 2f, 450f);

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];

				if (player.active && !player.dead && player.Distance(Projectile.Center) <= Projectile.localAI[1] + time)
				{
                    player.AddBuff(BuffID.WitheredArmor, 600);
                    player.AddBuff(BuffID.WitheredWeapon, 600);
                }
            }

            for (int numDusts = 0; numDusts < 35; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, Color.Purple, 1f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}
        }
    }
}