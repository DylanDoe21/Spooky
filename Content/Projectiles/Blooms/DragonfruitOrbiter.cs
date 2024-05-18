using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Terraria.Audio;

namespace Spooky.Content.Projectiles.Blooms
{
    public class DragonfruitOrbiter : ModProjectile
    {
        private static Asset<Texture2D> EyeTexture;
		private static Asset<Texture2D> LipsTexture;

		public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

		public override void PostDraw(Color lightColor)
		{
			EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/DragonfruitOrbiterEye");
			LipsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/DragonfruitOrbiterLips");

			Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			Main.spriteBatch.Draw(Projectile.ai[0] == 0 ? EyeTexture.Value : LipsTexture.Value, drawPos, null, lightColor, 0, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = Projectile.Center.X < player.Center.X ? -1 : 1;

            Projectile.rotation += 0.05f * Projectile.direction;

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().Dragonfruit)
            {
                Projectile.Kill();
            }

			int foundTarget = HomeOnTarget();
            if (foundTarget != -1)
            {
				NPC target = Main.npc[foundTarget];
				Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 15;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
			}
            else
            {
				Projectile.timeLeft = 300;

                Projectile.ai[1] += (int)Projectile.ai[2] * 0.005f;
                double rad = Projectile.ai[1] * (Math.PI / 180);
                int distance = (int)Projectile.ai[2];
                Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
			}
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 260;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            for (int numGores = 1; numGores <= 3; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/DragonfruitOrbiterGore" + numGores).Type);
                }
            }
		}
    }
}