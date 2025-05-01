using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Blooms
{
    public class CucumberStomach : ModProjectile
    {
		bool StuckOnNPC = false;

		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Chain");

			Vector2 drawOriginChain = new Vector2(ChainTexture.Width() / 2, ChainTexture.Height() / 2);
			Vector2 myCenter = Projectile.Center - new Vector2(0, 2).RotatedBy(Parent.rotation);
			Vector2 p0 = player.Center;
			Vector2 p1 = player.Center;
			Vector2 p2 = myCenter - new Vector2(0, 2).RotatedBy(Parent.rotation);
			Vector2 p3 = myCenter;

			int segments = 32;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;
				Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				Vector2 toNext = drawPosNext - drawPos2;
				float rotation = toNext.ToRotation();
				float distance = toNext.Length();

				Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

				Main.EntitySpriteDraw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, drawOriginChain, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
			}

			var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 drawPos = Projectile.Center - Main.screenPosition;

			Main.EntitySpriteDraw(ProjTexture.Value, drawPos, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			if (Projectile.Center.X > player.Center.X)
            {
                Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.spriteDirection = -1;
            }

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = player.Center.X - vector.X;
			float RotateY = player.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			if (player.Distance(Parent.Center) >= 500f || !Parent.active)
			{
				Projectile.ai[0] = 1;
			}

			if (Projectile.ai[0] == 0)
			{
				Projectile.timeLeft = 600;

				float ExtraVelocity = Vector2.Distance(Projectile.Center, Parent.Center) <= 150 ? 15 : Vector2.Distance(Projectile.Center, Parent.Center) / 10;

				Vector2 desiredVelocity = Projectile.DirectionTo(Parent.Center) * ExtraVelocity;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

				if (Projectile.Hitbox.Intersects(Parent.Hitbox))
				{
					Projectile.Center = Parent.Center;
					Projectile.gfxOffY = Parent.gfxOffY;

					if (Projectile.scale > 1)
					{
						Projectile.scale -= 0.1f;
					}

					//inflict damage over time
					Parent.AddBuff(ModContent.BuffType<CucumberStomachDebuff>(), 2);

					//heal the player, more frequently if raining
					int HealingTime = Main.raining ? 70 : 100;

					Projectile.ai[1]++;
					if (Projectile.ai[1] >= HealingTime)
					{
						SoundEngine.PlaySound(SoundID.Item177 with { Pitch = -1f }, Projectile.Center);

						Projectile.scale = 1.5f;

						float HealAmount = player.statLifeMax2 * 0.02f;
						
						player.statLife += (int)HealAmount;
						player.HealEffect((int)HealAmount, true);

						Projectile.ai[1] = 0;
					}
				}
			}
			else
			{
				Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 35;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}
    }
}