using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class VenomHarpoonProj : ModProjectile
	{	
		NPC GrappledNPC = null;

		float SaveRotation;

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

			Vector2 ParentCenter = player.MountedCenter + new Vector2(player.direction == -1 ? -65 : 65, -10);

			Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpiderCave/BallSpiderWeb");

			Rectangle? chainSourceRectangle = null;
			float chainHeightAdjustment = 0f;

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10;
			}

			float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

			while (chainLengthRemainingToDraw > 0f)
			{
				var chainTextureToDraw = chainTexture;

				Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, lightColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorToParent * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			return true;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			target.AddBuff(BuffID.Venom, 300);

			if (Projectile.ai[0] == 0 && Projectile.ai[1] < 15)
			{
				if (!target.boss)
				{	
					GrappledNPC = target;
				}

				Projectile.ai[1] = 15;
				Projectile.ai[0] = 1;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.ai[1] = 15;

			return false;
		}

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];
			Vector2 ParentCenter = player.MountedCenter + new Vector2(player.direction == -1 ? -65 : 65, -10);

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			switch (Projectile.ai[0])
            {
				//shooting out
				case 0: 
                {
					Projectile.ai[1]++;

					if (Projectile.ai[1] == 2)
					{
						SaveRotation = Projectile.rotation;
					}

					if (Projectile.ai[1] >= 15)
					{
						Projectile.tileCollide = false;

						Projectile.rotation = SaveRotation;

						Vector2 RetractSpeed = Projectile.Center - ParentCenter;
						RetractSpeed.Normalize();
						RetractSpeed *= 50;
						Projectile.velocity = -RetractSpeed;

						if (Projectile.Distance(player.Center) <= 100f)
						{
							Projectile.Kill();
						}
					}

                    break;
				}

				//retract to player while pulling enemy
				case 1: 
                {
					Projectile.tileCollide = false;

					Projectile.rotation = SaveRotation;
					
					Vector2 RetractSpeed = Projectile.Center - ParentCenter;
					RetractSpeed.Normalize();
					RetractSpeed *= 10;
					Projectile.velocity = -RetractSpeed;

					if (GrappledNPC != null)
					{
						if (GrappledNPC.Distance(player.Center) >= 300f)
						{
							GrappledNPC.position = Projectile.Center - GrappledNPC.Size / 2;
						}
					}

					if (Projectile.Distance(player.Center) <= 100f)
					{
						Projectile.Kill();
					}

					break;
				}
			}

			if (Projectile.Distance(player.Center) >= 1000f)
			{
				Projectile.Kill();
			}

			player.itemRotation = 0;

			if (Projectile.ai[1] > 1)
			{
				player.SetDummyItemTime(2);
			}
		}
	}
}