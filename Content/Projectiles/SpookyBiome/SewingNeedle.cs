using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SewingNeedle : ModProjectile
	{
		float SaveRotation;

        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults() 
        {
			Projectile.width = 10;
			Projectile.height = 24;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/BallSpiderWeb");

            Vector2 ParentCenter = player.MountedCenter + new Vector2(player.direction == -1 ? -65 : 65, -10);

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

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			//TODO: multiply damage here
		}

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];
			Vector2 ParentCenter = player.MountedCenter;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			player.itemRotation = 0;

			Projectile.ai[0]++;

			if (Projectile.ai[0] == 2)
			{
				//SaveRotation = Projectile.rotation;
			}

			if (Projectile.ai[0] >= 15)
			{
				Projectile.tileCollide = false;

				//Projectile.rotation = SaveRotation;

				Vector2 RetractSpeed = Projectile.Center - ParentCenter;
				RetractSpeed.Normalize();
				RetractSpeed *= 12;
				Projectile.velocity = -RetractSpeed;

				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}
	}
}