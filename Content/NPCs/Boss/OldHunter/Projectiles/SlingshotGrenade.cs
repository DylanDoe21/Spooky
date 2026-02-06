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
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.OldHunter.Projectiles
{
    public class SlingshotGrenade : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
                float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.YellowGreen;

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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, scale * 0.75f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Frostburn, 600);
			Projectile.Kill();
		}

        public override void AI()       
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

			Projectile.velocity.Y += 0.3f;
			if (Projectile.velocity.Y > 2f)
			{
				Projectile.velocity.Y += 0.6f;
			}

			if (Projectile.ai[0] == 0)
			{
				Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

				Vector2 ArenaOriginPosition = Flags.OldHunterPosition;

				float PositionY = target.Center.Y <= ArenaOriginPosition.Y - 185 ? ArenaOriginPosition.Y - 185 : target.Center.Y;
				Vector2 GrenadeGoTo = new Vector2(target.Center.X + (target.velocity.X / 20), PositionY);

				int MaxHeight = Projectile.Center.Y <= ArenaOriginPosition.Y ? 150 : 250;
				//int MaxVelocityX = Projectile.Center.Y <= ArenaOriginPosition.Y ? 15 : 12;
				Projectile.velocity = NPCGlobalHelper.GetArcVelocity(Projectile, GrenadeGoTo, 0.35f, MaxHeight, MaxHeight + 1, maxXvel: 15);

				//Projectile.NewProjectile(Projectile.GetSource_Death(), GrenadeGoTo, Vector2.Zero, 
                //ModContent.ProjectileType<SolarDeathbeamTelegraph>(), Projectile.damage, Projectile.knockBack);

				Projectile.ai[0]++;
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
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SlingshotGrenadeExplosion>(), Projectile.damage, Projectile.knockBack);

			float maxAmount = 15;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
				Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
				float intensity = Main.rand.NextFloat(1f, 10f);

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, -2f, 0, default, 2.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position = Projectile.Center + vector12;
				Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }
}