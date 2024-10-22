using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SkullWisp : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> TrailTexture;
        private static Asset<Texture2D> AuraTexture;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Raven);
            Projectile.width = 20;
			Projectile.height = 28;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			Projectile.timeLeft = 2;
			Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
			AIType = ProjectileID.Raven;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.Purple, Color.OrangeRed, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.5f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			//draw aura
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyBiome/SkullWispAura");

			Vector2 AuraDrawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 360; i += 90)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.OrangeRed);

                Vector2 circular = new Vector2(Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(AuraTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, AuraDrawOrigin, Projectile.scale, effects, 0);
            }

			return true;
		}

		public override bool MinionContactDamage()
		{
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Lighting.AddLight(Projectile.Center, 0.25f, 0.12f, 0f);

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = new Vector2(Projectile.Center.X, Projectile.Center.Y + 5);
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SkullWispBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SkullWispBuff>()))
            {
				Projectile.timeLeft = 2;
			}

			//go to the player if they are too fars
			if (Projectile.Distance(player.Center) > 1200f)
			{
				Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
				Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
				Projectile.netUpdate = true;
			}

			//prevent projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= pushAway;
					}
					else
					{
						Projectile.velocity.X += pushAway;
					}
					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= pushAway;
					}
					else
					{
						Projectile.velocity.Y += pushAway;
					}
				}
			}
		}
	}
}