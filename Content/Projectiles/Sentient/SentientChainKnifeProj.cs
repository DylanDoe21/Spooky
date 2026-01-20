using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientChainKnifeProj : ModProjectile
	{
		int SaveDirection;
		float SaveRotation;

		int ChainOffsetY = 0;

		bool IsStickingToTarget = false;

		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> ChainTexture1;
		private static Asset<Texture2D> ChainTexture2;

		public static readonly SoundStyle BleedSound1 = new("Spooky/Content/Sounds/EggEvent/BiomassExplode2", SoundType.Sound);
		public static readonly SoundStyle BleedSound2 = new("Spooky/Content/Sounds/EggEvent/BolsterSqueeze", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
		}

        public override void SetDefaults() 
        {
			Projectile.width = 46;
			Projectile.height = 46;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
		}
		
		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            ChainTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientChainKnifeSegment1");
			ChainTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientChainKnifeSegment2");

            bool flip = false;
			if (player.direction == 1)
			{
				flip = true;
			}

			Vector2 chainDrawOrigin = new Vector2(0, ChainTexture1.Height() / 2);
			Vector2 myCenter = Projectile.Center - new Vector2(0, 16).RotatedBy(Projectile.rotation);
			Vector2 p0 = player.MountedCenter;
			Vector2 p1 = player.MountedCenter - new Vector2(IsStickingToTarget ? ChainOffsetY : 0, 16).RotatedBy(Projectile.rotation);
			Vector2 p2 = myCenter - new Vector2(IsStickingToTarget ? ChainOffsetY : 0, 16).RotatedBy(Projectile.rotation);
			Vector2 p3 = myCenter;

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			var chainEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

			if (SaveDirection != 0)
			{
				chainEffects = player.Center.X < Projectile.Center.X ? SpriteEffects.None : SpriteEffects.FlipVertically;
			}

			int segments = 36;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;
				Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				Vector2 toNext = (drawPosNext - drawPos2);
				float rotation = toNext.ToRotation();
				float distance = toNext.Length();

				lightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

				Main.spriteBatch.Draw(i % 2 == 0 ? ChainTexture2.Value : ChainTexture1.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), 
				rotation, chainDrawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), chainEffects, 0f);
			}

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			if (Projectile.ai[2] >= 120)
			{
				//draw sparkle
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;
                DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, vector, Color.Crimson, Color.Transparent, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1f * time, 1f * time), new Vector2(5, 5));
			}

			return false;
		}

		private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Projectile.owner];

			if (!IsStickingToTarget && Main.mouseLeft && Projectile.Hitbox.Intersects(target.Hitbox) && Projectile.ai[0] < 12)
			{
				SaveRotation = Projectile.rotation;
				SaveDirection = Projectile.direction;

				Projectile.ai[1] = target.whoAmI;
				Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
				IsStickingToTarget = true;
				Projectile.netUpdate = true;
			}
		}

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];
			Vector2 ParentCenter = player.MountedCenter;

			player.direction = Projectile.Center.X < player.Center.X ? -1 : 1;

			player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

			int npcTarget = (int)Projectile.ai[1];
			NPC targetedNPC = Main.npc[npcTarget];

			if (!IsStickingToTarget)
			{
				SaveDirection = 0;
				ChainOffsetY = 0;

				Projectile.frame = 0;

				Projectile.spriteDirection = player.direction;
				
				Vector2 vectorTowardsPlayer = Projectile.DirectionTo(ParentCenter).SafeNormalize(Vector2.Zero);
				Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;

				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 12)
				{
					Vector2 RetractSpeed = Projectile.Center - ParentCenter;
					RetractSpeed.Normalize();
					RetractSpeed *= 45;
					Projectile.velocity = -RetractSpeed;

					if (Projectile.Distance(player.Center) <= 55f)
					{
						Projectile.Kill();
					}
				}
			}
			else
			{
				Projectile.frame = 1;

				Projectile.rotation = SaveRotation;
				Projectile.spriteDirection = SaveDirection;

				Projectile.timeLeft = 300;

				if (player.Center.Y <= Projectile.Center.Y)
				{
					if (SaveDirection == -1)
					{
						if (ChainOffsetY < 50)
						{
							ChainOffsetY += 5;
						}
					}
					else
					{
						if (ChainOffsetY > -50)
						{
							ChainOffsetY -= 5;
						}
					}
				}
				else
				{
					if (SaveDirection == -1)
					{
						if (ChainOffsetY > -50)
						{
							ChainOffsetY -= 5;
						}
					}
					else
					{
						if (ChainOffsetY < 50)
						{
							ChainOffsetY += 5;
						}
					}
				}

				//retract immediately if the player isnt holding left click or if the projectile is too far away
				if (!Main.mouseLeft || Projectile.Distance(player.Center) >= 750f)
				{
					if (Projectile.ai[2] >= 120)
					{
						if (targetedNPC.active && !targetedNPC.dontTakeDamage)
						{
							SoundEngine.PlaySound(BleedSound1, targetedNPC.Center);
							SoundEngine.PlaySound(BleedSound2, targetedNPC.Center);

							Vector2 DustVelocity = player.Center - Projectile.Center;
							DustVelocity.Normalize();
							DustVelocity *= 12f;

							for (int numDusts = 0; numDusts < 25; numDusts++)
							{
								Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Blood,
								new Vector2(DustVelocity.X + Main.rand.Next(-7, 8), DustVelocity.Y + Main.rand.Next(-7, 8)) * Main.rand.NextFloat(0.1f, 1f), default, default, Main.rand.NextFloat(1f, 4f));
								dust.noGravity = true;
							}

							targetedNPC.AddBuff(ModContent.BuffType<SentientChainKnifeBleed>(), 300);

							player.ApplyDamageToNPC(targetedNPC, Projectile.damage * 3, 0f, default);
						}
					}

					Projectile.ai[0] = 12;
					IsStickingToTarget = false;
				}
				else
				{
					//keeps track of how long the projectile is stuck in the enemy
					Projectile.ai[2]++;

					if (npcTarget < 0 || npcTarget >= 200)
					{
						Projectile.ai[0] = 12;
						IsStickingToTarget = false;
					}
					else if (targetedNPC.active && !targetedNPC.dontTakeDamage)
					{
						Projectile.Center = targetedNPC.Center - Projectile.velocity * 2;
						Projectile.gfxOffY = targetedNPC.gfxOffY;
					}
					else
					{
						Projectile.ai[0] = 12;
						IsStickingToTarget = false;
					}
				}
			}
		}
	}
}