using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SewingNeedle : ModProjectile
	{
        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults() 
        {
			Projectile.width = 14;
			Projectile.height = 14;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/BallSpiderWeb");

            Vector2 ParentCenter = player.MountedCenter;

			Rectangle? chainSourceRectangle = null;
			float chainHeightAdjustment = 0f;

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10;
			}

			float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

			while (chainLengthRemainingToDraw > 0f)
			{
				Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, lightColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorToParent * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			return true;
		}

		int numHits = 0;

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			numHits++;
			
			if (numHits < 5)
			{
				Projectile.damage = (int)(damageDone * 1.1f);
			}

			target.AddBuff(ModContent.BuffType<PiercedDebuff>(), int.MaxValue);
		}

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];
			Vector2 ParentCenter = player.MountedCenter;

			Vector2 vectorTowardsPlayer = Projectile.DirectionTo(ParentCenter).SafeNormalize(Vector2.Zero);
			Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;

			Projectile.ai[0]++;

			if (Projectile.ai[0] >= 25)
			{
				Vector2 RetractSpeed = Projectile.Center - ParentCenter;
				RetractSpeed.Normalize();
				RetractSpeed *= 35;
				Projectile.velocity = -RetractSpeed;

				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}
	}
}