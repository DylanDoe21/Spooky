using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.NoseCult.Projectiles
{
	public class NoseCultistBruteFlail : ModProjectile
	{
		public override string Texture => "Spooky/Content/Projectiles/SpookyHell/BoogerFlailProj";

		private const string ChainTexturePath = "Spooky/Content/Projectiles/SpookyHell/BoogerFlailChain";

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults() 
        {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults() 
        {
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyHell/BoogerFlailChain");

            Vector2 ParentCenter = new Vector2(Parent.Center.X + (Parent.direction == -1 ? -38 : 38), Parent.Center.Y + 3);
			
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
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorToParent * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			SpriteEffects spriteEffects = SpriteEffects.None;

			if (Projectile.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - oldPos / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
			}

			return true;
		}

		public override void AI() 
        {
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			if (!Parent.active || Parent.type != ModContent.NPCType<NoseCultistBrute>()) 
            {
				Projectile.Kill();
				return;
			}

			Vector2 RealFlailPosition = new Vector2(Parent.Center.X + (Parent.direction == -1 ? -38 : 38), Parent.Center.Y + 3);

			Vector2 vectorToFlailPosition = Projectile.DirectionTo(RealFlailPosition).SafeNormalize(Vector2.Zero);

			Projectile.rotation = vectorToFlailPosition.ToRotation() + MathHelper.PiOver2;

			Projectile.ai[1] += 0.75f;
			Vector2 offsetFromHand = new Vector2(Parent.direction).RotatedBy((float)Math.PI * 10f * (Projectile.ai[1] / 60f) * Parent.direction);

			offsetFromHand.X *= 3.2f;
			offsetFromHand.Y *= 2.5f;

			Projectile.Center = RealFlailPosition + offsetFromHand * 25f;
			Projectile.velocity = Vector2.Zero;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			Vector2 RealFlailPosition = new Vector2(Parent.Center.X + (Parent.direction == -1 ? -50 : 50), Parent.Center.Y);
			Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(RealFlailPosition) - RealFlailPosition;
			shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
			float hitRadius = 125f; // The length of the semi-major radius of the ellipse (the long end)
			return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
		}
	}
}